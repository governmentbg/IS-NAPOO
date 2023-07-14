using Data.Models.Common;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System.Linq;
using Data.Models.Data.DOC;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using System.IO;
using Syncfusion.XlsIO;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Common.Framework;
using Data.Models.Data.Candidate;

namespace ISNAPOO.Core.Services.DOC
{
    public class DOCService : BaseService, IDOCService
    {
        private readonly IRepository repository;
        private readonly ILogger<DOCService> _logger;
        private readonly ISpecialityService specialityService;
        private readonly ISettingService settingService;
        private readonly IDataSourceService dataSourceService;

        #region Index import dos
        private int docNameIndex = 0;
        private int docRegulationIndex = 1;
        private int docStartDateIndex = 2;
        private int professionIndex = 3;
        private int specialiesIndex = 4;
        private int RequirementsCandidatesIndex = 5;
        private int descriptionProfessionIndex = 6;
        private int nkpdsIndex = 7;
        private int eruCodeIndex = 8;
        private int eruNameIndex = 9;
        private int eruTypeProffesionIndex = 10;
        private int NKRIndex = 11;
        private int EKRIndex = 12;
        private int ruTexkIndex = 13;
        private int requirementsMaterialBaseIndex = 14;
        private int requirementsТrainersIndex = 15;
        private int newspaperNumberIndex = 16;
        private int publishDateIndex = 17;


        #endregion

        public DOCService(IRepository repository, ILogger<DOCService> logger, ISpecialityService specialityService,
            IDataSourceService dataSourceService, ISettingService settingService) : base(repository)
        {
            this.repository = repository;
            this._logger = logger;
            this.specialityService = specialityService;
            this.settingService = settingService;
            this.dataSourceService = dataSourceService;
        }

        public async Task<DocVM> GetActiveDocByProfessionIdAsync(ProfessionVM professionVM)
        {

            Data.Models.Data.DOC.DOC doc = await this.repository.AllReadonly<Data.Models.Data.DOC.DOC>(FilterByProfessionId(professionVM)).Include(x => x.ERUs).Include(x => x.docNkpds).FirstOrDefaultAsync();

            if (doc == null)
            {
                return null;
            }

            return doc.To<DocVM>();
        }
        public async Task<List<DocVM>> GetAllActiveDocAsync()
        {

            List<Data.Models.Data.DOC.DOC> doc = this.repository.AllReadonly<Data.Models.Data.DOC.DOC>(p => p.IdStatus == dataSourceService.GetActiveStatusID()).ToList();
            if (doc == null)
            {
                return null;
            }
            List<DocVM> result = new List<DocVM>();
            foreach (var temp in doc)
            {
                result.Add(temp.To<DocVM>());
            }


            return result;
        }

        public async Task<IEnumerable<DocVM>> GetAllDocAsync()
        {
            var kvStatusValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusSPPOO");

            var doc = this.repository.All<Data.Models.Data.DOC.DOC>().Include(x => x.ERUs);

            IEnumerable<DocVM> result = doc.To<DocVM>(x => x.Profession, x => x.Specialities, x => x.DOCNKPDs, x => x.ERUs).ToList();

            foreach (var item in result)
            {
                var specialities = item.Specialities.Select(x => x.ComboBoxName).ToList();
                item.SpecialitiesJoin = string.Join("<br />", specialities);
                item.StatusName = kvStatusValues.FirstOrDefault(s => s.IdKeyValue == item.IdStatus).Name;
            }

            return result;
        }
        public async Task<IEnumerable<DocVM>> GetAllDOCByStatus(string statusName)
        {
            var kvStatusValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", statusName);

            var doc = this.repository.All<Data.Models.Data.DOC.DOC>(d => d.IdStatus == kvStatusValue.IdKeyValue);

            IEnumerable<DocVM> result = doc.To<DocVM>(x => x.Profession, x => x.Specialities, x => x.DOCNKPDs).ToList();

            foreach (var item in result)
            {
                var specialities = item.Specialities.Select(x => x.ComboBoxName).ToList();
                item.SpecialitiesJoin = string.Join("<br />", specialities);
                item.StatusName = statusName;
            }

            return result;
        }

        public async Task<IEnumerable<DocVM>> GetAllDocAsync(DocVM filterDocVM) 
        {
            var kvStatusValues = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("StatusSPPOO");

            var doc = this.repository.All<Data.Models.Data.DOC.DOC>(FilterDocValue(filterDocVM));

            IEnumerable<DocVM> result = await doc.To<DocVM>(x => x.Profession, x => x.Specialities, x => x.DOCNKPDs).ToListAsync();

            foreach (var item in result)
            {
                var specialities = item.Specialities.Select(x => x.ComboBoxName).ToList();
                item.SpecialitiesJoin = string.Join("<br />", specialities);
                item.StatusName = kvStatusValues.FirstOrDefault(s => s.IdKeyValue == item.IdStatus).Name;
            }

            return result;
        }

