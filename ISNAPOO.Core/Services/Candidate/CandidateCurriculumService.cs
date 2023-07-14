using Data.Models.Common;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.DOC;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace ISNAPOO.Core.Services.Candidate
{
    public class CandidateCurriculumService : BaseService, ICandidateCurriculumService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly ISettingService settingService;
        private readonly ILogger<CandidateCurriculumService> _logger;

        #region Index import Curriculum
        private int professionalTrainingIndex = 0;
        private int subjectIndex = 1;
        private int topicIndex = 2;
        private int theoryIndex = 3;
        private int practiceIndex = 4;
        #endregion

        public CandidateCurriculumService(
            IRepository repository,
            IDataSourceService dataSourceService,
            ISettingService settingService,
            ILogger<CandidateCurriculumService> logger,
            AuthenticationStateProvider authenticationStateProvider)
            : base(repository, authenticationStateProvider)
        {
            this._logger = logger;
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.settingService = settingService;
        }

        public async Task<IEnumerable<CandidateCurriculumVM>> GetAllCandidateCurriculumsAsync()
        {
            var data = this.repository.AllReadonly<CandidateCurriculum>();

            return await data.To<CandidateCurriculumVM>().ToListAsync();
        }

        public async Task<IEnumerable<CandidateCurriculumVM>> GetAllCurriculumsByIdCandidateProviderSpecialityAsync(int idCandidateProviderSpeciality)
        {
            var data = this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateProviderSpeciality == idCandidateProviderSpeciality);

            return await data.To<CandidateCurriculumVM>(x => x.CandidateCurriculumERUs.Select(y => y.ERU)).ToListAsync();
        }

        public MemoryStream CreateExcelWithErrors(ResultContext<List<CandidateCurriculumVM>> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешките:";
                //sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    //var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    //var msg = splitMsg[0].Trim();
                    //var cell = splitMsg[1].Trim();

                    //sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"A{rowCounter}"].Text = item;
                    //sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        public async Task<ResultContext<List<CandidateCurriculumVM>>> ImportCurriculumAsync(MemoryStream file, string fileName, int idCandidateCurriculumModification)
        {
            ResultContext<List<CandidateCurriculumVM>> resultContext = new ResultContext<List<CandidateCurriculumVM>>();

            List<CandidateCurriculumVM> candidateCurriculumVMs = new List<CandidateCurriculumVM>();

            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportCandidateCurriculum";
                var filePath = settingResource + filePathMain;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }

                var path = @"" + filePath + "\\" + fileName;

                using (FileStream filestream = new FileStream(path, FileMode.Create, FileAccess.Write))
                {
                    file.WriteTo(filestream);
                    filestream.Close();
                    file.Close();
                }

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        IApplication app = excelEngine.Excel;

                        IWorkbook workbook = app.Workbooks.Open(fileStream, ExcelOpenType.Automatic);

                        IWorksheet worksheet = workbook.Worksheets[0];
                        if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[3].Text)
                            || string.IsNullOrEmpty(worksheet.Rows[0].Columns[4].Text))
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на учебна програма!");
                            return resultContext;
                        }

                        var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                        var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                        var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                        var fourthHeader = worksheet.Rows[0].Columns[3].Text.Trim();
                        var fifthHeader = worksheet.Rows[0].Columns[4].Text.Trim();
                        bool skipFirstRow = true;

                        //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                        if (firstHeader != "Раздел" || secondHeader != "Предмет" || thirdHeader != "Тема" || fourthHeader != "Теория" || fifthHeader != "Практика")
                        {
                            resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на учебна програма!");
                            return resultContext;
                        }

                        var rowCounter = 5;
                        foreach (var row in worksheet.Rows)
                        {
                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(row.Cells[professionalTrainingIndex].Value))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            var professionalTraining = row.Cells[professionalTrainingIndex].Value.Trim();
                            if (professionalTraining[0] == 'A' || professionalTraining[0] == 'a' || professionalTraining[0] == 'А' || professionalTraining[0] == 'а')
                            {
                                if (!int.TryParse(professionalTraining[1].ToString(), out int value) || int.Parse(professionalTraining[1].ToString()) < 1 || int.Parse(professionalTraining[1].ToString()) > 3)
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter} не отговаря на изискванията за вид на професионална подготовка!");
                                }
                                else
                                {
                                    professionalTraining = "A" + professionalTraining[1];
                                }
                            }
                            else if (professionalTraining == "Б" || professionalTraining == "б")
                            {
                                professionalTraining = "B";
                            }
                            else
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter} не отговаря на изискванията за вид на професионална подготовка!");
                            }

                            var subject = row.Cells[subjectIndex].Value.Trim();
                            var topic = row.Cells[topicIndex].Value.Trim();
                            var theory = row.Cells[theoryIndex].Value.Trim();
                            var practice = row.Cells[practiceIndex].Value.Trim();

                            var candidateCurriculum = new CandidateCurriculumVM();
                            candidateCurriculum.IdCandidateCurriculumModification = idCandidateCurriculumModification;

                            candidateCurriculum.UploadedFileName = "#";

                            var keyValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("ProfessionalTraining", professionalTraining);
                            if (keyValue != null)
                            {
                                candidateCurriculum.IdProfessionalTraining = keyValue.IdKeyValue;
                                candidateCurriculum.ProfessionalTraining = keyValue.DefaultValue1;
                            }

                            candidateCurriculum.Subject = subject;
                            if (string.IsNullOrEmpty(candidateCurriculum.Subject))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Предмет' е задължително!");
                            }
                            else if (candidateCurriculum.Subject.Length > 1000)
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Предмет' не може да съдържа повече от 1000 символа!");
                            }

                            candidateCurriculum.Topic = topic;
                            if (string.IsNullOrEmpty(candidateCurriculum.Topic))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Тема' е задължително!");
                            }
                            else if (candidateCurriculum.Topic.Length > 4000)
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: полето 'Тема' не може да съдържа повече от 4000 символа!");
                            }

                            if (string.IsNullOrEmpty(theory) && string.IsNullOrEmpty(practice))
                            {
                                resultContext.AddErrorMessage($"Ред {rowCounter}: при едно от полетата 'Теория' или 'Практика' трябва да има поне една въведена стойност!");
                            }

                            if (!string.IsNullOrEmpty(theory))
                            {
                                if (double.TryParse(theory, out double value))
                                {
                                    candidateCurriculum.Theory = double.Parse(theory);
                                    if (candidateCurriculum.Theory < 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само положително число!");
                                    }
                                    else if (candidateCurriculum.Theory % 1 != 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само цяло число!");
                                    }
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Теория' може да бъде само цяло число!");
                                }
                            }

                            if (!string.IsNullOrEmpty(practice))
                            {
                                if (double.TryParse(practice, out double value))
                                {
                                    candidateCurriculum.Practice = double.Parse(practice);
                                    if (candidateCurriculum.Practice < 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само положително число!");
                                    }
                                    else if (candidateCurriculum.Practice % 1 != 0)
                                    {
                                        resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само цяло число!");
                                    }
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Ред {rowCounter}: стойността на полето 'Практика' може да бъде само цяло число!");
                                }
                            }

                            candidateCurriculumVMs.Add(candidateCurriculum);

                            rowCounter++;
                        }
                    }

                    if (candidateCurriculumVMs.Any())
                    {
                        resultContext.AddMessage("Импортът приключи успешно!");
                    }
                    else
                    {
                        resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за учебна програма!");
                    }

                    resultContext.ResultContextObject = candidateCurriculumVMs;
                }
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<CandidateCurriculumVM>> AddCandidateCurriculumAsync(ResultContext<CandidateCurriculumVM> inputContext, bool ignoreErus = false, bool callFromImportModal = false)
        {
            ResultContext<CandidateCurriculumVM> resultContext = new ResultContext<CandidateCurriculumVM>();

            try
            {
                var keyValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                resultContext.ResultContextObject = inputContext.ResultContextObject;
                var professionalTraining = keyValues.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = professionalTraining;

                var candidateCurriculumForDb = inputContext.ResultContextObject.To<CandidateCurriculum>();
                if (callFromImportModal)
                {
                    var candidatecurriculumFromDb = await this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateCurriculumModification == inputContext.ResultContextObject.IdCandidateCurriculumModification && x.IdProfessionalTraining == inputContext.ResultContextObject.IdProfessionalTraining && x.Subject == inputContext.ResultContextObject.Subject && x.Topic == inputContext.ResultContextObject.Topic).Include(x => x.CandidateProviderSpeciality).ToListAsync();
                    var candidateProviderWithCurriculum = candidatecurriculumFromDb.FirstOrDefault();
                    candidateCurriculumForDb.CandidateProviderSpeciality = null;
                    candidateCurriculumForDb.CandidateCurriculumERUs = null;
                    if (candidateProviderWithCurriculum is null)
                    {
                        await this.repository.AddAsync<CandidateCurriculum>(candidateCurriculumForDb);
                    }
                    else
                    {
                        candidateCurriculumForDb.IdCreateUser = candidateProviderWithCurriculum.IdCreateUser;
                        candidateCurriculumForDb.CreationDate = candidateProviderWithCurriculum.CreationDate;
                        candidateCurriculumForDb.IdCandidateCurriculum = candidateProviderWithCurriculum.IdCandidateCurriculum;
                        this.repository.Update<CandidateCurriculum>(candidateCurriculumForDb);
                    }

                    await this.repository.SaveChangesAsync();
                }
                else
                {
                    candidateCurriculumForDb.CandidateProviderSpeciality = null;
                    candidateCurriculumForDb.CandidateCurriculumERUs = null;

                    await this.repository.AddAsync<CandidateCurriculum>(candidateCurriculumForDb);
                    await this.repository.SaveChangesAsync();

                }

                inputContext.ResultContextObject.IdCandidateCurriculum = candidateCurriculumForDb.IdCandidateCurriculum;
                inputContext.ResultContextObject.CreationDate = candidateCurriculumForDb.CreationDate;
                inputContext.ResultContextObject.ModifyDate = candidateCurriculumForDb.ModifyDate;

                if (!ignoreErus)
                {
                    await this.HandleCurriculumERUsAsync(inputContext.ResultContextObject.SelectedERUs, candidateCurriculumForDb.IdCandidateCurriculum);
                }

                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<CandidateCurriculumVM>> UpdateCandidateCurriculumAsync(ResultContext<CandidateCurriculumVM> inputContext)
        {
            ResultContext<CandidateCurriculumVM> resultContext = new ResultContext<CandidateCurriculumVM>();

            try
            {
                var keyValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProfessionalTraining");

                resultContext.ResultContextObject = inputContext.ResultContextObject;
                var professionalTraining = keyValues.FirstOrDefault(x => x.IdKeyValue == resultContext.ResultContextObject.IdProfessionalTraining).DefaultValue1;
                resultContext.ResultContextObject.ProfessionalTraining = professionalTraining;

                var candidateCurriculumFromDb = await this.repository.GetByIdAsync<CandidateCurriculum>(inputContext.ResultContextObject.IdCandidateCurriculum);
                inputContext.ResultContextObject.IdCreateUser = candidateCurriculumFromDb.IdCreateUser;
                inputContext.ResultContextObject.CreationDate = candidateCurriculumFromDb.CreationDate;
                candidateCurriculumFromDb = inputContext.ResultContextObject.To<CandidateCurriculum>();
                candidateCurriculumFromDb.CandidateProviderSpeciality = null;
                candidateCurriculumFromDb.CandidateCurriculumERUs = null;

                this.repository.Update<CandidateCurriculum>(candidateCurriculumFromDb);
                await this.repository.SaveChangesAsync();

                await this.HandleCurriculumERUsAsync(inputContext.ResultContextObject.SelectedERUs, candidateCurriculumFromDb.IdCandidateCurriculum);
                resultContext.ResultContextObject = candidateCurriculumFromDb.To<CandidateCurriculumVM>();
                resultContext.AddMessage("Записът е успешен!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);

                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> DeleteCandidateCurriculumAsync(int idCandidateCurriculum)
        {
            var resultContext = new ResultContext<NoResult>();

            try
            {
                var data = this.repository.AllReadonly<CandidateCurriculum>(x => x.IdCandidateCurriculum == idCandidateCurriculum)
                    .Include(x => x.CandidateCurriculumERUs).AsNoTracking()
                    .FirstOrDefault();

                if (data is not null)
                {
                    if (data.CandidateCurriculumERUs.Any())
                    {
                        this.repository.HardDeleteRange<CandidateCurriculumERU>(data.CandidateCurriculumERUs);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<CandidateCurriculum>(data.IdCandidateCurriculum);
                    await this.repository.SaveChangesAsync();

                    resultContext.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public async Task<ResultContext<NoResult>> DeleteListCandidateCurriculumAsync(List<CandidateCurriculumVM> candidateCurriculums)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                foreach (var candidate in candidateCurriculums)
                {
                    var candidateCurriculumERUs = await this.repository.AllReadonly<CandidateCurriculumERU>(x => x.IdCandidateCurriculum == candidate.IdCandidateCurriculum).ToListAsync();
                    foreach (var eru in candidateCurriculumERUs)
                    {
                        await this.repository.HardDeleteAsync<CandidateCurriculumERU>(eru.IdCandidateCurriculumERU);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<CandidateCurriculum>(candidate.IdCandidateCurriculum);
                    await this.repository.SaveChangesAsync();
                }

                var msg = candidateCurriculums.Count == 1 ? "Записът е изтрит успешно!" : "Записите са изтрити успешно!";
                resultContext.AddMessage(msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                resultContext.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return resultContext;
        }

        private async Task HandleCurriculumERUsAsync(List<ERUVM> erus, int idCandidateCurriculum)
        {
            foreach (var eru in erus)
            {
                var candidateCurriculumERU = this.repository.AllReadonly<CandidateCurriculumERU>(x => x.IdCandidateCurriculum == idCandidateCurriculum && x.IdERU == eru.IdERU).FirstOrDefault();
                if (candidateCurriculumERU is null)
                {
                    CandidateCurriculumERU curriculumERU = new CandidateCurriculumERU()
                    {
                        IdCandidateCurriculum = idCandidateCurriculum,
                        IdERU = eru.IdERU,
                    };

                    await this.repository.AddAsync<CandidateCurriculumERU>(curriculumERU);
                    await this.repository.SaveChangesAsync();
                }
            }
        }
    }
}