        public async Task<DocVM> GetDOCByIdAsync(DocVM model)
        {
            var data = await this.repository.All<Data.Models.Data.DOC.DOC>(x => x.IdDOC == model.IdDOC)
                .Include(x => x.docNkpds)
                .FirstOrDefaultAsync();

            this.repository.Detach<Data.Models.Data.DOC.DOC>(data);

            DocVM docViewModel = data.To<DocVM>();

            return docViewModel;

        }

        public async Task<string> UpdateDOCAsync(DocVM model)
        {
            try
            {
                var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Active");

                var updatedEnity = await this.GetByIdAsync<Data.Models.Data.DOC.DOC>(model.IdDOC);


                this.repository.Detach<Data.Models.Data.DOC.DOC>(updatedEnity);

                updatedEnity = model.To<Data.Models.Data.DOC.DOC>();

                updatedEnity.Specialities = null;
                updatedEnity.docNkpds = null;

                this.repository.Update(updatedEnity);
                var result = await this.repository.SaveChangesAsync();


                foreach (var item in model.Specialities)
                {
                    Speciality speciality = await this.repository.GetByIdAsync<Speciality>(item.IdSpeciality);

                    speciality.IdDOC = updatedEnity.IdDOC;
                    this.repository.Update(speciality);
                    await this.repository.SaveChangesAsync();
                }


                await SaveNKPDsToDoc(model.IdDOC, model.IdsNkpd);

                string msg;
                if (result > 0)
                {
                    model.IdModifyUser = updatedEnity.IdModifyUser;
                    model.ModifyDate = updatedEnity.ModifyDate;

                    msg = "Записът e успешeн!";
                    _logger.LogInformation(msg);
                }
                else
                {
                    msg = "Грешка при запис в базата!";
                    _logger.LogError(msg);
                }

                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return ex.Message;

            }

        }

        public async Task<bool> CheckForActiveDocWithSameProfession(DocVM model)
        {

            var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Active");

            if (model.IdStatus == kvActiveStatus.IdKeyValue)
            {
                var filter = new DocVM
                {
                    IdStatus = model.IdStatus,
                    IdProfession = model.IdProfession,
                };

                var list = await GetAllDocAsync(filter);

                if (list != null && list.Count() > 0)
                {
                    //Ако редактираме същия дос в момента трябва ни пусне
                    if (list.Count() == 1)
                    {
                        var docDb = list.FirstOrDefault();
                        if (docDb.IdDOC == model.IdDOC)
                        {
                            return false;
                        }
                    }

                    return true;
                }
            }

            return false;
        }

        public async Task<bool> CheckForActiveDocWithSameSpeciality(DocVM model)
        {

            var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Active");
            var specIds = model.Specialities.Select(x => x.IdSpeciality).ToList();

            if (model.IdStatus == kvActiveStatus.IdKeyValue)
            {
                var filter = new DocVM
                {
                    IdStatus = model.IdStatus,
                };

                var list = await GetAllDocAsync(filter);

                if (list != null && list.Count() > 0)
                {
                    foreach (var doc in list)
                    {
                        //Ако редактираме същия дос в момента трябва ни пусне
                        if (doc.IdDOC == model.IdDOC)
                        {
                            continue;
                        }

                        foreach (var item in doc.Specialities)
                        {
                            if (specIds.Contains(item.IdSpeciality))
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }

        public async Task<string> CreateDOCAsync(DocVM model)
        {

            string msg;

            try
            {
                var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Active");

                var data = model.To<Data.Models.Data.DOC.DOC>();

                data.Specialities = null;
                await this.repository.AddAsync<Data.Models.Data.DOC.DOC>(data);

                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    model.IdDOC = data.IdDOC;
                    model.IdCreateUser = data.IdCreateUser;
                    model.CreationDate = data.CreationDate;
                    model.IdModifyUser = data.IdModifyUser;
                    model.ModifyDate = data.ModifyDate;


                    foreach (var item in model.Specialities)
                    {
                        Speciality speciality = await this.repository.GetByIdAsync<Speciality>(item.IdSpeciality);

                        speciality.IdDOC = data.IdDOC;
                        this.repository.Update(speciality);
                        result = await this.repository.SaveChangesAsync();
                    }


                    await SaveNKPDsToDoc(model.IdDOC, model.IdsNkpd);

                    if (result > 0)
                    {
                        msg = "Записът e успешeн!";
                    }
                    else
                    {
                        msg = "Грешка при запис в базата!";
                    }
                }
                else
                {
                    msg = "Грешка при запис в базата.";
                }

                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return ex.Message;
            }

        }

        private async Task SaveNKPDsToDoc(int idDoc, List<int> IdsNkpd)
        {
            int result = GlobalConstants.INVALID_ID_ZERO;
            var nkpdsByDoc = await this.repository.All<DOC_DOC_NKPD>(x => x.IdDOC == idDoc).Select(x => x.IdNKPD).ToListAsync();
            foreach (var id in IdsNkpd)
            {
                if (nkpdsByDoc.Contains(id))
                {
                    continue;
                }

                DOC_DOC_NKPD docNKPD = new DOC_DOC_NKPD
                {
                    IdNKPD = id,
                    IdDOC = idDoc
                };

                await this.repository.AddAsync<DOC_DOC_NKPD>(docNKPD);
            }

            if (IdsNkpd.Count > 0)
            {
                result = await this.repository.SaveChangesAsync();
            }

        }

        private async Task SaveNKPDsToSpecialities(List<SpecialityVM> listSpec, List<int> IdsNkpd)
        {
            int result = GlobalConstants.INVALID_ID_ZERO;

            foreach (var item in listSpec)
            {

                var nkpdsBySpec = await this.repository.All<SpecialityNKPD>(x => x.IdSpeciality == item.IdSpeciality).Select(x => x.IdNKPD).ToListAsync();
                foreach (var id in IdsNkpd)
                {
                    if (nkpdsBySpec.Contains(id))
                    {
                        continue;
                    }

                    SpecialityNKPD specNKPD = new SpecialityNKPD
                    {
                        IdNKPD = id,
                        IdSpeciality = item.IdSpeciality,
                    };

                    await this.repository.AddAsync<SpecialityNKPD>(specNKPD);
                }
            }

            if (IdsNkpd.Count > 0)
            {
                result = await this.repository.SaveChangesAsync();
            }

        }

        private async Task<int> DelteAllNKPDsFromDocByIdDoc(int idDoc)
        {
            try
            {

                int result = GlobalConstants.INVALID_ID_ZERO;

                List<DOC_DOC_NKPD> docNKPDs = this.repository.AllReadonly<DOC_DOC_NKPD>(x => x.IdDOC == idDoc).AsNoTracking().ToList();
                this.repository.HardDeleteRange<DOC_DOC_NKPD>(docNKPDs);
                result = await this.repository.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return GlobalConstants.INVALID_ID_ZERO;

            }
        }

        public async Task<int> DelteNKPDFromDocById(int idNKPD, int idDoc)
        {
            try
            {
                int result = GlobalConstants.INVALID_ID_ZERO;

                List<DOC_DOC_NKPD> docNKPDs = this.repository.AllReadonly<DOC_DOC_NKPD>(x => x.IdDOC == idDoc).AsNoTracking().ToList();
                var nkpdDB = docNKPDs.FirstOrDefault(x => x.IdNKPD == idNKPD);

                if (nkpdDB != null)
                {
                    this.repository.HardDelete<DOC_DOC_NKPD>(nkpdDB);
                    result = await this.repository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return GlobalConstants.INVALID_ID_ZERO;

            }
        }

        public async Task<int> DelteERUById(int idEru)
        {
            try
            {
                int result = GlobalConstants.INVALID_ID_ZERO;


                List<ERUSpeciality> eRUSpecialities = this.repository.All<ERUSpeciality>(x => x.IdERU == idEru).AsNoTracking().ToList();
                if (eRUSpecialities != null && eRUSpecialities.Count > GlobalConstants.INVALID_ID_ZERO)
                {
                    this.repository.HardDeleteRange<ERUSpeciality>(eRUSpecialities);
                    result = await this.repository.SaveChangesAsync();
                }

                List<CandidateCurriculumERU> candidateCurriculumERUs = this.repository.All<CandidateCurriculumERU>(x => x.IdERU == idEru).AsNoTracking().ToList();
                if (candidateCurriculumERUs != null && candidateCurriculumERUs.Count > GlobalConstants.INVALID_ID_ZERO)
                {
                    this.repository.HardDeleteRange<CandidateCurriculumERU>(candidateCurriculumERUs);
                    result = await this.repository.SaveChangesAsync();
                }

                ERU eruDb = await this.repository.GetByIdAsync<ERU>(idEru);
                if (eruDb != null)
                {
                    this.repository.HardDelete<ERU>(eruDb);
                    result = await this.repository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return GlobalConstants.INVALID_ID_ZERO;

            }
        }

        public async Task<int> DelteDocById(int idDoc)
        {
            try
            {
                int result = GlobalConstants.INVALID_ID_ZERO;


                List<ERU> eRUs = this.repository.All<ERU>(x => x.IdDOC == idDoc).AsNoTracking().ToList();

                foreach (var eru in eRUs)
                {
                    await DelteERUById(eru.IdERU);
                }

                List<Speciality> specialities = this.repository.All<Speciality>(x => x.IdDOC == idDoc).AsNoTracking().ToList();
                foreach (var spec in specialities)
                {
                    await RemoveDocFromSpecialityById(spec.IdSpeciality);
                }

                List<DOC_DOC_NKPD> docNKPDs = this.repository.All<DOC_DOC_NKPD>(x => x.IdDOC == idDoc).AsNoTracking().ToList();
                if (docNKPDs != null && docNKPDs.Count > GlobalConstants.INVALID_ID_ZERO)
                {
                    this.repository.HardDeleteRange<DOC_DOC_NKPD>(docNKPDs);
                    result = await this.repository.SaveChangesAsync();
                }

                var docDb = await this.repository.GetByIdAsync<Data.Models.Data.DOC.DOC>(idDoc);
                if (docDb != null)
                {
                    this.repository.HardDelete<Data.Models.Data.DOC.DOC>(docDb);
                    result = await this.repository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return GlobalConstants.INVALID_ID_ZERO;

            }
        }

        public async Task<int> RemoveDocFromSpecialityById(int idSpeciality)
        {
            try
            {
                int result = GlobalConstants.INVALID_ID_ZERO;

                var specDB = await this.repository.GetByIdAsync<Speciality>(idSpeciality);

                if (specDB != null)
                {
                    //this.repository.Detach<Speciality>(specDB);

                    specDB.IdDOC = null;

                    this.repository.Update(specDB);
                    result = await this.repository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return GlobalConstants.INVALID_ID_ZERO;

            }
        }

        public async Task<int> DelteSpecialityFromERUById(int idSpeciality, int idERU)
        {
            try
            {
                int result = GlobalConstants.INVALID_ID_ZERO;

                List<ERUSpeciality> eruSpecilities = this.repository.AllReadonly<ERUSpeciality>(x => x.IdERU == idERU).AsNoTracking().ToList();
                var specEruDB = eruSpecilities.FirstOrDefault(x => x.IdSpeciality == idSpeciality);

                if (specEruDB != null)
                {
                    // this.repository.Detach<ERUSpeciality>(specEruDB);
                    this.repository.HardDelete<ERUSpeciality>(specEruDB);
                    result = await this.repository.SaveChangesAsync();
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Message - {ex.Message}");
                _logger.LogError($"InnerException - {ex.InnerException}");
                _logger.LogError($"StackTrace - {ex.StackTrace}");
                return GlobalConstants.INVALID_ID_ZERO;

            }
        }

        protected Expression<Func<Data.Models.Data.DOC.DOC, bool>> FilterDocValue(DocVM model)
        {
            var predicate = PredicateBuilder.True<Data.Models.Data.DOC.DOC>();

            if (model.IdProfession > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdProfession == model.IdProfession);
            }

            if (model.IdStatus > GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdStatus == model.IdStatus);
            }

            return predicate;
        }

        protected Expression<Func<Data.Models.Data.DOC.DOC, bool>> FilterByProfessionId(ProfessionVM professionVM)
        {
            var predicate = PredicateBuilder.True<Data.Models.Data.DOC.DOC>();

            predicate = predicate.And(p => p.IdProfession == professionVM.IdProfession && p.IdStatus == dataSourceService.GetActiveStatusID());

            return predicate;
        }

        public async Task<ResultContext<DocVM>> ImportDOCAsync(ResultContext<DocVM> resultContext, MemoryStream file, string fileName)
        {
            try
            {
                var kvTypeProfessionList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.ProfessionalTraining);
                var kvNKRList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.NKRLevel);
                var kvEKRList = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(SPPOOValues.EKRLevel);
                var kvDraftStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Draft");
                var kvActiveStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Active");

                int counter = GlobalConstants.INVALID_ID_ZERO;
                int result = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName");
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportDOS";
                var filePath = settingResource.SettingValue + filePathMain;

                var isAddNewRecord = false;

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
                        var firstHeader = worksheet.Rows[0].Columns[0].Text;
                        bool skipFirstRow = true;
                        //Проверка по 1 клетка за да се провери дали файла за импорт на дос
                        if (firstHeader != "Наименование на документа, съдържащ ДОС")
                        {
                            resultContext.AddMessage("Файлът, който се опитвате да качите не отговаря на шаблона за импорт на ДОС!");
                            return resultContext;
                        }



                        foreach (var row in worksheet.Rows)
                        {
                            var isReadyForSave = true;

                            //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                            if (counter == 5)
                            {
                                break;
                            }

                            //Пропуска 1 ред който е с хедърите
                            if (skipFirstRow || string.IsNullOrEmpty(row.Cells[docNameIndex].Value))
                            {
                                skipFirstRow = false;
                                counter++;
                                continue;
                            }

                            var docName = row.Cells[docNameIndex].Value;
                            var docRegulationName = row.Cells[docRegulationIndex].Value;
                            var docStartDate = row.Cells[docStartDateIndex].Value;
                            var professionCode = row.Cells[professionIndex].Value;
                            var specialitiesCodes = row.Cells[specialiesIndex].Value;
                            var reqCandidates = row.Cells[RequirementsCandidatesIndex].Value;
                            var descProfession = row.Cells[descriptionProfessionIndex].Value;
                            var nkpds = row.Cells[nkpdsIndex].Value;
                            var eruCode = row.Cells[eruCodeIndex].Value;
                            var eruName = row.Cells[eruNameIndex].Value;
                            var eruTypeProfession = row.Cells[eruTypeProffesionIndex].Value;
                            var nkrLevel = row.Cells[NKRIndex].Value;
                            var ekrLevel = row.Cells[EKRIndex].Value;
                            var ruText = row.Cells[ruTexkIndex].Value;
                            var reqBase = row.Cells[requirementsMaterialBaseIndex].Value;
                            var reqTrainers = row.Cells[requirementsТrainersIndex].Value;
                            var newspaperNumber = row.Cells[newspaperNumberIndex].Value;
                            var publishDate = row.Cells[publishDateIndex].Value;

                            //Вземаме ид на ДОС ако вече го има или го създаваме
                            var docId = GlobalConstants.INVALID_ID;
                            var doc = await this.repository.All<Data.Models.Data.DOC.DOC>(x => x.Name == docName).FirstOrDefaultAsync();
                            if (doc == null)
                            {
                                doc = new Data.Models.Data.DOC.DOC();


                                doc.UploadedFileName = "#";
                                doc.IdStatus = kvActiveStatus.IdKeyValue;

                                doc.Name = docName;
                                doc.Regulation = docRegulationName;
                                doc.RequirementsCandidates = reqCandidates;
                                doc.DescriptionProfession = descProfession;
                                doc.RequirementsMaterialBase = reqBase;
                                doc.RequirementsТrainers = reqTrainers;
                                doc.NewspaperNumber = newspaperNumber;
                                

                                var date = DateTime.MinValue;
                                if (DateTime.TryParse(docStartDate, out date))
                                {
                                    doc.StartDate = date;
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"Дата ({docStartDate}) е невалидна & C{row.Row}");
                                    isReadyForSave = false;
                                }

                                DateTime pubDate = DateTime.MinValue;
                                if (DateTime.TryParse(publishDate, out pubDate))
                                {
                                    doc.PublicationDate = pubDate;
                                }
                                else
                                {
                                    doc.PublicationDate = null;
                                }

                                var profession = await this.repository.All<Profession>(x => x.Code == professionCode && (x.IdStatus == kvActiveStatus.IdKeyValue || x.IdStatus == kvDraftStatus.IdKeyValue)).FirstOrDefaultAsync();
                                if (profession != null)
                                {
                                    doc.IdProfession = profession.IdProfession;
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"В базата не е намерена професия ({professionCode}) & D{row.Row}");
                                    isReadyForSave = false;
                                }

                                if (isReadyForSave)
                                {
                                    await this.repository.AddAsync<Data.Models.Data.DOC.DOC>(doc);
                                    result = await this.repository.SaveChangesAsync();
                                }

                                if (result > GlobalConstants.INVALID_ID_ZERO)
                                {
                                    isAddNewRecord = true;
                                    docId = doc.IdDOC;
                                }

                            }
                            else
                            {
                                docId = doc.IdDOC;

                                doc.NewspaperNumber = newspaperNumber;

                                DateTime pubDate = DateTime.MinValue;
                                if (DateTime.TryParse(publishDate, out pubDate))
                                {
                                    doc.PublicationDate = pubDate;
                                }
                                else
                                {
                                    doc.PublicationDate = null;
                                }

                                this.repository.Detach<Data.Models.Data.DOC.DOC>(doc);

                                doc.Specialities = null;
                                doc.docNkpds = null;

                                this.repository.Update(doc);
                                
                                result = await this.repository.SaveChangesAsync();
                            }

                            //Добавяме ид на ДОС към специалността
                            var specList = new List<SpecialityVM>();
                            if (!resultContext.ListErrorMessages.Any() || resultContext.ListErrorMessages.All(x => x.Contains("НКПД")))
                            {
                                var splitSpec = specialitiesCodes.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                                foreach (var item in splitSpec)
                                {
                                    var spec = await this.repository.All<Speciality>(x => x.Code == item.Trim() && (x.IdStatus == kvActiveStatus.IdKeyValue || x.IdStatus == kvDraftStatus.IdKeyValue)).FirstOrDefaultAsync();
                                    if (spec != null)
                                    {
                                        spec.IdDOC = docId;
                                        specList.Add(spec.To<SpecialityVM>());

                                        if (isReadyForSave)
                                        {
                                            await this.repository.SaveChangesAsync();
                                        }

                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"В базата не е намерена специалност ({item}) & E{row.Row}");
                                        continue;
                                    }
                                }
                            }

                            //Импорт на НКПД-та към ДОС
                            var idsNkpd = new List<int>();
                            var splitNkpds = nkpds.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                            foreach (var item in splitNkpds)
                            {
                                var nkpdCode = item.Replace("-", string.Empty);

                                var nkpdDB = await this.repository.All<Data.Models.Data.DOC.NKPD>(x => x.Code == nkpdCode.Trim()).FirstOrDefaultAsync();
                                if (nkpdDB != null)
                                {
                                    idsNkpd.Add(nkpdDB.IdNKPD);
                                }
                                else
                                {
                                    resultContext.AddErrorMessage($"В базата не е намеренo НКПД ({item}) & H{row.Row}");
                                    continue;
                                }

                            }

                            if (isReadyForSave)
                            {
                                //Записваме НКПД в таблица за връзка с ДОС
                                await SaveNKPDsToDoc(docId, idsNkpd);

                                //Записваме НКПД в таблица за връзка с Специалност
                                await SaveNKPDsToSpecialities(specList, idsNkpd);

                            }

                            //Създаване на ЕРУ
                            var eruModel = new ERUVM();
                            eruModel.IdDOC = docId;

                            //Взимаме всички ерута за съответния ДОС и проверяваме дали вече има съсдадено еру със същия код
                            //ако няма ще се създаде
                            var eruList = await GetAllERUsByDocIdAsync(eruModel);
                            var eruDb = eruList.FirstOrDefault(e => e.Code == eruCode);

                            if (eruDb != null)
                            {
                                var listSpecDB = await specialityService.GetSpecialitiesByERUIdAsync(eruDb.IdERU);

                                foreach (var item in specList)
                                {
                                    var specDB = listSpecDB.FirstOrDefault(s => s.Code == item.Code);
                                    if (specDB != null)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        var eruSpeciality = new ERUSpeciality()
                                        {
                                            IdERU = eruDb.IdERU,
                                            IdSpeciality = item.IdSpeciality,
                                        };

                                        await this.repository.AddAsync<ERUSpeciality>(eruSpeciality);
                                        await this.repository.SaveChangesAsync();
                                    }
                                }

                                continue;
                            }

                            eruModel.Code = eruCode;
                            eruModel.Name = eruName;
                            eruModel.RUText = ruText;
                            eruModel.Specialities = specList;

                            var kvProfession = kvTypeProfessionList.Where(x => x.Name == eruTypeProfession).FirstOrDefault();
                            if (kvProfession != null)
                            {
                                eruModel.IdProfessionalTraining = kvProfession.IdKeyValue;
                            }
                            else
                            {
                                resultContext.AddErrorMessage($"В базата не е намерена Вид професионална подготовка ({eruTypeProfession}) & K{row.Row}");
                                isReadyForSave = false;
                            }

                            var kvNKR = kvNKRList.Where(x => x.Name == nkrLevel).FirstOrDefault();
                            if (kvNKR != null)
                            {
                                eruModel.IdNKRLevel = kvNKR.IdKeyValue;
                            }
                            else
                            {
                                resultContext.AddErrorMessage($"В базата не е намерено Ниво по НКР ({nkrLevel}) & L{row.Row}");
                                isReadyForSave = false;
                            }

                            var kvEKR = kvEKRList.Where(x => x.Name == ekrLevel).FirstOrDefault();
                            if (kvEKR != null)
                            {
                                eruModel.IdEKRLevel = kvEKR.IdKeyValue;
                            }
                            else
                            {
                                resultContext.AddErrorMessage($"В базата не е намерено Ниво по ЕКР ({ekrLevel}) & M{row.Row}");
                                isReadyForSave = false;
                            }

                            if (isReadyForSave)
                            {
                                await CreateERUAsync(eruModel);
                                isAddNewRecord = true;
                            }

                        }
                    }
                }

                if (isAddNewRecord)
                {
                    resultContext.AddMessage("Импортът приключи успешно!");
                }

                return resultContext;
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        public async Task<MemoryStream> CreateExcelWithErrors(ResultContext<DocVM> resultContext)
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 50;
                sheet.Range[$"A1"].Text = "Вид на грешката";
                sheet.Range[$"B1"].Text = "Позиция във файла";

                var rowCounter = 2;
                foreach (var item in resultContext.ListErrorMessages)
                {
                    var splitMsg = item.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                    var msg = splitMsg[0].Trim();
                    var cell = splitMsg[1].Trim();

                    sheet.Range[$"A{rowCounter}"].Text = msg;
                    sheet.Range[$"B{rowCounter}"].Text = cell;

                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }

        #region ERU
        public async Task<IEnumerable<ERUVM>> GetAllERUsByDocIdAsync(ERUVM filterERUVMVM, bool includeSpecialities = true)
        {
            var list = this.repository.All<ERU>(FilterEruValue(filterERUVMVM));
            var keyValues = dataSourceService.GetAllKeyValueList();
            var docs = await this.GetAllDocAsync();

            IEnumerable<ERUVM> result;
            if (includeSpecialities)
            {
                result = await list.To<ERUVM>(x => x.ERUSpecialities).ToListAsync();
            }
            else
            {
                result = await list.To<ERUVM>().ToListAsync();
            }

            foreach (var item in result)
            {
                item.ProfessionalTrainingName = keyValues.FirstOrDefault(k => item.IdProfessionalTraining == k.IdKeyValue)?.Name ?? string.Empty;
                item.NKRLevelName = keyValues.FirstOrDefault(k => item.IdNKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                item.EKRLevelName = keyValues.FirstOrDefault(k => item.IdEKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                item.NameOfDOC = docs.FirstOrDefault(d => d.IdDOC == item.IdDOC).Name;
            }

            return result.OrderBy(x => x.ERUIntCodeSplit);
        }

        public async Task<IEnumerable<ERUVM>> GetAllERUsByIdSpecialityAsync(int idSpeciality)
        {
            var list = this.repository.AllReadonly<ERUSpeciality>(x => x.IdSpeciality == idSpeciality);
            var keyValues = dataSourceService.GetAllKeyValueList();
            var docs = await this.GetAllDocAsync();

            var listAsVM = await list.To<ERUSpecialityVM>(x => x.ERU).ToListAsync();
            IEnumerable<ERUVM> result = listAsVM.Select(x => x.ERU).ToList();
            foreach (var item in result)
            {
                item.ProfessionalTrainingName = keyValues.FirstOrDefault(k => item.IdProfessionalTraining == k.IdKeyValue)?.Name ?? string.Empty;
                item.NKRLevelName = keyValues.FirstOrDefault(k => item.IdNKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                item.EKRLevelName = keyValues.FirstOrDefault(k => item.IdEKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                item.NameOfDOC = docs.FirstOrDefault(d => d.IdDOC == item.IdDOC).Name;
            }

            return result.OrderBy(x => x.ERUIntCodeSplit);
        }

        public async Task<IEnumerable<ERUVM>> GetAllERUsByActiveDOCsAsync()
        {
            var list = this.repository.All<ERU>();
            var keyValues = dataSourceService.GetAllKeyValueList();
            var kvStatusSource = await dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Active");

            IEnumerable<ERUVM> result = await list.To<ERUVM>(x => x.DOC).ToListAsync();
            var dataVM = result.Where(x => x.DOC.IdStatus == kvStatusSource.IdKeyValue).ToList();
            foreach (var item in result)
            {
                item.ProfessionalTrainingName = keyValues.FirstOrDefault(k => item.IdProfessionalTraining == k.IdKeyValue)?.Name ?? string.Empty;
                item.NKRLevelName = keyValues.FirstOrDefault(k => item.IdNKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                item.EKRLevelName = keyValues.FirstOrDefault(k => item.IdEKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
            }

            return dataVM;
        }

        protected Expression<Func<ERU, bool>> FilterEruValue(ERUVM model)
        {
            var predicate = PredicateBuilder.True<ERU>();

            predicate = predicate.And(p => p.IdDOC == model.IdDOC);

            return predicate;
        }

        public async Task<string> UpdateERUAsync(ERUVM model)
        {
            try
            {
                var updatedEnity = await this.GetByIdAsync<ERU>(model.IdERU);

                updatedEnity = model.To<ERU>();

                var listSpec = await specialityService.GetSpecialitiesByERUIdAsync(model.IdERU);
                var ids = listSpec.Select(s => s.IdSpeciality).ToList();

                foreach (var item in model.Specialities)
                {
                    if (!ids.Contains(item.IdSpeciality))
                    {
                        var eruSpeciality = new ERUSpeciality()
                        {
                            IdERU = model.IdERU,
                            IdSpeciality = item.IdSpeciality,
                        };

                        await this.repository.AddAsync<ERUSpeciality>(eruSpeciality);
                        //await this.repository.SaveChangesAsync();
                        //this.repository.Detach<ERUSpeciality>(eruSpeciality);
                    }
                }

                this.repository.Update<ERU>(updatedEnity);
                var result = await this.repository.SaveChangesAsync();
                this.repository.Detach<ERU>(updatedEnity);
                string msg;
                if (result > 0)
                {
                    model.IdModifyUser = updatedEnity.IdModifyUser;
                    model.ModifyDate = updatedEnity.ModifyDate;
                    msg = "Записът e успешeн!";
                    _logger.LogInformation(msg);
                }
                else
                {
                    msg = "Грешка при запис в базата!";
                    _logger.LogError(msg);
                }

                return msg;

            }
            catch (Exception ex)
            {

                return ex.Message;
            }

        }

        public async Task<string> CreateERUAsync(ERUVM model)
        {

            string msg;

            try
            {
                var data = model.To<ERU>();
                //data.Specialities = new List<Speciality>();

                await this.repository.AddAsync<ERU>(data);
                var result = await this.repository.SaveChangesAsync();

                if (result > 0)
                {
                    model.IdERU = data.IdERU;
                    model.IdCreateUser = data.IdCreateUser;
                    model.CreationDate = data.CreationDate;
                    model.IdModifyUser = data.IdModifyUser;
                    model.ModifyDate = data.ModifyDate;

                    foreach (var item in model.Specialities)
                    {
                        var eruSpeciality = new ERUSpeciality()
                        {
                            IdERU = model.IdERU,
                            IdSpeciality = item.IdSpeciality,
                        };

                        await this.repository.AddAsync<ERUSpeciality>(eruSpeciality);
                    }

                    if (model.Specialities.Count > 0)
                    {
                        result = await this.repository.SaveChangesAsync();
                    }

                    if (result > 0)
                    {
                        msg = "Записът e успешeн!";
                    }
                    else
                    {
                        msg = "Грешка при запис в базата!";
                    }
                }
                else
                {
                    msg = "Грешка при запис в базата.";
                }

                return msg;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.StackTrace);
                return ex.Message;
            }

        }

        public async Task<IEnumerable<ERUVM>> GetERUsByIdsAsync(List<int> ids)
        {
            IQueryable<ERU> erus = this.repository.AllReadonly<ERU>(FilterByIds(ids));
            var keyValues = dataSourceService.GetAllKeyValueList();

            IEnumerable<ERUVM> result = await erus.To<ERUVM>().ToListAsync();

            foreach (var item in result)
            {
                item.ProfessionalTrainingName = keyValues.FirstOrDefault(k => item.IdProfessionalTraining == k.IdKeyValue)?.Name ?? string.Empty;
                item.NKRLevelName = keyValues.FirstOrDefault(k => item.IdNKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
                item.EKRLevelName = keyValues.FirstOrDefault(k => item.IdEKRLevel == k.IdKeyValue)?.Name ?? string.Empty;
            }

            return result;
        }

        public async Task<ERUVM> GetERUByIdAsync(ERUVM eru)
        {
            IQueryable<ERU> eruFromDb = this.repository.AllReadonly<ERU>(x => x.IdERU == eru.IdERU);

            return await eruFromDb.To<ERUVM>().FirstOrDefaultAsync();
        }

        protected Expression<Func<ERU, bool>> FilterByIds(List<int> ids)
        {
            var predicate = PredicateBuilder.True<ERU>();

            predicate = predicate.And(n => ids.Contains(n.IdERU));

            return predicate;
        }

        #endregion
    }
}
