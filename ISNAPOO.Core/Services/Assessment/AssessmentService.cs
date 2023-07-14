using Data.Models.Common;
using Data.Models.Data.Archive;
using Data.Models.Data.Assessment;
using Data.Models.Data.Candidate;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math.EC.Multiplier;
using Syncfusion.Office;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ISNAPOO.Core.Services.Assessment
{
    public class AssessmentService : BaseService, IAssessmentService
    {
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        private readonly IMailService mailService;
        private readonly ILogger<AssessmentService> _logger;

        public AssessmentService(IRepository repository,
            IDataSourceService dataSourceService,
            IMailService mailService,
            ILogger<AssessmentService> logger,
            AuthenticationStateProvider authStateProvider)
            : base(repository, authStateProvider)
        {
            this.repository = repository;
            this.dataSourceService = dataSourceService;
            this.mailService = mailService;
            this._logger = logger;
        }

        #region Report
        public async Task<MemoryStream> CreateExcelSummarizedReportWithSurveyResultsAsync(ResultContext<List<SurveyResultVM>> resultContext)
        {
            var survey = resultContext.ResultContextObject.FirstOrDefault()!.Survey;
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 40;
                sheet.Range[$"A1"].Text = "Въпрос";
                sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                sheet.Range["B1"].ColumnWidth = 40;
                sheet.Range[$"B1"].Text = "Отговор";
                sheet.Range[$"B1"].CellStyle.Font.Bold = true;
                sheet.Range["C1"].ColumnWidth = 40;
                sheet.Range[$"C1"].Text = "Брой отговорили";
                sheet.Range[$"C1"].CellStyle.Font.Bold = true;

                var kvOpenQuestionValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("QuestionType", "Open");
                var rowCounter = 2;
                foreach (var question in survey.Questions)
                {
                    foreach (var answer in question.Answers)
                    {
                        sheet.Range[$"A{rowCounter}"].Text = question.Text;
                        sheet.Range[$"A{rowCounter}"].WrapText = true;
                        sheet.Range[$"B{rowCounter}"].Text = question.IdQuestType == kvOpenQuestionValue.IdKeyValue ? "-" : answer.Text;
                        sheet.Range[$"B{rowCounter}"].WrapText = true;
                        sheet.Range[$"C{rowCounter}"].Number = answer.UserAnswers.Count;
                        sheet.Range[$"C{rowCounter}"].WrapText = true;

                        rowCounter++;
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }
        public async Task<MemoryStream> CreateExcelReportWithSurveyResultsAsync(List<SelfAssessmentReportVM> selfAssessmentReportVM, List<SurveyResultVM> listSurveyResultVM, string cpoCipoHeaderTxt, List<List<SelfAssessmentSummaryProfessionalTrainingVM>> selfAssSummTrainingVM, List<List<UserAnswerModel>> userAnswersListModels)
        {
            var survey = listSurveyResultVM.FirstOrDefault()!.Survey;
           
            int newHeaderCounter = 0;
            var commonInfo = selfAssSummTrainingVM.FirstOrDefault();
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 40;
                sheet.Range[$"A1"].Text = "Лицензия";
                sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                sheet.Range["B1"].ColumnWidth = 40;
                sheet.Range[$"B1"].Text = cpoCipoHeaderTxt;
                sheet.Range[$"B1"].CellStyle.Font.Bold = true;
                sheet.Range["C1"].ColumnWidth = 40;
                sheet.Range[$"C1"].Text = "Година";
                sheet.Range[$"C1"].CellStyle.Font.Bold = true;
                sheet.Range["D1"].ColumnWidth = 40;
                sheet.Range[$"D1"].Text = "Дата на подаване";
                sheet.Range[$"D1"].CellStyle.Font.Bold = true;
                sheet.Range["E1"].ColumnWidth = 40;
                sheet.Range[$"E1"].Text = "Статус";
                sheet.Range[$"E1"].CellStyle.Font.Bold = true;
                sheet.Range["F1"].ColumnWidth = 40;
                sheet.Range[$"F1"].Text = "Коментар";
                sheet.Range[$"F1"].CellStyle.Font.Bold = true;

                var headerCounter = 7;
                foreach (var info in commonInfo)
                {
                    var headerSymbols = this.GetExcelHeaderSymbolByNumber(headerCounter);
                    sheet.Range[$"{headerSymbols}1"].Text = info.ProfessionalTrainingIndicatorName;
                    sheet.Range[$"{headerSymbols}1"].ColumnWidth = 20;
                    sheet.Range[$"{headerSymbols}1"].CellStyle.Font.Bold = true;
                    sheet.Range[$"{headerSymbols}1"].WrapText = true;

                    headerCounter++;
                }

                foreach (var areaSelfAssessment in survey.Questions.GroupBy(x => x.AreaSelfAssessment).OrderBy(x => x.Key.Order))
                {
                    foreach (var info in survey.Questions.Where(q => q.IdAreaSelfAssessment == areaSelfAssessment.Key.IdKeyValue))
                    {

                        var headerSymbols = this.GetExcelHeaderSymbolByNumber(headerCounter);
                        sheet.Range[$"{headerSymbols}1"].Text = info.Text;
                        sheet.Range[$"{headerSymbols}1"].ColumnWidth = 20;
                        sheet.Range[$"{headerSymbols}1"].CellStyle.Font.Bold = true;
                        sheet.Range[$"{headerSymbols}1"].WrapText = true;

                        headerCounter++;
                    }
                }
                
                var rowCounter = 2;
                
                foreach (var selfAssessmentReport in selfAssessmentReportVM)
                {                
                            sheet.Range[$"A{rowCounter}"].Text = selfAssessmentReport.CandidateProvider.LicenceNumber;
                            sheet.Range[$"A{rowCounter}"].WrapText = true;
                            sheet.Range[$"B{rowCounter}"].Text = selfAssessmentReport.CandidateProvider.ProviderOwner;
                            sheet.Range[$"B{rowCounter}"].WrapText = true;
                            sheet.Range[$"C{rowCounter}"].Number = selfAssessmentReport.Year;
                            sheet.Range[$"C{rowCounter}"].WrapText = true;
                            sheet.Range[$"D{rowCounter}"].Text = selfAssessmentReport.FilingDateAsStr;
                            sheet.Range[$"D{rowCounter}"].WrapText = true;
                            sheet.Range[$"E{rowCounter}"].Text = selfAssessmentReport.Status;
                            sheet.Range[$"E{rowCounter}"].WrapText = true;
                            sheet.Range[$"F{rowCounter}"].Text = selfAssessmentReport.CommentSelfAssessmentReportStatus;
                            sheet.Range[$"F{rowCounter}"].WrapText = true;                           
                                                      
                    rowCounter++;
                }

                rowCounter = 2;          
               
                foreach (var selfass in selfAssSummTrainingVM)
                {
                    headerCounter = 7;
                    var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                    foreach (var item in selfass)
                    {
                        headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                        sheet.Range[$"{headerSymbol}{rowCounter}"].Number = item.ProfessionalTrainingIndicatorCount;
                        sheet.Range[$"{headerSymbol}{rowCounter}"].WrapText = true;
                        headerCounter = headerCounter + 1;
                    }
                    rowCounter++;
                }

                rowCounter = 2;
                
                foreach (var userAnswersList in userAnswersListModels)
                {
                    newHeaderCounter = headerCounter;
                    var headerSymbol = this.GetExcelHeaderSymbolByNumber(newHeaderCounter);
                    foreach (var answer in userAnswersList)
                    {
                        headerSymbol = this.GetExcelHeaderSymbolByNumber(newHeaderCounter);
                        if (answer.HasPoints == true)
                        {
                            sheet.Range[$"{headerSymbol}{rowCounter}"].Number = (double)answer.Points;
                        }
                        else
                        {
                            sheet.Range[$"{headerSymbol}{rowCounter}"].Text = answer.OpenAnswerText;
                        }
                        sheet.Range[$"{headerSymbol}{rowCounter}"].WrapText = true;
                        newHeaderCounter++;
                    }                      
                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }
        public async Task<MemoryStream> CreateExcelDetailedReportWithSurveyResultsAsync(ResultContext<List<SurveyResultVM>> resultContext)
        {
            var survey = resultContext.ResultContextObject.FirstOrDefault()!.Survey;
            var surveyResults = resultContext.ResultContextObject;
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                var kvOpenQuestionValue = await this.dataSourceService.GetKeyValueByIntCodeAsync("QuestionType", "Open");
                if (surveyResults.Any(x => x.ConsultingClient != null))
                {
                    sheet.Range["A1"].ColumnWidth = 20;
                    sheet.Range[$"A1"].Text = "ЦИПО";
                    sheet.Range[$"A1"].CellStyle.Font.Bold = true;

                    var headerCounter = 2;
                    foreach (var question in survey.Questions)
                    {
                        var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                        sheet.Range[$"{headerSymbol}1"].Text = question.Text;
                        sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                        sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                        sheet.Range[$"{headerSymbol}1"].WrapText = true;

                        headerCounter++;
                    }

                    var rowCounter = 2;
                    foreach (var surveyResult in surveyResults)
                    {
                        string cipo = String.Empty;
                        if (surveyResult.CandidateProvider is not null)
                        {
                            cipo = !string.IsNullOrEmpty(surveyResult.CandidateProvider.ProviderName)
                                ? $"ЦИПО {surveyResult.CandidateProvider.ProviderName} към {surveyResult.CandidateProvider.ProviderOwner}"
                                : $"ЦИПО към {surveyResult.CandidateProvider.ProviderOwner}";

                            sheet.Range[$"A{rowCounter}"].Text = cipo;
                            sheet.Range[$"A{rowCounter}"].WrapText = true;

                            headerCounter = 2;
                            foreach (var question in survey.Questions)
                            {
                                var userAnswerOpenList = await this.GetUserAnswerOpenListByIdQuestionAndIdSurveyResultAsync(question.IdQuestion, surveyResult.IdSurveyResult);
                                var userAnswerOpenIds = userAnswerOpenList.Select(x => x.IdUserAnswerOpen).ToList();
                                var userAnswersByListIdUserAnswerOpen = await this.GetUserAnswersByListIdsUserAnswerOpenAsync(userAnswerOpenIds);

                                var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                                if (question.IdQuestType != kvOpenQuestionValue.IdKeyValue)
                                {
                                    var answersText = string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.Answer != null ? x.Answer.Text : string.Empty).ToList());
                                    if (userAnswersByListIdUserAnswerOpen.Any(x => x.UserAnswerOpen != null && !string.IsNullOrEmpty(x.UserAnswerOpen.Text)))
                                    {
                                        answersText += "; " + string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.UserAnswerOpen.Text).ToList());
                                    }

                                    sheet.Range[$"{headerSymbol}{rowCounter}"].Text = answersText;
                                }
                                else
                                {
                                    sheet.Range[$"{headerSymbol}{rowCounter}"].Text = string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.UserAnswerOpen != null ? x.UserAnswerOpen.Text : string.Empty).ToList());
                                }

                                sheet.Range[$"{headerSymbol}{rowCounter}"].WrapText = true;

                                headerCounter++;
                            }

                            rowCounter++;
                        }
                    }
                }
                else if (surveyResults.Any(x => x.ClientCourse != null))
                {
                    sheet.Range["A1"].ColumnWidth = 20;
                    sheet.Range[$"A1"].Text = "ЦПО";
                    sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                    sheet.Range["B1"].ColumnWidth = 20;
                    sheet.Range[$"B1"].Text = "Курс";
                    sheet.Range[$"B1"].CellStyle.Font.Bold = true;
                    sheet.Range["C1"].ColumnWidth = 30;
                    sheet.Range[$"C1"].Text = "Професионално направление";
                    sheet.Range[$"C1"].CellStyle.Font.Bold = true;
                    sheet.Range["D1"].ColumnWidth = 20;
                    sheet.Range[$"D1"].Text = "Професия";
                    sheet.Range[$"D1"].CellStyle.Font.Bold = true;
                    sheet.Range["E1"].ColumnWidth = 20;
                    sheet.Range[$"E1"].Text = "Специалност";
                    sheet.Range[$"E1"].CellStyle.Font.Bold = true;

                    var headerCounter = 6;
                    foreach (var question in survey.Questions)
                    {
                        var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                        sheet.Range[$"{headerSymbol}1"].Text = question.Text;
                        sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                        sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                        sheet.Range[$"{headerSymbol}1"].WrapText = true;

                        headerCounter++;
                    }

                    var rowCounter = 2;
                    foreach (var surveyResult in surveyResults)
                    {
                        string cpo = String.Empty;
                        string course = String.Empty;
                        string professionalDirection = String.Empty;
                        string profession = String.Empty;
                        string speciality = String.Empty;
                        if (surveyResult.CandidateProvider is not null)
                        {
                            cpo = !string.IsNullOrEmpty(surveyResult.CandidateProvider.ProviderName)
                                ? $"ЦПО {surveyResult.CandidateProvider.ProviderName} към {surveyResult.CandidateProvider.ProviderOwner}"
                                : $"ЦПО към {surveyResult.CandidateProvider.ProviderOwner}";

                            sheet.Range[$"A{rowCounter}"].Text = cpo;
                            sheet.Range[$"A{rowCounter}"].WrapText = true;

                            if (surveyResult.ClientCourse is not null)
                            {
                                if (surveyResult.ClientCourse.Course is not null)
                                {
                                    course = surveyResult.ClientCourse.Course.CourseName;
                                    if (surveyResult.ClientCourse.Course.Program is not null)
                                    {
                                        if (surveyResult.ClientCourse.Course.Program.IdSpeciality != 0)
                                        {
                                            var specialityFromSPPOO = this.dataSourceService.GetAllSpecialitiesList().FirstOrDefault(x => x.IdSpeciality == surveyResult.ClientCourse.Course.Program.IdSpeciality);
                                            if (specialityFromSPPOO is not null)
                                            {
                                                speciality = specialityFromSPPOO.CodeAndName;

                                                var professionFromSPPOO = this.dataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == specialityFromSPPOO.IdProfession);
                                                if (professionFromSPPOO is not null)
                                                {
                                                    profession = professionFromSPPOO.CodeAndName;

                                                    var professionalDirectionFromSPPOO = this.dataSourceService.GetAllProfessionalDirectionsList().FirstOrDefault(x => x.IdProfessionalDirection == professionFromSPPOO.IdProfessionalDirection);
                                                    if (professionalDirectionFromSPPOO is not null)
                                                    {
                                                        professionalDirection = professionalDirectionFromSPPOO.DisplayNameAndCode;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            sheet.Range[$"B{rowCounter}"].Text = course;
                            sheet.Range[$"B{rowCounter}"].WrapText = true;
                            sheet.Range[$"C{rowCounter}"].Text = professionalDirection;
                            sheet.Range[$"C{rowCounter}"].WrapText = true;
                            sheet.Range[$"D{rowCounter}"].Text = profession;
                            sheet.Range[$"D{rowCounter}"].WrapText = true;
                            sheet.Range[$"E{rowCounter}"].Text = speciality;
                            sheet.Range[$"E{rowCounter}"].WrapText = true;

                            headerCounter = 6;
                            foreach (var question in survey.Questions)
                            {
                                var userAnswerOpenList = await this.GetUserAnswerOpenListByIdQuestionAndIdSurveyResultAsync(question.IdQuestion, surveyResult.IdSurveyResult);
                                var userAnswerOpenIds = userAnswerOpenList.Select(x => x.IdUserAnswerOpen).ToList();
                                var userAnswersByListIdUserAnswerOpen = await this.GetUserAnswersByListIdsUserAnswerOpenAsync(userAnswerOpenIds);

                                var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                                if (question.IdQuestType != kvOpenQuestionValue.IdKeyValue)
                                {
                                    var answersText = string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.Answer != null ? x.Answer.Text : string.Empty).ToList());
                                    if (userAnswersByListIdUserAnswerOpen.Any(x => x.UserAnswerOpen != null && !string.IsNullOrEmpty(x.UserAnswerOpen.Text)))
                                    {
                                        answersText += "; " + string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.UserAnswerOpen.Text).ToList());
                                    }

                                    sheet.Range[$"{headerSymbol}{rowCounter}"].Text = answersText;
                                }
                                else
                                {
                                    sheet.Range[$"{headerSymbol}{rowCounter}"].Text = string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.UserAnswerOpen != null ? x.UserAnswerOpen.Text : string.Empty).ToList());
                                }

                                sheet.Range[$"{headerSymbol}{rowCounter}"].WrapText = true;

                                headerCounter++;
                            }

                            rowCounter++;
                        }
                    }
                }
                else if (surveyResults.Any(x => x.ValidationClient != null))
                {
                    sheet.Range["A1"].ColumnWidth = 20;
                    sheet.Range[$"A1"].Text = "ЦПО";
                    sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                    sheet.Range["B1"].ColumnWidth = 20;
                    sheet.Range[$"B1"].Text = "Професионално направление";
                    sheet.Range[$"B1"].CellStyle.Font.Bold = true;
                    sheet.Range["C1"].ColumnWidth = 20;
                    sheet.Range[$"C1"].Text = "Професия";
                    sheet.Range[$"C1"].CellStyle.Font.Bold = true;
                    sheet.Range["D1"].ColumnWidth = 20;
                    sheet.Range[$"D1"].Text = "Специалност";
                    sheet.Range[$"D1"].CellStyle.Font.Bold = true;

                    var headerCounter = 5;
                    foreach (var question in survey.Questions)
                    {
                        var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                        sheet.Range[$"{headerSymbol}1"].Text = question.Text;
                        sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                        sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                        sheet.Range[$"{headerSymbol}1"].WrapText = true;

                        headerCounter++;
                    }

                    var rowCounter = 2;
                    foreach (var surveyResult in surveyResults)
                    {
                        string cpo = String.Empty;
                        string professionalDirection = String.Empty;
                        string profession = String.Empty;
                        string speciality = String.Empty;
                        if (surveyResult.CandidateProvider is not null)
                        {
                            cpo = !string.IsNullOrEmpty(surveyResult.CandidateProvider.ProviderName)
                                ? $"ЦПО {surveyResult.CandidateProvider.ProviderName} към {surveyResult.CandidateProvider.ProviderOwner}"
                                : $"ЦПО към {surveyResult.CandidateProvider.ProviderOwner}";

                            sheet.Range[$"A{rowCounter}"].Text = cpo;
                            sheet.Range[$"A{rowCounter}"].WrapText = true;

                            if (surveyResult.ValidationClient is not null)
                            {
                                if (surveyResult.ValidationClient.IdSpeciality != 0)
                                {
                                    var specialityFromSPPOO = this.dataSourceService.GetAllSpecialitiesList().FirstOrDefault(x => x.IdSpeciality == surveyResult.ClientCourse.Course.Program.IdSpeciality);
                                    if (specialityFromSPPOO is not null)
                                    {
                                        speciality = specialityFromSPPOO.CodeAndName;

                                        var professionFromSPPOO = this.dataSourceService.GetAllProfessionsList().FirstOrDefault(x => x.IdProfession == specialityFromSPPOO.IdProfession);
                                        if (professionFromSPPOO is not null)
                                        {
                                            profession = professionFromSPPOO.CodeAndName;

                                            var professionalDirectionFromSPPOO = this.dataSourceService.GetAllProfessionalDirectionsList().FirstOrDefault(x => x.IdProfessionalDirection == professionFromSPPOO.IdProfessionalDirection);
                                            if (professionalDirectionFromSPPOO is not null)
                                            {
                                                professionalDirection = professionalDirectionFromSPPOO.DisplayNameAndCode;
                                            }
                                        }
                                    }
                                }
                            }

                            sheet.Range[$"B{rowCounter}"].Text = professionalDirection;
                            sheet.Range[$"B{rowCounter}"].WrapText = true;
                            sheet.Range[$"C{rowCounter}"].Text = profession;
                            sheet.Range[$"C{rowCounter}"].WrapText = true;
                            sheet.Range[$"D{rowCounter}"].Text = speciality;
                            sheet.Range[$"D{rowCounter}"].WrapText = true;

                            headerCounter = 5;
                            foreach (var question in survey.Questions)
                            {
                                var userAnswerOpenList = await this.GetUserAnswerOpenListByIdQuestionAndIdSurveyResultAsync(question.IdQuestion, surveyResult.IdSurveyResult);
                                var userAnswerOpenIds = userAnswerOpenList.Select(x => x.IdUserAnswerOpen).ToList();
                                var userAnswersByListIdUserAnswerOpen = await this.GetUserAnswersByListIdsUserAnswerOpenAsync(userAnswerOpenIds);

                                var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                                if (question.IdQuestType != kvOpenQuestionValue.IdKeyValue)
                                {
                                    var answersText = string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.Answer != null ? x.Answer.Text : string.Empty).ToList());
                                    if (userAnswersByListIdUserAnswerOpen.Any(x => x.UserAnswerOpen != null && !string.IsNullOrEmpty(x.UserAnswerOpen.Text)))
                                    {
                                        answersText += "; " + string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.UserAnswerOpen.Text).ToList());
                                    }

                                    sheet.Range[$"{headerSymbol}{rowCounter}"].Text = answersText;
                                }
                                else
                                {
                                    sheet.Range[$"{headerSymbol}{rowCounter}"].Text = string.Join(", ", userAnswersByListIdUserAnswerOpen.Select(x => x.UserAnswerOpen != null ? x.UserAnswerOpen.Text : string.Empty).ToList());
                                }

                                sheet.Range[$"{headerSymbol}{rowCounter}"].WrapText = true;

                                headerCounter++;
                            }

                            rowCounter++;
                        }
                    }
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }
        private string GetExcelHeaderSymbolByNumber(int number)
        {
            string headerSymbol = String.Empty;
            if (number == 4)
            {
                headerSymbol = "D";
            }
            else if (number == 5)
            {
                headerSymbol = "E";
            }
            else if (number == 6)
            {
                headerSymbol = "F";
            }
            else if (number == 7)
            {
                headerSymbol = "G";
            }
            else if (number == 8)
            {
                headerSymbol = "H";
            }
            else if (number == 9)
            {
                headerSymbol = "I";
            }
            else if (number == 10)
            {
                headerSymbol = "J";
            }
            else if (number == 11)
            {
                headerSymbol = "K";
            }
            else if (number == 12)
            {
                headerSymbol = "L";
            }
            else if (number == 13)
            {
                headerSymbol = "M";
            }
            else if (number == 14)
            {
                headerSymbol = "N";
            }
            else if (number == 15)
            {
                headerSymbol = "O";
            }
            else if (number == 16)
            {
                headerSymbol = "P";
            }
            else if (number == 17)
            {
                headerSymbol = "Q";
            }
            else if (number == 18)
            {
                headerSymbol = "R";
            }
            else if (number == 19)
            {
                headerSymbol = "S";
            }
            else if (number == 20)
            {
                headerSymbol = "T";
            }
            else if (number == 21)
            {
                headerSymbol = "U";
            }
            else if (number == 22)
            {
                headerSymbol = "V";
            }
            else if (number == 23)
            {
                headerSymbol = "W";
            }
            else if (number == 24)
            {
                headerSymbol = "X";
            }
            else if (number == 25)
            {
                headerSymbol = "Y";
            }
            else if (number == 26)
            {
                headerSymbol = "Z";
            }
            else if (number == 27)
            {
                headerSymbol = "AA";
            }
            else if (number == 28)
            {
                headerSymbol = "AB";
            }
            else if (number == 29)
            {
                headerSymbol = "AC";
            }
            else if (number == 30)
            {
                headerSymbol = "AD";
            }
            else if (number == 31)
            {
                headerSymbol = "AE";
            }
            else if (number == 32)
            {
                headerSymbol = "AF";
            }
            else if (number == 33)
            {
                headerSymbol = "AG";
            }
            else if (number == 34)
            {
                headerSymbol = "AH";
            }
            else if (number == 35)
            {
                headerSymbol = "AI";
            }
            else if (number == 36)
            {
                headerSymbol = "AJ";
            }
            else if (number == 37)
            {
                headerSymbol = "AK";
            }
            else if (number == 38)
            {
                headerSymbol = "AL";
            }
            else if (number == 39)
            {
                headerSymbol = "AM";
            }
            else if (number == 40)
            {
                headerSymbol = "AN";
            }
            else if (number == 41)
            {
                headerSymbol = "AO";
            }
            else if (number == 42)
            {
                headerSymbol = "AP";
            }
            else if (number == 43)
            {
                headerSymbol = "AQ";
            }
            else if (number == 44)
            {
                headerSymbol = "AR";
            }
            else if (number == 45)
            {
                headerSymbol = "AS";
            }
            else if (number == 46)
            {
                headerSymbol = "AT";
            }
            else if (number == 47)
            {
                headerSymbol = "AU";
            }
            else if (number == 48)
            {
                headerSymbol = "AV";
            }
            else if (number == 49)
            {
                headerSymbol = "AW";
            }
            else if (number == 50)
            {
                headerSymbol = "AX";
            }
            else if (number == 51)
            {
                headerSymbol = "AY";
            }
            else if (number == 52)
            {
                headerSymbol = "AZ";
            }
            else if (number == 53)
            {
                headerSymbol = "BA";
            }
            else if (number == 54)
            {
                headerSymbol = "BB";
            }
            else if (number == 55)
            {
                headerSymbol = "BC";
            }
            else if (number == 56)
            {
                headerSymbol = "BD";
            }
            else if (number == 57)
            {
                headerSymbol = "BE";
            }
            else if (number == 58)
            {
                headerSymbol = "BF";
            }
            else if (number == 59)
            {
                headerSymbol = "BG";
            }
            else if (number == 60)
            {
                headerSymbol = "BH";
            }
            else if (number == 61)
            {
                headerSymbol = "BI";
            }
            else if (number == 62)
            {
                headerSymbol = "BJ";
            }
            else if (number == 63)
            {
                headerSymbol = "BK";
            }
            else if (number == 64)
            {
                headerSymbol = "BL";
            }
            else if (number == 65)
            {
                headerSymbol = "BM";
            }
            else if (number == 66)
            {
                headerSymbol = "BN";
            }
            else if (number == 67)
            {
                headerSymbol = "BO";
            }
            else if (number == 68)
            {
                headerSymbol = "BP";
            }
            else if (number == 69)
            {
                headerSymbol = "BQ";
            }
            else if (number == 70)
            {
                headerSymbol = "BR";
            }
            else if (number == 71)
            {
                headerSymbol = "BS";
            }
            else if (number == 72)
            {
                headerSymbol = "BT";
            }
            else if (number == 73)
            {
                headerSymbol = "BU";
            }
            else if (number == 74)
            {
                headerSymbol = "BV";
            }
            else if (number == 75)
            {
                headerSymbol = "BW";
            }
            else if (number == 76)
            {
                headerSymbol = "BX";
            }
            else if (number == 77)
            {
                headerSymbol = "BY";
            }
            else if (number == 78)
            {
                headerSymbol = "BZ";
            }
            return headerSymbol;
        }

        #endregion

        #region Survey
        public async Task<IEnumerable<SurveyVM>> GetAllSurveysByIdSurveyTypeAsync(int idSurveyType)
        {
            var surveys = this.repository.AllReadonly<Survey>(x => x.IdSurveyТype == idSurveyType);
            var surveysAsVM = await surveys.To<SurveyVM>(x => x.SurveyResults).ToListAsync();

            var kvSurveyTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SurveyType");
            var kvFrameworkProgramSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.DefaultValue3 != null).ToList();
            foreach (var survey in surveysAsVM)
            {
                survey.FiledOutCount = await this.FiledOutSurveyCountAsync(survey.IdSurvey);
                survey.SurveysSentCount = survey.SurveyResults.Count;

                var courseType = kvFrameworkProgramSource.FirstOrDefault(x => x.IdKeyValue == survey.IdTrainingCourseType);
                if (courseType is not null)
                {
                    survey.TrainingCourseTypeValue = courseType.Name;
                }
            }

            return surveysAsVM.OrderByDescending(x => x.EndDate).ToList();
        }

        public async Task<IEnumerable<SurveyVM>> GetAllSurveysByIdSurveyTypeAndIdCandidateProviderAsync(int idSurveyType, int idCandidateProvider)
        {
            var surveys = new List<SurveyVM>();
            var surveyResults = await this.repository.AllReadonly<SurveyResult>(x => x.IdCandidate_Provider == idCandidateProvider && (x.IdClientCourse != null || x.IdValidationClient != null))
                .Include(x => x.Survey).ToListAsync();

            var kvSurveyTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SurveyType");
            var kvFrameworkProgramSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.DefaultValue3 != null).ToList();
            foreach (var surveyResult in surveyResults)
            {
                if (surveyResult.Survey is not null && surveyResult.Survey.IdSurveyТype == idSurveyType)
                {
                    if (!surveys.Any(x => x.IdSurvey == surveyResult.Survey.IdSurvey))
                    {
                        var survey = surveyResult.Survey.To<SurveyVM>();
                        survey.FiledOutCount = await this.FiledOutSurveyCountAsync(survey.IdSurvey);
                        survey.SurveysSentCount = surveyResults.Where(x => x.Survey != null && x.Survey.IdSurveyТype == idSurveyType && x.Survey.IdSurvey == survey.IdSurvey).Count();

                        var courseType = kvFrameworkProgramSource.FirstOrDefault(x => x.IdKeyValue == survey.IdTrainingCourseType);
                        if (courseType is not null)
                        {
                            survey.TrainingCourseTypeValue = courseType.Name;
                        }

                        surveys.Add(survey);
                    }
                }
            }

            return surveys.OrderByDescending(x => x.EndDate).ToList();
        }

        public async Task<SurveyVM> GetSurveyByIdAsync(int idSurvey)
        {
            var surveyFromDb = this.repository.AllReadonly<Survey>(x => x.IdSurvey == idSurvey);

            return await surveyFromDb.To<SurveyVM>(x => x.Questions.OrderBy(n => n.Order).ThenBy(n => n.Text), x => x.Questions.Select(y => y.Answers), x => x.SurveyResults).FirstOrDefaultAsync();
        }

        public async Task<SurveyVM> GetSurveySelfAssessmentByYear(int year, string surveyTarget)
        {

            if (surveyTarget == "LicensingCPO")
            {
                surveyTarget = "ForCPO";
            }
            if (surveyTarget == "LicensingCIPO")
            {
                surveyTarget = "ForCIPO";
            }


            var kvSurveyType = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyType", "SelfAssessment");
            var kvSurveyTarget = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyTarget", surveyTarget);


            var surveyFromDb = this.repository.AllReadonly<Survey>(x =>
                        x.Year == year &&
                        x.IdSurveyTarget == kvSurveyTarget.IdKeyValue &&
                        x.IdSurveyТype == kvSurveyType.IdKeyValue);

            return await surveyFromDb.To<SurveyVM>().FirstOrDefaultAsync();
        }

        public async Task<List<SurveyResultVM>> GetSurveyResultsForReportWithIncludesByIdSurveyAsync(int idSurvey, bool isResultForNAPOO = true)
        {
            var surveyResultsFromDb = isResultForNAPOO
                ? this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == idSurvey && x.EndDate.HasValue)
                : this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == idSurvey && x.EndDate.HasValue && x.IdCandidate_Provider == this.UserProps.IdCandidateProvider);

            return await surveyResultsFromDb.To<SurveyResultVM>(x => x.Survey.Questions.OrderBy(n => n.Order).ThenBy(n => n.Text), x => x.Survey.Questions.Select(y => y.Answers.Select(ua => ua.UserAnswers.Select(o => o.UserAnswerOpen)))).ToListAsync();
        }

        public async Task<List<SurveyResultVM>> GetSurveyResultsForReportWithSurveyResultIncludesByIdSurveyAsync(int idSurvey, bool isResultForNAPOO = true)
        {
            var surveyResultsFromDb = isResultForNAPOO
                ? this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == idSurvey && x.EndDate.HasValue)
                : this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == idSurvey && x.EndDate.HasValue && x.IdCandidate_Provider == this.UserProps.IdCandidateProvider);

            return await surveyResultsFromDb.To<SurveyResultVM>(x => x.Survey.Questions.OrderBy(n => n.Order).ThenBy(n => n.Text),
                x => x.UserAnswerOpens,
                x => x.ClientCourse.Course.Program,
                x => x.ConsultingClient,
                x => x.ValidationClient,
                x => x.CandidateProvider).OrderBy(x => x.IdCandidate_Provider).ToListAsync();
        }

        public async Task<ResultContext<NoResult>> DeleteSurveyByIdAsync(int idSurvey)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var kvForSurveyTypeSelfAssessment = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyType", "SelfAssessment");
                

                var surveyFromDb = await this.repository.GetByIdAsync<Survey>(idSurvey);

                /// Ако анкетата е за самооценка  се проверява дали има резултати
                /// За останалите анкети се проверява статуса да е съзаден
                if (surveyFromDb.IdSurveyТype == kvForSurveyTypeSelfAssessment.IdKeyValue)
                {
                    var countSurveyResult = await this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == idSurvey).CountAsync();
                    if (countSurveyResult > 0) 
                    {
                        result.AddErrorMessage("Не можете да изтриете този шаблон, защото има доклади попълнени от ЦПО/ЦИПО!");
                        return result;
                    }
                }
                else {
                    var kvSurveyStatusTypeCreated = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyStatusType", "Created");

                    if (surveyFromDb.IdSurveyStatus != kvSurveyStatusTypeCreated.IdKeyValue) 
                    {
                        result.AddErrorMessage("Не можете да изтриете тази анкета, защото е на статус изпратена.");
                        return result;
                    }

                }


                if (surveyFromDb is not null)
                {
                    var questions = await this.repository.AllReadonly<Question>(x => x.IdSurvey == surveyFromDb.IdSurvey).ToListAsync();
                    if (questions.Any())
                    {
                        List<Answer> answers = new List<Answer>();
                        foreach (var question in questions)
                        {
                            var answersFromQuestion = await this.repository.AllReadonly<Answer>(x => x.IdQuestion == question.IdQuestion).ToListAsync();
                            if (answersFromQuestion.Any())
                            {
                                answers.AddRange(answersFromQuestion);
                            }
                        }

                        if (answers.Any())
                        {
                            this.repository.HardDeleteRange<Answer>(answers);
                            await this.repository.SaveChangesAsync();
                        }

                        this.repository.HardDeleteRange<Question>(questions);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<Survey>(surveyFromDb.IdSurvey);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<ResultContext<SurveyVM>> CreateSurveyAsync(ResultContext<SurveyVM> resultContext)
        {
            var model = resultContext.ResultContextObject;

            try
            {
                bool ifExist = await IsSurveyExist(model);

                if (ifExist is true)
                {
                    resultContext.AddErrorMessage("Не можете да създадете повече от един шаблон на доклад за самооценка в рамките на една и съща календарна година!");
                }
                else
                {
                    var surveyForDb = model.To<Survey>();
                    surveyForDb.Questions = null;

                    await this.repository.AddAsync<Survey>(surveyForDb);
                    await this.repository.SaveChangesAsync();

                    model.IdSurvey = surveyForDb.IdSurvey;
                    model.IdCreateUser = surveyForDb.IdCreateUser;
                    model.IdModifyUser = surveyForDb.IdModifyUser;
                    model.CreationDate = surveyForDb.CreationDate;
                    model.ModifyDate = surveyForDb.ModifyDate;

                    resultContext.AddMessage("Записът е успешен!");
                }
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

        private async Task<bool> IsSurveyExist(SurveyVM model)
        {
            try
            {
                var kvForCPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyTarget", "ForCPO");
                var kvForCIPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyTarget", "ForCIPO");
                var kvForSurveyType = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyType", "SelfAssessment");


                /// Ако анкетата не ми е за самооценка  не се прави проверка
                if (model.IdSurveyТype != kvForSurveyType.IdKeyValue)
                {
                    return false;
                }


                if (model.IdSurveyTarget == kvForCIPO.IdKeyValue)
                {
                    var surveyCIPO = this.repository.AllReadonly<Survey>
                                (x => x.IdSurveyТype == kvForSurveyType.IdKeyValue && x.Year == model.Year && x.IdSurveyTarget == model.IdSurveyTarget).FirstOrDefault();
                    if (surveyCIPO != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else if (model.IdSurveyTarget == kvForCPO.IdKeyValue)
                {
                    var surveyCPO = this.repository.AllReadonly<Survey>
                                (x => x.IdSurveyТype == kvForSurveyType.IdKeyValue && x.Year == model.Year && x.IdSurveyTarget == model.IdSurveyTarget).FirstOrDefault();
                    if (surveyCPO != null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return true;
        }

        public async Task<ResultContext<SurveyVM>> UpdateSurveyAsync(ResultContext<SurveyVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var surveyFromDb = await this.repository.GetByIdAsync<Survey>(model.IdSurvey);
                surveyFromDb = model.To<Survey>();
                surveyFromDb.Questions = null;

                this.repository.Update<Survey>(surveyFromDb);
                await this.repository.SaveChangesAsync();

                model.IdModifyUser = surveyFromDb.IdModifyUser;
                model.ModifyDate = surveyFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
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

        public async Task<ResultContext<SurveyVM>> SendSurveyAsync(ResultContext<SurveyVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var typeFrameworkProgramSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram");
                var surveyTargetCourse = typeFrameworkProgramSource.FirstOrDefault(x => x.IdKeyValue == model.IdTrainingCourseType!.Value);
                var kvProfessionalQualification = typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification");
                var kvPartProfession = typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession");
                var kvCareerGuidanceAndCounselling = typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "CareerGuidanceAndCounselling");
                var kvCourseRegulation1And7 = typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseRegulation1And7");
                var kvSurveyResultStatusSent = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyResultStatusType", "Sent");
                if (surveyTargetCourse is not null)
                {
                    int sentSurveysCount = 0;
                    int clientsCount = 0;
                    var keyValueIntCode = surveyTargetCourse.KeyValueIntCode;
                    if (keyValueIntCode == kvProfessionalQualification!.KeyValueIntCode || keyValueIntCode == kvPartProfession!.KeyValueIntCode || keyValueIntCode == kvCourseRegulation1And7!.KeyValueIntCode)
                    {
                        var taughtClientsForPeriod = await this.GetTaughtClientCoursesAsync(model);
                        if (!taughtClientsForPeriod.Any())
                        {
                            resultContext.AddErrorMessage("Няма лица, които да са дали съгласие за използване на информацията за контакт от НАПОО и да отговарят на посочените критерии в анкетата! Не са изпратени анкети.");
                        }
                        else
                        {
                            foreach (var client in taughtClientsForPeriod)
                            {
                                var surveyResult = await this.CreateSurveyResultForClientCourseAsync(client, model, kvSurveyResultStatusSent.IdKeyValue);
                                var surveyResultContext = this.SetSurveyResultVMResultContext(surveyResult, model);

                                await this.SendEmailToClientAsync(surveyResultContext, client.EmailAddress!);
                            }

                            sentSurveysCount = taughtClientsForPeriod.Count();
                            clientsCount = taughtClientsForPeriod.DistinctBy(p => new { p.IdIndentType, p.Indent }).Count();
                        }
                    }
                    else if (keyValueIntCode == kvCareerGuidanceAndCounselling!.KeyValueIntCode)
                    {
                        var consultedClientsForPeriod = await this.GetConsultedClientsAsync(model);
                        if (!consultedClientsForPeriod.Any())
                        {
                            resultContext.AddErrorMessage("Няма лица, които да са дали съгласие за използване на информацията за контакт от НАПОО и да отговарят на посочените критерии в анкетата! Не са изпратени анкети.");
                        }
                        else
                        {
                            foreach (var client in consultedClientsForPeriod)
                            {
                                var surveyResult = await this.CreateSurveyResultForConsultingClientAsync(client, model, kvSurveyResultStatusSent.IdKeyValue);
                                var surveyResultContext = this.SetSurveyResultVMResultContext(surveyResult, model);

                                await this.SendEmailToClientAsync(surveyResultContext, client.EmailAddress!);
                            }

                            sentSurveysCount = consultedClientsForPeriod.Count();
                            clientsCount = consultedClientsForPeriod.DistinctBy(p => new { p.IdIndentType, p.Indent }).Count();
                        }
                    }
                    else
                    {
                        var validatedClientsForPeriod = await this.GetValidatedClientsAsync(model);
                        if (!validatedClientsForPeriod.Any())
                        {
                            resultContext.AddErrorMessage("Няма лица, които да са дали съгласие за използване на информацията за контакт от НАПОО и да отговарят на посочените критерии в анкетата! Не са изпратени анкети.");
                        }
                        else
                        {
                            foreach (var client in validatedClientsForPeriod)
                            {
                                var surveyResult = await this.CreateSurveyResultForValidationClientAsync(client, model, kvSurveyResultStatusSent.IdKeyValue);
                                var surveyResultContext = this.SetSurveyResultVMResultContext(surveyResult, model);

                                await this.SendEmailToClientAsync(surveyResultContext, client.EmailAddress!);
                            }

                            sentSurveysCount = validatedClientsForPeriod.Count();
                            clientsCount = validatedClientsForPeriod.DistinctBy(p => new { p.IdIndentType, p.Indent }).Count();
                        }
                    }

                    if (!resultContext.HasErrorMessages)
                    {
                        var kvSurveyStatusSent = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyStatusType", "Sent");
                        model.IdSurveyStatus = kvSurveyStatusSent.IdKeyValue;
                        await this.UpdateSurveyStatusAsync(model.IdSurvey, kvSurveyStatusSent.IdKeyValue);

                        resultContext.AddMessage($"Изпращането на анкетата приключи успешно! Изпратени са {sentSurveysCount} броя анкети към {clientsCount} лица.");
                    }
                }
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

        public async Task<ResultContext<SurveyVM>> ReSendSurveyAsync(ResultContext<SurveyVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var surveyResults = this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == model.IdSurvey);
                var sureyResultsAsVM = await surveyResults.To<SurveyResultVM>(x => x.ClientCourse, x => x.ValidationClient, x => x.ConsultingClient).ToListAsync();

                foreach (var surveyResult in sureyResultsAsVM)
                {
                    if (surveyResult.ClientCourse is not null)
                    {
                        var surveyResultContext = this.SetSurveyResultVMResultContext(surveyResult, model);
                        await this.SendEmailToClientAsync(surveyResultContext, surveyResult.ClientCourse.EmailAddress!);
                    }

                    if (surveyResult.ValidationClient is not null)
                    {
                        var surveyResultContext = this.SetSurveyResultVMResultContext(surveyResult, model);
                        await this.SendEmailToClientAsync(surveyResultContext, surveyResult.ValidationClient.EmailAddress!);
                    }

                    if (surveyResult.ConsultingClient is not null)
                    {
                        var surveyResultContext = this.SetSurveyResultVMResultContext(surveyResult, model);
                        await this.SendEmailToClientAsync(surveyResultContext, surveyResult.ConsultingClient.EmailAddress!);
                    }
                }

                resultContext.AddMessage("Анкетата е препратена успешно!");
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

        public async Task<(MemoryStream MS, bool IsMacroIncluded)> GetSurveyTemplateWithQuestionsFilledByIdSurveyAsync(int idSurvey)
        {
            var survey = await this.repository.AllReadonly<Survey>(x => x.IdSurvey == idSurvey)
                .Include(x => x.Questions.OrderBy(y => y.Order))
                    .ThenInclude(x => x.Answers)
                        .AsNoTracking()
                .FirstOrDefaultAsync();

            if (survey is not null)
            {
                var trainingCourseTypesSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.DefaultValue3 == "survey").ToList();
                var kvCourseSPK = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification");
                var kvCoursePP = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession");
                var kvConsulting = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CareerGuidanceAndCounselling");
                var kvValidationPP = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfPartOfProfession");
                var kvValidationSPK = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");
                var kvLegalCapacity = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseRegulation1And7");
                var questionTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QuestionType");
                var kvMultipleQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Multiple");
                var kvSingleQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Single");
                var kvMultipleOpenQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "MultipleOpen");
                var kvSingleOpenQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "SingleOpen");
                var indentTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");

                using (ExcelEngine excelEngine = new ExcelEngine())
                {
                    IApplication application = excelEngine.Excel;
                    application.DefaultVersion = ExcelVersion.Excel2016;

                    IWorkbook workbook = application.Workbooks.Create(1);
                    IWorksheet sheet = workbook.Worksheets[0];

                    var procedureCounter = 1;
                    var command = new StringBuilder();
                    var procedure = new StringBuilder();
                    if (survey.IdTrainingCourseType == kvConsulting!.IdKeyValue
                        || survey.IdTrainingCourseType == kvValidationPP!.IdKeyValue
                        || survey.IdTrainingCourseType == kvValidationSPK!.IdKeyValue)
                    {
                        sheet.Range["A1"].ColumnWidth = 20;
                        sheet.Range[$"A1"].Text = "№ на лицензия";
                        sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                        sheet.Range["B1"].ColumnWidth = 30;
                        sheet.Range[$"B1"].Text = "Вид на идентификатора";
                        sheet.Range[$"B2"].Text = "ЕГН";
                        sheet.Range[$"B1"].CellStyle.Font.Bold = true;
                        sheet.Range["C1"].ColumnWidth = 20;
                        sheet.Range[$"C1"].Text = "ЕГН/ЛНЧ/ИДН";
                        sheet.Range[$"C1"].CellStyle.Font.Bold = true;

                        IDataValidation indentDataValidation = sheet.Range["B2:B5000"].DataValidation;
                        indentDataValidation.AllowType = ExcelDataType.User;
                        indentDataValidation.ListOfValues = indentTypesSource.Select(x => x.Name).ToArray();

                        var headerCounter = 4;
                        foreach (var question in survey.Questions)
                        {
                            var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                            sheet.Range[$"{headerSymbol}1"].Text = question.Text;
                            sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                            sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                            sheet.Range[$"{headerSymbol}1"].WrapText = true;

                            if (question.IdQuestType == kvMultipleQuestion!.IdKeyValue || question.IdQuestType == kvMultipleOpenQuestion!.IdKeyValue)
                            {
                                IDataValidation answersDataValidation = sheet.Range[$"{headerSymbol}2:{headerSymbol}5000"].DataValidation;
                                answersDataValidation.AllowType = ExcelDataType.User;
                                var answerTexts = question.Answers.OrderBy(x => x.IdAnswer).Select(x => x.Text).ToList();
                                var countSymbols = answerTexts.Sum(x => x.Length);
                                if (countSymbols > 250 || question.Answers.Any(x => x.Text.Contains(",")))
                                {
                                    workbook.Allow3DRangesInDataValidation = true;

                                    IWorksheet namedSheet = workbook.Worksheets.Create($"{question.IdQuestion}");
                                    namedSheet.Visibility = WorksheetVisibility.Hidden;
                                    var counter = 1;
                                    foreach (var answer in answerTexts)
                                    {
                                        namedSheet.Range[$"A{counter++}"].Text = answer;
                                    }

                                    answersDataValidation.ListOfValues = new string[] { };
                                    answersDataValidation.DataRange = workbook.Worksheets[$"{question.IdQuestion}"].Range[$"A1:A{counter}"];
                                }
                                else
                                {
                                    answersDataValidation.ListOfValues = answerTexts.Select(x => x.ToString()).ToArray();
                                }

                                command.AppendLine($"Sub Procedure{procedureCounter++}(ByVal Target As Range)\nDim Oldvalue As String\nDim Newvalue As String\nApplication.EnableEvents = True\nOn Error GoTo Exitsub\n");
                                for (int i = 2; i <= 150; i++)
                                {
                                    command.AppendLine($"If Target.Address = \"${headerSymbol}${i}\" Then\n  If Target.SpecialCells(xlCellTypeAllValidation) Is Nothing Then\n    GoTo Exitsub\n  Else: If Target.Value = \"\" Then GoTo Exitsub Else\n    Application.EnableEvents = False\n    Newvalue = Target.Value\n    Application.Undo\n    Oldvalue = Target.Value\n      If Oldvalue = \"\" Then\n        Target.Value = Newvalue\n      Else\n        If InStr(1, Oldvalue, Newvalue) = 0 Then\n            Target.Value = Oldvalue & \"; \" & Newvalue\n      Else:\n        Target.Value = Oldvalue\n      End If\n    End If\n  End If\nEnd If");
                                }

                                command.AppendLine("\nApplication.EnableEvents = True\nExitsub:\nApplication.EnableEvents = True\nEnd Sub\n");
                            }
                            else if (question.IdQuestType == kvSingleQuestion!.IdKeyValue || question.IdQuestType == kvSingleOpenQuestion!.IdKeyValue)
                            {
                                IDataValidation answersDataValidation = sheet.Range[$"{headerSymbol}2:{headerSymbol}5000"].DataValidation;
                                answersDataValidation.AllowType = ExcelDataType.User;
                                var answerTexts = question.Answers.OrderBy(x => x.IdAnswer).Select(x => x.Text).ToList();
                                var countSymbols = answerTexts.Sum(x => x.Length);
                                if (countSymbols > 250 || question.Answers.Any(x => x.Text.Contains(",")))
                                {
                                    workbook.Allow3DRangesInDataValidation = true;

                                    IWorksheet namedSheet = workbook.Worksheets.Create($"{question.IdQuestion}");
                                    namedSheet.Visibility = WorksheetVisibility.Hidden;
                                    var counter = 1;
                                    foreach (var answer in answerTexts)
                                    {
                                        namedSheet.Range[$"A{counter++}"].Text = answer;
                                    }

                                    answersDataValidation.ListOfValues = new string[] { };
                                    answersDataValidation.DataRange = workbook.Worksheets[$"{question.IdQuestion}"].Range[$"A1:A{counter}"];
                                }
                                else
                                {
                                    answersDataValidation.ListOfValues = answerTexts.Select(x => x.ToString()).ToArray();
                                }
                            }

                            if (question.IdQuestType == kvSingleOpenQuestion!.IdKeyValue || question.IdQuestType == kvMultipleOpenQuestion!.IdKeyValue)
                            {
                                headerCounter++;
                                headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                                sheet.Range[$"{headerSymbol}1"].Text = $"Отворен отговор към въпрос {question.Text}";
                                sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                                sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                                sheet.Range[$"{headerSymbol}1"].WrapText = true;
                            }

                            headerCounter++;
                        }
                    }
                    else
                    {
                        sheet.Range["A1"].ColumnWidth = 20;
                        sheet.Range[$"A1"].Text = "№ на лицензия";
                        sheet.Range[$"A1"].CellStyle.Font.Bold = true;
                        sheet.Range["B1"].ColumnWidth = 20;
                        sheet.Range[$"B1"].Text = "Име на курс";
                        sheet.Range[$"B1"].CellStyle.Font.Bold = true;
                        sheet.Range["C1"].ColumnWidth = 30;
                        sheet.Range[$"C1"].Text = "Вид на идентификатора";
                        sheet.Range[$"C1"].CellStyle.Font.Bold = true;
                        sheet.Range[$"C2"].Text = "ЕГН";
                        sheet.Range["D1"].ColumnWidth = 20;
                        sheet.Range[$"D1"].Text = "ЕГН/ЛНЧ/ИДН";
                        sheet.Range[$"D1"].CellStyle.Font.Bold = true;

                        IDataValidation indentDataValidation = sheet.Range["C2:C5000"].DataValidation;
                        indentDataValidation.AllowType = ExcelDataType.User;
                        indentDataValidation.ListOfValues = indentTypesSource.Select(x => x.Name).ToArray();

                        var headerCounter = 5;
                        foreach (var question in survey.Questions)
                        {
                            var headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                            sheet.Range[$"{headerSymbol}1"].Text = question.Text;
                            sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                            sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                            sheet.Range[$"{headerSymbol}1"].WrapText = true;

                            if (question.IdQuestType == kvMultipleQuestion!.IdKeyValue || question.IdQuestType == kvMultipleOpenQuestion!.IdKeyValue)
                            {
                                IDataValidation answersDataValidation = sheet.Range[$"{headerSymbol}2:{headerSymbol}5000"].DataValidation;
                                answersDataValidation.AllowType = ExcelDataType.User;
                                var answerTexts = question.Answers.OrderBy(x => x.IdAnswer).Select(x => x.Text).ToList();
                                var countSymbols = answerTexts.Sum(x => x.Length);
                                if (countSymbols > 250 || question.Answers.Any(x => x.Text.Contains(",")))
                                {
                                    workbook.Allow3DRangesInDataValidation = true;

                                    IWorksheet namedSheet = workbook.Worksheets.Create($"{question.IdQuestion}");
                                    namedSheet.Visibility = WorksheetVisibility.Hidden;
                                    var counter = 1;
                                    foreach (var answer in answerTexts)
                                    {
                                        namedSheet.Range[$"A{counter++}"].Text = answer;
                                    }

                                    answersDataValidation.ListOfValues = new string[] { };
                                    answersDataValidation.DataRange = workbook.Worksheets[$"{question.IdQuestion}"].Range[$"A1:A{counter}"];
                                }
                                else
                                {
                                    answersDataValidation.ListOfValues = answerTexts.Select(x => x.ToString()).ToArray();
                                }

                                command.AppendLine($"Sub Procedure{procedureCounter++}(ByVal Target As Range)\nDim Oldvalue As String\nDim Newvalue As String\nApplication.EnableEvents = True\nOn Error GoTo Exitsub\n");
                                for (int i = 2; i <= 150; i++)
                                {
                                    command.AppendLine($"If Target.Address = \"${headerSymbol}${i}\" Then\n  If Target.SpecialCells(xlCellTypeAllValidation) Is Nothing Then\n    GoTo Exitsub\n  Else: If Target.Value = \"\" Then GoTo Exitsub Else\n    Application.EnableEvents = False\n    Newvalue = Target.Value\n    Application.Undo\n    Oldvalue = Target.Value\n      If Oldvalue = \"\" Then\n        Target.Value = Newvalue\n      Else\n        If InStr(1, Oldvalue, Newvalue) = 0 Then\n            Target.Value = Oldvalue & \"; \" & Newvalue\n      Else:\n        Target.Value = Oldvalue\n      End If\n    End If\n  End If\nEnd If");
                                }

                                command.AppendLine("\nApplication.EnableEvents = True\nExitsub:\nApplication.EnableEvents = True\nEnd Sub\n");
                            }
                            else if (question.IdQuestType == kvSingleQuestion!.IdKeyValue || question.IdQuestType == kvSingleOpenQuestion!.IdKeyValue)
                            {
                                IDataValidation answersDataValidation = sheet.Range[$"{headerSymbol}2:{headerSymbol}5000"].DataValidation;
                                answersDataValidation.AllowType = ExcelDataType.User;
                                var answerTexts = question.Answers.OrderBy(x => x.IdAnswer).Select(x => x.Text).ToList();
                                var countSymbols = answerTexts.Sum(x => x.Length);
                                if (countSymbols > 250 || question.Answers.Any(x => x.Text.Contains(",")))
                                {
                                    workbook.Allow3DRangesInDataValidation = true;

                                    IWorksheet namedSheet = workbook.Worksheets.Create($"{question.IdQuestion}");
                                    namedSheet.Visibility = WorksheetVisibility.Hidden;
                                    var counter = 1;
                                    foreach (var answer in answerTexts)
                                    {
                                        namedSheet.Range[$"A{counter++}"].Text = answer;
                                    }

                                    answersDataValidation.ListOfValues = new string[] { };
                                    answersDataValidation.DataRange = workbook.Worksheets[$"{question.IdQuestion}"].Range[$"A1:A{counter}"];
                                }
                                else
                                {
                                    answersDataValidation.ListOfValues = answerTexts.Select(x => x.ToString()).ToArray();
                                }
                            }

                            if (question.IdQuestType == kvSingleOpenQuestion!.IdKeyValue || question.IdQuestType == kvMultipleOpenQuestion!.IdKeyValue)
                            {
                                headerCounter++;
                                headerSymbol = this.GetExcelHeaderSymbolByNumber(headerCounter);
                                sheet.Range[$"{headerSymbol}1"].Text = $"Отворен отговор към въпрос {question.Text}";
                                sheet.Range[$"{headerSymbol}1"].ColumnWidth = 20;
                                sheet.Range[$"{headerSymbol}1"].CellStyle.Font.Bold = true;
                                sheet.Range[$"{headerSymbol}1"].WrapText = true;
                            }

                            headerCounter++;
                        }
                    }

                    if (survey.Questions.Any(x => x.IdQuestType == kvMultipleOpenQuestion!.IdKeyValue || x.IdQuestType == kvMultipleQuestion!.IdKeyValue))
                    {
                        //Creating Vba project
                        IVbaProject project = workbook.VbaProject;

                        //Accessing vba modules collection
                        IVbaModules vbaModules = project.Modules;

                        //Accessing sheet module
                        IVbaModule vbaModule = vbaModules[sheet.CodeName];

                        var builder = new StringBuilder();
                        //Adding vba code to the module
                        for (int i = 1; i < procedureCounter; i++)
                        {
                            builder.AppendLine($"Call Procedure{i}(Target)");
                        }

                        vbaModule.Code = $"Private Sub Worksheet_Change(ByVal Target As Range)\nDim Oldvalue As String\nDim Newvalue As String\nApplication.EnableEvents = True\nOn Error GoTo Exitsub\n{builder.ToString()}\nApplication.EnableEvents = True\nExitsub:\nApplication.EnableEvents = True\nEnd Sub\n {command.ToString()}";
                        //vbaModule.Code = $"Private Sub Worksheet_Change(ByVal Target As Range)\nDim Oldvalue As String\nDim Newvalue As String\nApplication.EnableEvents = True\nOn Error GoTo Exitsub\n{command.ToString()}\nApplication.EnableEvents = True\nExitsub:\nApplication.EnableEvents = True\nEnd Sub\n";

                        using (MemoryStream stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream, ExcelSaveType.SaveAsMacro);
                            return (stream, true);
                        }
                    }
                    else
                    {
                        using (MemoryStream stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            return (stream, false);
                        }
                    }
                }
            }

            return (null, false);
        }

        public async Task<ResultContext<List<SurveyResultVM>>> ImportSurveyResultsAsync(MemoryStream file, string fileName, SurveyVM survey)
        {
            ResultContext<List<SurveyResultVM>> resultContext = new ResultContext<List<SurveyResultVM>>();
            List<SurveyResultVM> surveyResults = new List<SurveyResultVM>();
            try
            {
                int counter = GlobalConstants.INVALID_ID_ZERO;

                var settingResource = (await this.dataSourceService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
                var filePathMain = $"\\UploadedFiles\\Temp\\ImportSurveyResults";
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

                        var trainingCourseTypesSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.DefaultValue3 == "survey").ToList();
                        var kvCourseSPK = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ProfessionalQualification");
                        var kvCoursePP = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "PartProfession");
                        var kvConsulting = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CareerGuidanceAndCounselling");
                        var kvValidationPP = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfPartOfProfession");
                        var kvValidationSPK = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");
                        var kvLegalCapacity = trainingCourseTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseRegulation1And7");
                        var kvLicenseTypeCPO = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
                        var questionTypesSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("QuestionType");
                        var kvMultipleQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Multiple");
                        var kvSingleQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Single");
                        var kvMultipleOpenQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "MultipleOpen");
                        var kvSingleOpenQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "SingleOpen");
                        var kvOpenQuestion = questionTypesSource.FirstOrDefault(x => x.KeyValueIntCode == "Open");
                        var kvSurveyResultFilled = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyResultStatusType", "Filled");

                        if (survey.IdTrainingCourseType == kvConsulting!.IdKeyValue
                        || survey.IdTrainingCourseType == kvValidationPP!.IdKeyValue
                        || survey.IdTrainingCourseType == kvValidationSPK!.IdKeyValue)
                        {
                            if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                                || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                                || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text))
                            {
                                resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на резултати от анкета!");
                                return resultContext;
                            }

                            var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                            var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                            var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                            bool skipFirstRow = true;

                            //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                            if (firstHeader != "№ на лицензия" || secondHeader != "Вид на идентификатора" || thirdHeader != "ЕГН/ЛНЧ/ИДН")
                            {
                                resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на резултати от анкета!");
                                return resultContext;
                            }

                            var rowCounter = 2;
                            foreach (var row in worksheet.Rows)
                            {
                                //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                                if (counter == 5)
                                {
                                    break;
                                }

                                var surveyResult = new SurveyResultVM();
                                surveyResult.IdSurvey = survey.IdSurvey;
                                surveyResult.IdStatus = kvSurveyResultFilled.IdKeyValue;

                                var licenceNumber = row.Cells[0].Value.Trim();

                                //Пропуска 1 ред който е с хедърите
                                if (skipFirstRow || string.IsNullOrEmpty(licenceNumber))
                                {
                                    skipFirstRow = false;
                                    counter++;
                                    continue;
                                }

                                CandidateProvider candidateProvider = null;
                                if (string.IsNullOrEmpty(licenceNumber))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен номер на лицензия!");
                                }
                                else
                                {
                                    long licenceAsLong;
                                    if (!long.TryParse(licenceNumber, out licenceAsLong))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен валиден номер на лицензия. Стойността може да съдържа само цифри!");
                                    }
                                    else
                                    {
                                        candidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => !string.IsNullOrEmpty(x.LicenceNumber) && x.LicenceNumber == licenceNumber).FirstOrDefaultAsync();
                                        if (candidateProvider is null)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен валиден номер на лицензия. Няма открит запис за такъв център!");
                                        }
                                        else
                                        {
                                            surveyResult.IdCandidate_Provider = candidateProvider.IdCandidate_Provider;
                                        }
                                    }
                                }

                                var kvIdentSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
                                var kvEGN = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "EGN");
                                var kvLNCh = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "LNK");
                                var kvIDN = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "IDN");
                                var identList = new List<string>()
                                {
                                    { kvEGN.Name.Trim().ToLower() },
                                    { kvLNCh.Name.Trim().ToLower() },
                                    { kvIDN.Name.Trim().ToLower() }
                                };

                                var identType = row.Cells[1].Value.Trim().ToLower();
                                var ident = row.Cells[2].Value.Trim();
                                if (string.IsNullOrEmpty(identType))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведен Вид на идентификатор!");
                                }
                                else
                                {
                                    if (!identList.Any(x => x == identType))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведенa валидна стойност за Вид на идентификатор!");
                                    }
                                    else
                                    {
                                        var kvIdent = kvIdentSource.FirstOrDefault(x => x.Name.ToLower() == identType.ToLower().Trim());
                                        if (kvIdent is not null)
                                        {
                                            if (string.IsNullOrEmpty(ident))
                                            {
                                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за ЕГН/ЛНЧ/ИДН!");
                                            }
                                            else
                                            {
                                                if (ident.Length != 10)
                                                {
                                                    resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за ЕГН/ЛНЧ/ИДН не може да съдържа по-малко или повече от 10 символа!");
                                                }
                                                else
                                                {
                                                    if (identType == kvEGN.Name.Trim().ToLower())
                                                    {
                                                        var checkEGN = new BasicEGNValidation(ident);
                                                        if (!checkEGN.Validate())
                                                        {
                                                            resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за ЕГН е невалидна!");
                                                        }
                                                        else
                                                        {
                                                            if (candidateProvider is not null)
                                                            {
                                                                if (candidateProvider.IdTypeApplication == kvLicenseTypeCPO.IdKeyValue)
                                                                {
                                                                    var validationClient = await this.repository.AllReadonly<ValidationClient>(x => x.IdCandidateProvider == candidateProvider.IdCandidate_Provider
                                                                     && x.IdIndentType == kvIdent.IdKeyValue && x.Indent == ident).FirstOrDefaultAsync();
                                                                    if (validationClient is null)
                                                                    {
                                                                        resultContext.AddErrorMessage($"На ред {rowCounter} не е намерено обучено лице с тази стойност на идентификатор при ЦПО, с този номер на лицензия!");
                                                                    }
                                                                    else
                                                                    {
                                                                        surveyResult.IdValidationClient = validationClient.IdValidationClient;
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    var consultingClient = await this.repository.AllReadonly<ConsultingClient>(x => x.IdCandidateProvider == candidateProvider.IdCandidate_Provider
                                                                     && x.IdIndentType == kvIdent.IdKeyValue && x.Indent == ident).FirstOrDefaultAsync();
                                                                    if (consultingClient is null)
                                                                    {
                                                                        resultContext.AddErrorMessage($"На ред {rowCounter} не е намерено консултирано лице с тази стойност на идентификатор при ЦИПО, с този номер на лицензия!");
                                                                    }
                                                                    else
                                                                    {
                                                                        surveyResult.IdConsultingClient = consultingClient.IdConsultingClient;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (candidateProvider is not null)
                                                        {
                                                            if (candidateProvider.IdTypeApplication == kvLicenseTypeCPO.IdKeyValue)
                                                            {
                                                                var validationClient = await this.repository.AllReadonly<ValidationClient>(x => x.IdCandidateProvider == candidateProvider.IdCandidate_Provider
                                                                 && x.IdIndentType == kvIdent.IdKeyValue && x.Indent == ident).FirstOrDefaultAsync();
                                                                if (validationClient is null)
                                                                {
                                                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е намерено обучено лице с тази стойност на идентификатор при ЦПО, с този номер на лицензия!");
                                                                }
                                                                else
                                                                {
                                                                    surveyResult.IdValidationClient = validationClient.IdValidationClient;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                var consultingClient = await this.repository.AllReadonly<ConsultingClient>(x => x.IdCandidateProvider == candidateProvider.IdCandidate_Provider
                                                                 && x.IdIndentType == kvIdent.IdKeyValue && x.Indent == ident).FirstOrDefaultAsync();
                                                                if (consultingClient is null)
                                                                {
                                                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е намерено консултирано лице с тази стойност на идентификатор при ЦИПО, с този номер на лицензия!");
                                                                }
                                                                else
                                                                {
                                                                    surveyResult.IdConsultingClient = consultingClient.IdConsultingClient;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                for (int i = 3; i < worksheet.Rows[0].Columns.Count(); i++)
                                {
                                    var questionFromHeader = worksheet.Rows[0].Columns[i].Text;
                                    if (string.IsNullOrEmpty(questionFromHeader))
                                    {
                                        break;
                                    }

                                    if (questionFromHeader.StartsWith("Отворен отговор към въпрос "))
                                    {
                                        continue;
                                    }

                                    var questionsFromSurvey = survey.Questions.Where(x => x.Text == questionFromHeader);
                                    if (questionsFromSurvey.Any())
                                    {
                                        foreach (var questionFromSurvey in questionsFromSurvey)
                                        {
                                            if (questionFromSurvey.IdQuestType == kvOpenQuestion!.IdKeyValue)
                                            {
                                                var userAnswerOpen = new UserAnswerOpenVM();
                                                var userAnswer = new UserAnswerVM();
                                                userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswerOpen.Text = worksheet.Rows[rowCounter - 1].Columns[i].Value != null ? worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim() : string.Empty;
                                                userAnswerOpen.UserAnswers.Add(userAnswer);
                                                surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                            }
                                            else if (questionFromSurvey.IdQuestType == kvSingleQuestion!.IdKeyValue)
                                            {
                                                var userAnswerOpen = new UserAnswerOpenVM();
                                                var userAnswer = new UserAnswerVM();
                                                userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswerOpen.Text = string.Empty;
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answerFromImport);
                                                if (answerFromSurvey is null)
                                                {
                                                    resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answerFromImport} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                }
                                                else
                                                {
                                                    userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                    userAnswerOpen.UserAnswers.Add(userAnswer);
                                                    surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                }
                                            }
                                            else if (questionFromSurvey.IdQuestType == kvMultipleQuestion!.IdKeyValue)
                                            {
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromImportAsArr = answerFromImport.Split("; ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var answer in answerFromImportAsArr)
                                                {
                                                    var userAnswerOpen = new UserAnswerOpenVM();
                                                    var userAnswer = new UserAnswerVM();
                                                    userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswerOpen.Text = string.Empty;
                                                    var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answer);
                                                    if (answerFromSurvey is null)
                                                    {
                                                        resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answer} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                    }
                                                    else
                                                    {
                                                        userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                        userAnswerOpen.UserAnswers.Add(userAnswer);
                                                        surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                    }
                                                }
                                            }
                                            else if (questionFromSurvey.IdQuestType == kvSingleOpenQuestion!.IdKeyValue)
                                            {
                                                var userAnswerOpen = new UserAnswerOpenVM();
                                                var userAnswer = new UserAnswerVM();
                                                userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswerOpen.Text = worksheet.Rows[rowCounter - 1].Columns[i + 1].Value != null ? worksheet.Rows[rowCounter - 1].Columns[i + 1].Value.Trim() : string.Empty;
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answerFromImport);
                                                if (answerFromSurvey is null)
                                                {
                                                    resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answerFromImport} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                }
                                                else
                                                {
                                                    userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                    userAnswerOpen.UserAnswers.Add(userAnswer);
                                                    surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                }
                                            }
                                            else
                                            {
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromImportAsArr = answerFromImport.Split("; ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var answer in answerFromImportAsArr)
                                                {
                                                    var userAnswerOpen = new UserAnswerOpenVM();
                                                    var userAnswer = new UserAnswerVM();
                                                    userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswerOpen.Text = worksheet.Rows[rowCounter - 1].Columns[i + 1].Value != null ? worksheet.Rows[rowCounter - 1].Columns[i + 1].Value.Trim() : string.Empty;
                                                    var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answer);
                                                    if (answerFromSurvey is null)
                                                    {
                                                        resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answer} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                    }
                                                    else
                                                    {
                                                        userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                        userAnswerOpen.UserAnswers.Add(userAnswer);
                                                        surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"Въпрос {questionFromHeader} не е намерен в анкетата, за която се импортират резултати!");
                                    }
                                }

                                rowCounter++;

                                surveyResults.Add(surveyResult);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(worksheet.Rows[0].Columns[0].Text)
                                || string.IsNullOrEmpty(worksheet.Rows[0].Columns[1].Text)
                                || string.IsNullOrEmpty(worksheet.Rows[0].Columns[2].Text)
                                || string.IsNullOrEmpty(worksheet.Rows[0].Columns[3].Text))
                            {
                                resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на резултати от анкета!");
                                return resultContext;
                            }

                            var firstHeader = worksheet.Rows[0].Columns[0].Text.Trim();
                            var secondHeader = worksheet.Rows[0].Columns[1].Text.Trim();
                            var thirdHeader = worksheet.Rows[0].Columns[2].Text.Trim();
                            var fourthHeader = worksheet.Rows[0].Columns[3].Text.Trim();
                            bool skipFirstRow = true;

                            //Проверка по 1 клетка за да се провери дали файла за импорт на учебна програма
                            if (firstHeader != "№ на лицензия" || secondHeader != "Име на курс" || thirdHeader != "Вид на идентификатора" || fourthHeader != "ЕГН/ЛНЧ/ИДН")
                            {
                                resultContext.AddErrorMessage("Файлът, който се опитвате да качите, не отговаря на шаблона за импорт на резултати от анкета!");
                                return resultContext;
                            }

                            var rowCounter = 2;
                            foreach (var row in worksheet.Rows)
                            {
                                //Ако сме пропуснали 5 реда приемаме че документа е приключил и са останали празни редове
                                if (counter == 5)
                                {
                                    break;
                                }

                                var licenceNumber = row.Cells[0].Value.Trim();

                                var surveyResult = new SurveyResultVM();
                                surveyResult.IdSurvey = survey.IdSurvey;
                                surveyResult.IdStatus = kvSurveyResultFilled.IdKeyValue;

                                //Пропуска 1 ред който е с хедърите
                                if (skipFirstRow || string.IsNullOrEmpty(licenceNumber))
                                {
                                    skipFirstRow = false;
                                    counter++;
                                    continue;
                                }

                                CandidateProvider candidateProvider = null;
                                if (string.IsNullOrEmpty(licenceNumber))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен номер на лицензия!");
                                }
                                else
                                {
                                    long licenceAsLong;
                                    if (!long.TryParse(licenceNumber, out licenceAsLong))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен валиден номер на лицензия. Стойността може да съдържа само цифри!");
                                    }
                                    else
                                    {
                                        candidateProvider = await this.repository.AllReadonly<CandidateProvider>(x => !string.IsNullOrEmpty(x.LicenceNumber) && x.LicenceNumber == licenceNumber).FirstOrDefaultAsync();
                                        if (candidateProvider is null)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен валиден номер на лицензия. Няма открит запис за такъв център!");
                                        }
                                        else
                                        {
                                            surveyResult.IdCandidate_Provider = candidateProvider.IdCandidate_Provider;
                                        }
                                    }
                                }

                                Course course = null;
                                var courseNameFromImport = row.Cells[1].Value.Trim();
                                if (string.IsNullOrEmpty(courseNameFromImport))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е въведено име на курс!");
                                }
                                else
                                {
                                    if (candidateProvider is not null)
                                    {
                                        course = await this.repository.AllReadonly<Course>(x => x.IdCandidateProvider == candidateProvider.IdCandidate_Provider && x.CourseName.ToLower() == courseNameFromImport.ToLower()).FirstOrDefaultAsync();
                                        if (course is null)
                                        {
                                            resultContext.AddErrorMessage($"На ред {rowCounter} не е въведен валиден курс, който да е проведен от въведеното ЦПО!");
                                        }
                                    }
                                }

                                var kvIdentSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
                                var kvEGN = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "EGN");
                                var kvLNCh = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "LNK");
                                var kvIDN = kvIdentSource.FirstOrDefault(x => x.KeyValueIntCode == "IDN");
                                var identList = new List<string>()
                                {
                                    { kvEGN.Name.Trim().ToLower() },
                                    { kvLNCh.Name.Trim().ToLower() },
                                    { kvIDN.Name.Trim().ToLower() }
                                };

                                var identType = row.Cells[2].Value.Trim().ToLower();
                                var ident = row.Cells[3].Value.Trim();
                                if (string.IsNullOrEmpty(identType))
                                {
                                    resultContext.AddErrorMessage($"На ред {rowCounter} няма въведен Вид на идентификатор!");
                                }
                                else
                                {
                                    if (!identList.Any(x => x == identType))
                                    {
                                        resultContext.AddErrorMessage($"На ред {rowCounter} няма въведенa валидна стойност за Вид на идентификатор!");
                                    }
                                    else
                                    {
                                        var kvIdent = kvIdentSource.FirstOrDefault(x => x.Name.ToLower() == identType.ToLower().Trim());
                                        if (kvIdent is not null)
                                        {
                                            if (string.IsNullOrEmpty(ident))
                                            {
                                                resultContext.AddErrorMessage($"На ред {rowCounter} няма въведена стойност за ЕГН/ЛНЧ/ИДН!");
                                            }
                                            else
                                            {
                                                if (ident.Length != 10)
                                                {
                                                    resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за ЕГН/ЛНЧ/ИДН не може да съдържа по-малко или повече от 10 символа!");
                                                }
                                                else
                                                {
                                                    if (identType == kvEGN.Name.Trim().ToLower())
                                                    {
                                                        var checkEGN = new BasicEGNValidation(ident);
                                                        if (!checkEGN.Validate())
                                                        {
                                                            resultContext.AddErrorMessage($"На ред {rowCounter} въведената стойност за ЕГН е невалидна!");
                                                        }
                                                        else
                                                        {
                                                            if (candidateProvider is not null && course is not null)
                                                            {
                                                                var clientCourse = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == course.IdCourse
                                                                 && x.IdIndentType == kvIdent.IdKeyValue && x.Indent == ident).FirstOrDefaultAsync();
                                                                if (clientCourse is null)
                                                                {
                                                                    resultContext.AddErrorMessage($"На ред {rowCounter} не е намерено обучено лице с тази стойност на идентификатор при ЦПО, с това име на курс!");
                                                                }
                                                                else
                                                                {
                                                                    surveyResult.IdClientCourse = clientCourse.IdClientCourse;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (candidateProvider is not null && course is not null)
                                                        {
                                                            var clientCourse = await this.repository.AllReadonly<ClientCourse>(x => x.IdCourse == course.IdCourse
                                                             && x.IdIndentType == kvIdent.IdKeyValue && x.Indent == ident).FirstOrDefaultAsync();
                                                            if (clientCourse is null)
                                                            {
                                                                resultContext.AddErrorMessage($"На ред {rowCounter} не е намерено обучено лице с тази стойност на идентификатор при ЦПО, с това име на курс!");
                                                            }
                                                            else
                                                            {
                                                                surveyResult.IdClientCourse = clientCourse.IdClientCourse;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                
                                for (int i = 4; i < worksheet.Rows[0].Columns.Count(); i++)
                                {
                                    var questionFromHeader = worksheet.Rows[0].Columns[i].Value;
                                    if (string.IsNullOrEmpty(questionFromHeader))
                                    {
                                        break;
                                    }

                                    if (questionFromHeader.StartsWith("Отворен отговор към въпрос "))
                                    {
                                        continue;
                                    }

                                    var questionsFromSurvey = survey.Questions.Where(x => x.Text == questionFromHeader);
                                    if (questionsFromSurvey.Any())
                                    {
                                        foreach (var questionFromSurvey in questionsFromSurvey)
                                        {
                                            if (questionFromSurvey.IdQuestType == kvOpenQuestion!.IdKeyValue)
                                            {
                                                var userAnswerOpen = new UserAnswerOpenVM();
                                                var userAnswer = new UserAnswerVM();
                                                userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswerOpen.Text = !string.IsNullOrEmpty(worksheet.Rows[rowCounter - 1].Columns[i].Value) ? worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim() : string.Empty;
                                                userAnswerOpen.UserAnswers.Add(userAnswer);
                                                surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                            }
                                            else if (questionFromSurvey.IdQuestType == kvSingleQuestion!.IdKeyValue)
                                            {
                                                var userAnswerOpen = new UserAnswerOpenVM();
                                                var userAnswer = new UserAnswerVM();
                                                userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswerOpen.Text = string.Empty;
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answerFromImport);
                                                if (answerFromSurvey is null)
                                                {
                                                    resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answerFromImport} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                }
                                                else
                                                {
                                                    userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                    userAnswerOpen.UserAnswers.Add(userAnswer);
                                                    surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                }
                                            }
                                            else if (questionFromSurvey.IdQuestType == kvMultipleQuestion!.IdKeyValue)
                                            {
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromImportAsArr = answerFromImport.Split("; ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var answer in answerFromImportAsArr)
                                                {
                                                    var userAnswerOpen = new UserAnswerOpenVM();
                                                    var userAnswer = new UserAnswerVM();
                                                    userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswerOpen.Text = string.Empty;
                                                    var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answer);
                                                    if (answerFromSurvey is null)
                                                    {
                                                        resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answer} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                    }
                                                    else
                                                    {
                                                        userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                        userAnswerOpen.UserAnswers.Add(userAnswer);
                                                        surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                    }
                                                }
                                            }
                                            else if (questionFromSurvey.IdQuestType == kvSingleOpenQuestion!.IdKeyValue)
                                            {
                                                var userAnswerOpen = new UserAnswerOpenVM();
                                                var userAnswer = new UserAnswerVM();
                                                userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                userAnswerOpen.Text = !string.IsNullOrEmpty(worksheet.Rows[rowCounter - 1].Columns[i + 1].Value) ? worksheet.Rows[rowCounter - 1].Columns[i + 1].Value.Trim() : string.Empty;
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answerFromImport);
                                                if (answerFromSurvey is null)
                                                {
                                                    resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answerFromImport} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                }
                                                else
                                                {
                                                    userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                    userAnswerOpen.UserAnswers.Add(userAnswer);
                                                    surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                }
                                            }
                                            else
                                            {
                                                var answerFromImport = worksheet.Rows[rowCounter - 1].Columns[i].Value.Trim();
                                                var answerFromImportAsArr = answerFromImport.Split("; ", StringSplitOptions.RemoveEmptyEntries).ToArray();
                                                foreach (var answer in answerFromImportAsArr)
                                                {
                                                    var userAnswerOpen = new UserAnswerOpenVM();
                                                    var userAnswer = new UserAnswerVM();
                                                    userAnswerOpen.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswer.IdQuestion = questionFromSurvey.IdQuestion;
                                                    userAnswerOpen.Text = !string.IsNullOrEmpty(worksheet.Rows[rowCounter - 1].Columns[i + 1].Value) ? worksheet.Rows[rowCounter - 1].Columns[i + 1].Value.Trim() : string.Empty;
                                                    var answerFromSurvey = questionFromSurvey.Answers.FirstOrDefault(x => x.Text == answer);
                                                    if (answerFromSurvey is null)
                                                    {
                                                        resultContext.AddErrorMessage($"На ред {rowCounter} отговор {answer} не е намерен към въпрос {questionFromSurvey.Text}!");
                                                    }
                                                    else
                                                    {
                                                        userAnswer.IdAnswer = answerFromSurvey.IdAnswer;
                                                        userAnswerOpen.UserAnswers.Add(userAnswer);
                                                        surveyResult.UserAnswerOpens.Add(userAnswerOpen);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        resultContext.AddErrorMessage($"Въпрос {questionFromHeader} не е намерен в анкетата, за която се импортират резултати!");
                                    }
                                }

                                rowCounter++;

                                surveyResults.Add(surveyResult);
                            }
                        }
                    }
                }

                if (surveyResults.Any())
                {
                    resultContext.AddMessage("Импортът приключи успешно!");
                }
                else
                {
                    resultContext.AddErrorMessage("Не може да бъде добавен празен шаблон за резултати от анкета!");
                }

                resultContext.ResultContextObject = surveyResults;
            }
            catch (Exception ex)
            {
                resultContext.AddErrorMessage(ex.Message);
            }

            return resultContext;
        }

        public MemoryStream CreateSurveyResultsExcelWithErrors(ResultContext<List<SurveyResultVM>> resultContext)
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

        public async Task<ResultContext<SurveyResultVM>> CreateSurveyResultFromExcelImportAsync(ResultContext<SurveyResultVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var surveyResultForDb = model.To<SurveyResult>();
                surveyResultForDb.UserAnswerOpens = null;
                surveyResultForDb.CandidateProvider = null;
                surveyResultForDb.Survey = null;

                await this.repository.AddAsync<SurveyResult>(surveyResultForDb);
                await this.repository.SaveChangesAsync();

                model.IdSurveyResult = surveyResultForDb.IdSurveyResult;

                foreach (var userAnswerOpen in model.UserAnswerOpens)
                {
                    userAnswerOpen.IdSurveyResult = surveyResultForDb.IdSurveyResult;
                    var userAnswerOpenForDb = userAnswerOpen.To<UserAnswerOpen>();
                    userAnswerOpenForDb.UserAnswers = null;
                    userAnswerOpenForDb.SurveyResult = null;
                    userAnswerOpenForDb.Question = null;

                    await this.repository.AddAsync<UserAnswerOpen>(userAnswerOpenForDb);
                    await this.repository.SaveChangesAsync();

                    userAnswerOpen.IdUserAnswerOpen = userAnswerOpenForDb.IdUserAnswerOpen;

                    foreach (var userAnswer in userAnswerOpen.UserAnswers)
                    {
                        userAnswer.IdUserAnswerOpen = userAnswerOpen.IdUserAnswerOpen;
                        var userAnswerForDb = userAnswer.To<UserAnswer>();
                        userAnswerForDb.Answer = null;
                        userAnswerForDb.UserAnswerOpen = null;
                        userAnswerForDb.Question = null;

                        await this.repository.AddAsync<UserAnswer>(userAnswerForDb);
                        await this.repository.SaveChangesAsync();

                        userAnswer.IdUserAnswer = userAnswerForDb.IdUserAnswer;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return resultContext;
        }

        public async Task<SurveyResultVM> CreateSurveyResultForSelfAssessmentAsync(int IdCandidate_Provider, SurveyVM survey, int idSurveyResultStatus)
        {
            SurveyResult surveyResult = new SurveyResult()
            {
                IdStatus = idSurveyResultStatus,
                IdSurvey = survey.IdSurvey,
                IdCandidate_Provider = IdCandidate_Provider
            };

            await this.repository.AddAsync<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            var token = this.GetEncryptedTokenForSurveyResult(survey, surveyResult.IdSurveyResult);
            surveyResult.Token = token;
            this.repository.Update<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            return surveyResult.To<SurveyResultVM>();
        }

        public async Task<ResultContext<NoResult>> CopySurveyByIdSurveyAsync(int idSurvey, int? year)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {
                var surveyFromDb = await this.repository.AllReadonly<Survey>(x => x.IdSurvey == idSurvey)
                    .Include(x => x.Questions.OrderBy(y => y.Order))
                        .ThenInclude(x => x.Answers).AsNoTracking()
                            .FirstOrDefaultAsync();



                if (surveyFromDb is not null)
                {
                    var kvSurveyStatusTypeCreated = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyStatusType", "Created");
                    var kvSurveyTypeSelfAssessment = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyType", "SelfAssessment");

                    var copiedSurvey = new Survey()
                    {
                        Name = surveyFromDb.Name,
                        AdditionalText = surveyFromDb.AdditionalText,
                        IdSurveyТype = surveyFromDb.IdSurveyТype,
                        EndDate = surveyFromDb.EndDate,
                        IdSurveyTarget = surveyFromDb.IdSurveyTarget,
                        IdTrainingCourseType = surveyFromDb.IdTrainingCourseType,
                        InternalCode = surveyFromDb.InternalCode,
                        StartDate = surveyFromDb.StartDate,
                        TrainingPeriodFrom = surveyFromDb.TrainingPeriodFrom,
                        TrainingPeriodTo = surveyFromDb.TrainingPeriodTo,
                        IdSurveyStatus = kvSurveyStatusTypeCreated.IdKeyValue,
                        Excellent = surveyFromDb.Excellent,
                        Good = surveyFromDb.Good,
                        Satisfactory = surveyFromDb.Satisfactory,
                        Year = year
                    };

                    bool ifExist = await IsSurveyExist(copiedSurvey.To<SurveyVM>());

                    if (ifExist is true)
                    {
                        resultContext.AddErrorMessage("Не можете да създадете повече от един шаблон на доклад за самооценка в рамките на една и съща календарна година!");

                        return resultContext;
                    }
                    await this.repository.AddAsync<Survey>(copiedSurvey);
                    await this.repository.SaveChangesAsync();

                    resultContext.NewEntityId = copiedSurvey.IdSurvey;

                    foreach (var question in surveyFromDb.Questions)
                    {
                        var copiedQuestion = new Question()
                        {
                            IdSurvey = copiedSurvey.IdSurvey,
                            Text = question.Text,
                            IdQuestType = question.IdQuestType,
                            IdNext = question.IdNext,
                            IdPrev = question.IdPrev,
                            IsRequired = question.IsRequired,
                            AnswersCount = question.AnswersCount,
                            Order = question.Order,
                            Description = question.Description,
                            IdAreaSelfAssessment = question.IdAreaSelfAssessment,
                            IdRatingIndicatorType = question.IdRatingIndicatorType

                        };

                        await this.repository.AddAsync<Question>(copiedQuestion);
                        await this.repository.SaveChangesAsync();

                        foreach (var answer in question.Answers)
                        {
                            var copiedAnswer = new Answer()
                            {
                                IdQuestion = copiedQuestion.IdQuestion,
                                Text = answer.Text,
                                Points = answer.Points,
                            };

                            await this.repository.AddAsync<Answer>(copiedAnswer);
                            await this.repository.SaveChangesAsync();
                        }
                    }

                    if (surveyFromDb.IdSurveyТype == kvSurveyTypeSelfAssessment.IdKeyValue)
                    {
                        resultContext.AddMessage("Доклада за самооценка е копиран успешно!");
                    }
                    else {
                        resultContext.AddMessage("Анкетата е копирана успешно!");
                    }

                    
                }
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

        private async Task<IEnumerable<ClientCourseVM>> GetTaughtClientCoursesAsync(SurveyVM survey)
        {
            var coursesForPeriod = this.repository.AllReadonly<Course>(x => x.EndDate!.Value.Date >= survey.TrainingPeriodFrom!.Value.Date && x.EndDate!.Value.Date <= survey.TrainingPeriodTo!.Value.Date && x.IdTrainingCourseType == survey.IdTrainingCourseType!.Value);
            var coursesForPeriodAsVM = await coursesForPeriod.To<CourseVM>(x => x.ClientCourses).ToListAsync();
            var clientCourses = new List<ClientCourseVM>();
            foreach (var course in coursesForPeriodAsVM)
            {
                clientCourses.AddRange(course.ClientCourses.Where(x => !string.IsNullOrEmpty(x.EmailAddress) && x.IsContactAllowed));
            }

            if (coursesForPeriodAsVM.Any())
            {
                var course = coursesForPeriodAsVM.FirstOrDefault();
                if (course is not null)
                {
                    foreach (var client in clientCourses)
                    {
                        client.IdCandidateProvider = course.IdCandidateProvider;
                    }
                }
            }

            return clientCourses;
        }

        private async Task<SurveyResultVM> CreateSurveyResultForClientCourseAsync(ClientCourseVM client, SurveyVM survey, int idSurveyResultStatus)
        {
            SurveyResult surveyResult = new SurveyResult()
            {
                IdClientCourse = client.IdClientCourse,
                IdStatus = idSurveyResultStatus,
                IdSurvey = survey.IdSurvey,
                IdCandidate_Provider = client.IdCandidateProvider!.Value
            };

            await this.repository.AddAsync<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            var token = this.GetEncryptedTokenForSurveyResult(survey, surveyResult.IdSurveyResult);
            surveyResult.Token = token;
            this.repository.Update<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            return surveyResult.To<SurveyResultVM>();
        }

        private async Task<IEnumerable<ConsultingClientVM>> GetConsultedClientsAsync(SurveyVM survey)
        {
            var consultedClientsForPeriod = this.repository.AllReadonly<ConsultingClient>(x => x.EndDate!.Value.Date >= survey.StartDate!.Value.Date && x.EndDate!.Value.Date <= survey.EndDate!.Value.Date);

            return await consultedClientsForPeriod.To<ConsultingClientVM>().ToListAsync();
        }

        private async Task<SurveyResultVM> CreateSurveyResultForConsultingClientAsync(ConsultingClientVM client, SurveyVM survey, int idSurveyResultStatus)
        {
            SurveyResult surveyResult = new SurveyResult()
            {
                IdConsultingClient = client.IdConsultingClient,
                IdStatus = idSurveyResultStatus,
                IdSurvey = survey.IdSurvey,
                IdCandidate_Provider = client.IdCandidateProvider
            };

            await this.repository.AddAsync<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            var token = this.GetEncryptedTokenForSurveyResult(survey, surveyResult.IdSurveyResult);
            surveyResult.Token = token;
            this.repository.Update<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            return surveyResult.To<SurveyResultVM>();
        }

        private async Task<IEnumerable<ValidationClientVM>> GetValidatedClientsAsync(SurveyVM survey)
        {
            var validatedClientsForPeriod = this.repository.AllReadonly<ValidationClient>(x => x.EndDate!.Value.Date >= survey.StartDate!.Value.Date && x.EndDate!.Value.Date <= survey.EndDate!.Value.Date && x.IdCourseType == survey.IdTrainingCourseType!.Value);

            return await validatedClientsForPeriod.To<ValidationClientVM>().ToListAsync();
        }

        private async Task<SurveyResultVM> CreateSurveyResultForValidationClientAsync(ValidationClientVM client, SurveyVM survey, int idSurveyResultStatus)
        {
            SurveyResult surveyResult = new SurveyResult()
            {
                IdValidationClient = client.IdValidationClient,
                IdStatus = idSurveyResultStatus,
                IdSurvey = survey.IdSurvey,
                IdCandidate_Provider = client.IdCandidateProvider
            };

            await this.repository.AddAsync<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            var token = this.GetEncryptedTokenForSurveyResult(survey, surveyResult.IdSurveyResult);
            surveyResult.Token = token;
            this.repository.Update<SurveyResult>(surveyResult);
            await this.repository.SaveChangesAsync();

            return surveyResult.To<SurveyResultVM>();
        }

        private async Task UpdateSurveyStatusAsync(int idSurvey, int idStatus)
        {
            var surveyFromDb = await this.repository.AllReadonly<Survey>(x => x.IdSurvey == idSurvey).FirstOrDefaultAsync();
            if (surveyFromDb is not null)
            {
                surveyFromDb.IdSurveyStatus = idStatus;

                this.repository.Update<Survey>(surveyFromDb);
                await this.repository.SaveChangesAsync();
            }
        }

        private string GetEncryptedTokenForSurveyResult(SurveyVM survey, int entityId)
        {
            var encryptedIdValue = BaseHelper.Encrypt(entityId.ToString());
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("Identifier", encryptedIdValue) };
            var validMinutes = (int)((survey.EndDate!.Value - DateTime.Now).TotalMinutes);
            var token = BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams, validMinutes);

            return token;
        }

        private async Task SendEmailToClientAsync(ResultContext<SurveyResultVM> resultContext, string emailAddress)
        {
            await this.mailService.SendEmailForSurveyAsync(resultContext, emailAddress);
        }

        private ResultContext<SurveyResultVM> SetSurveyResultVMResultContext(SurveyResultVM surveyResultVM, SurveyVM survey)
        {
            var surveyResultResultContext = new ResultContext<SurveyResultVM>();
            surveyResultVM.EmailTemplateHeader = survey.EmailTemplateHeader;
            surveyResultVM.EmailTemplateText = survey.EmailTemplateText;
            surveyResultVM.SurveyEndDate = survey.EndDate;
            surveyResultResultContext.ResultContextObject = surveyResultVM;

            return surveyResultResultContext;
        }

        private async Task<int> FiledOutSurveyCountAsync(int idSurvey)
        {
            return (await this.repository.AllReadonly<SurveyResult>(x => x.IdSurvey == idSurvey && x.EndDate.HasValue).ToListAsync()).Count;
        }
        #endregion

        #region Question
        public async Task<int> GetNextQuestionOrderByIdSurveyAsync(int idSurvey)
        {
            var questions = await this.repository.AllReadonly<Question>(x => x.IdSurvey == idSurvey).ToListAsync();

            return questions.Count + 1;
        }

        public async Task<ResultContext<QuestionVM>> CreateQuestionAsync(ResultContext<QuestionVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var questionForDb = model.To<Question>();
                questionForDb.Answers = null;
                questionForDb.Survey = null;

                await this.repository.AddAsync<Question>(questionForDb);
                await this.repository.SaveChangesAsync();

                model.IdQuestion = questionForDb.IdQuestion;
                model.IdCreateUser = questionForDb.IdCreateUser;
                model.IdModifyUser = questionForDb.IdModifyUser;
                model.CreationDate = questionForDb.CreationDate;
                model.ModifyDate = questionForDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
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

        public async Task<ResultContext<QuestionVM>> UpdateQuestionAsync(ResultContext<QuestionVM> resultContext)
        {
            var model = resultContext.ResultContextObject;
            try
            {
                var questionFromDb = await this.repository.GetByIdAsync<Question>(model.IdQuestion);
                questionFromDb = model.To<Question>();
                questionFromDb.Answers = null;
                questionFromDb.Survey = null;

                this.repository.Update<Question>(questionFromDb);
                await this.repository.SaveChangesAsync();

                model.IdModifyUser = questionFromDb.IdModifyUser;
                model.ModifyDate = questionFromDb.ModifyDate;

                resultContext.AddMessage("Записът е успешен!");
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

        public async Task<QuestionVM> GetQuestionByIdAsync(int idQuestion)
        {
            var questions = this.repository.AllReadonly<Question>(x => x.IdQuestion == idQuestion);

            return await questions.To<QuestionVM>().FirstOrDefaultAsync();
        }

        public async Task<ResultContext<NoResult>> DeleteQuestionByIdAsync(int idQuestion)
        {
            var result = new ResultContext<NoResult>();
            try
            {
                var questionFromDb = await this.repository.GetByIdAsync<Question>(idQuestion);
                if (questionFromDb is not null)
                {
                    var answers = await this.repository.AllReadonly<Answer>(x => x.IdQuestion == questionFromDb.IdQuestion).ToListAsync();
                    if (answers.Any())
                    {
                        this.repository.HardDeleteRange<Answer>(answers);
                        await this.repository.SaveChangesAsync();
                    }

                    await this.repository.HardDeleteAsync<Question>(questionFromDb.IdQuestion);
                    await this.repository.SaveChangesAsync();

                    result.AddMessage("Записът е изтрит успешно!");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
                result.AddErrorMessage("Грешка при запис в базата данни!");
            }

            return result;
        }

        public async Task<IEnumerable<QuestionVM>> GetQuestionsByIdSurveyAsync(int idSurvey)
        {
            return await this.repository.AllReadonly<Question>(x => x.IdSurvey == idSurvey).To<QuestionVM>().OrderBy(x => x.Order).ToListAsync();
        }

        #endregion

        #region Answer
        public async Task<AnswerVM[]> GetAnswersByIdQuestionAsync(int idQuestion)
        {
            var answers = this.repository.AllReadonly<Answer>(x => x.IdQuestion == idQuestion);

            return await answers.To<AnswerVM>().OrderBy(x => x.IdAnswer).ToArrayAsync();
        }

        public async Task<bool> CreateAnswerAsync(AnswerVM answer)
        {
            try
            {
                var answerForDb = answer.To<Answer>();
                answerForDb.Question = null;

                await this.repository.AddAsync<Answer>(answerForDb);
                await this.repository.SaveChangesAsync();

                answer.IdAnswer = answerForDb.IdAnswer;
                answer.IdCreateUser = answerForDb.IdCreateUser;
                answer.CreationDate = answerForDb.CreationDate;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return false;
        }

        public async Task<bool> UpdateAnswerAsync(AnswerVM answer)
        {
            try
            {
                var answerFromDb = await this.repository.GetByIdAsync<Answer>(answer.IdAnswer);
                if (answerFromDb is not null)
                {
                    answerFromDb = answer.To<Answer>();
                    answerFromDb.Question = null;

                    this.repository.Update<Answer>(answerFromDb);
                    await this.repository.SaveChangesAsync();

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }

            return false;
        }
        #endregion

        #region Survey result
        public async Task<SurveyResultVM> GetSurveyResultByIdAsync(int idSurveyResult)
        {
            var surveyResult = this.repository.AllReadonly<SurveyResult>(x => x.IdSurveyResult == idSurveyResult);

            return await surveyResult.To<SurveyResultVM>(x => x.Survey.Questions.Select(y => y.Answers)).FirstOrDefaultAsync();
        }

        public async Task<SurveyResultVM> GetSurveyResultWithIncludesUserAnswerByIdAsync(int idSurveyResult)
        {
            var surveyResult = this.repository.AllReadonly<SurveyResult>(x => x.IdSurveyResult == idSurveyResult);

            return await surveyResult.To<SurveyResultVM>(
                x => x.Survey,
                x => x.Survey.Questions.Select(y => y.Answers),
                x => x.UserAnswerOpens,
                x => x.UserAnswerOpens.Select(y => y.UserAnswers)
              ).FirstOrDefaultAsync();
        }
        public async Task<SurveyResultVM> GetSurveyResultsWithIncludesUserAnswerByIdAsync(int? idSurveyResult, int idCandidateProvider)
        {           
            var newSurveyRes = this.repository.AllReadonly<SurveyResult>(x => x.IdSurveyResult == idSurveyResult && x.IdCandidate_Provider == idCandidateProvider);
            return await newSurveyRes.To<SurveyResultVM>(
                x => x.Survey,
                x => x.Survey.Questions.Select(y => y.Answers),
                x => x.UserAnswerOpens,
                x => x.UserAnswerOpens.Select(y => y.UserAnswers)
                ).FirstOrDefaultAsync();
        }
        public async Task SetSurveyResultStartDateAsync(int idSurveyResult)
        {
            var surveyResult = await this.repository.GetByIdAsync<SurveyResult>(idSurveyResult);
            if (surveyResult is not null)
            {
                surveyResult.StartDate = DateTime.Now;

                this.repository.Update<SurveyResult>(surveyResult);
                await this.repository.SaveChangesAsync();
            }
        }

        public async Task SetSurveyResultEndDateAndStatusToFiledAsync(SurveyResultVM surveyResultVM)
        {
            try
            {
                var surveyResult = await this.repository.GetByIdAsync<SurveyResult>(surveyResultVM.IdSurveyResult);
                if (surveyResult is not null)
                {
                    var kvFiledSurveyResultStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyResultStatusType", "Filled");
                    surveyResult.IdStatus = kvFiledSurveyResultStatus.IdKeyValue;
                    surveyResult.EndDate = DateTime.Now;

                    this.repository.Update<SurveyResult>(surveyResult);
                    await this.repository.SaveChangesAsync();

                    surveyResultVM.IdStatus = surveyResult.IdStatus;
                    surveyResultVM.EndDate = surveyResult.EndDate;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message);
                _logger.LogError(ex.StackTrace);
            }
        }
        #endregion

        #region User answer
        public async Task<ResultContext<NoResult>> CreateUserAnswersAsync(List<UserAnswerModel> userAnswerModels, SurveyResultVM surveyResult)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {



                var userAnswersOpen = await this.repository.AllReadonly<UserAnswerOpen>(x => x.IdSurveyResult == surveyResult.IdSurveyResult).ToListAsync();



                if (userAnswersOpen.Any())
                {

                    foreach (var userAnswerOpen in userAnswersOpen)
                    {
                        var userAnswers = await this.repository.AllReadonly<UserAnswer>(x => x.IdUserAnswerOpen == userAnswerOpen.IdUserAnswerOpen).ToListAsync();
                        this.repository.HardDeleteRange<UserAnswer>(userAnswers);
                        await this.repository.SaveChangesAsync();
                    }

                    this.repository.HardDeleteRange<UserAnswerOpen>(userAnswersOpen);
                    await this.repository.SaveChangesAsync();
                }




                foreach (var userAnswerModel in userAnswerModels)
                {
                    var idUserAnswerOpen = await this.CreateUserAnswerOpenAsync(userAnswerModel.IdQuestion, surveyResult.IdSurveyResult, userAnswerModel.OpenAnswerText);

                    foreach (var idAnswer in userAnswerModel.AnswerIds)
                    {
                        await this.CreateUserAnswerAsync(idUserAnswerOpen, userAnswerModel.IdQuestion, idAnswer);
                    }

                    if (!userAnswerModel.AnswerIds.Any())
                    {
                        await this.CreateUserAnswerAsync(idUserAnswerOpen, userAnswerModel.IdQuestion, null);
                    }
                }

                resultContext.AddMessage("Анкетата е изпратена успешно към НАПОО!");
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

        private async Task<int> CreateUserAnswerOpenAsync(int idQuestion, int idSurveyResult, string text)
        {
            UserAnswerOpen userAnswerOpen = new UserAnswerOpen()
            {
                IdSurveyResult = idSurveyResult,
                IdQuestion = idQuestion,
                Text = text
            };

            await this.repository.AddAsync<UserAnswerOpen>(userAnswerOpen);
            await this.repository.SaveChangesAsync();

            return userAnswerOpen.IdUserAnswerOpen;
        }

        private async Task CreateUserAnswerAsync(int idUserAnswerOpen, int idQuestion, int? idAnswer)
        {
            Answer answer = null;

            if (idAnswer.HasValue)
            {
                answer = await this.repository.GetByIdAsync<Answer>(idAnswer.Value);
            }



            UserAnswer userAnswer = new UserAnswer()
            {
                IdUserAnswerOpen = idUserAnswerOpen,
                IdQuestion = idQuestion,
                IdAnswer = idAnswer,
                Points = (answer != null ? answer.Points : null)

            };

            await this.repository.AddAsync<UserAnswer>(userAnswer);
            await this.repository.SaveChangesAsync();
        }

        private async Task<IEnumerable<UserAnswerVM>> GetUserAnswersByListIdsUserAnswerOpenAsync(List<int> ids)
        {
            var userAnswer = this.repository.AllReadonly<UserAnswer>(x => ids.Contains(x.IdUserAnswerOpen));

            return await userAnswer.To<UserAnswerVM>(x => x.Answer, x => x.UserAnswerOpen).ToListAsync();
        }

        private async Task<IEnumerable<UserAnswerOpenVM>> GetUserAnswerOpenListByIdQuestionAndIdSurveyResultAsync(int idQuestion, int idSurveyResult)
        {
            var userAnswer = this.repository.AllReadonly<UserAnswerOpen>(x => x.IdQuestion == idQuestion && x.IdSurveyResult == idSurveyResult);

            return await userAnswer.To<UserAnswerOpenVM>().ToListAsync();
        }
        #endregion

        #region SelfAssessment
        public async Task<IEnumerable<SurveyVM>> GetAllSelfAssessmentSurveyAsync(string surveyTarget)
        {

            var kvSurveyTarget = await this.dataSourceService.GetKeyValueByIntCodeAsync("SurveyTarget", surveyTarget);

            var kvSurveyTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("SurveyType");
            var kvSelfAssessment = kvSurveyTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "SelfAssessment");

            var surveys = this.repository.AllReadonly<Survey>().Where(s => s.IdSurveyТype == kvSelfAssessment.IdKeyValue && s.IdSurveyTarget == kvSurveyTarget.IdKeyValue);
            var surveysAsVM = surveys.To<SurveyVM>().ToList();


            var kvFrameworkProgramSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).Where(x => x.DefaultValue3 != null).ToList();
            foreach (var survey in surveysAsVM)
            {
                var type = kvSurveyTypeSource.FirstOrDefault(x => x.IdKeyValue == survey.IdSurveyТype);
                if (type is not null)
                {
                    survey.SurveyTypeValue = type.Name;
                }

                var courseType = kvFrameworkProgramSource.FirstOrDefault(x => x.IdKeyValue == survey.IdTrainingCourseType);
                if (courseType is not null)
                {
                    survey.TrainingCourseTypeValue = courseType.Name;
                }
            }

            return surveysAsVM.OrderBy(x => x.Name).ToList();
        }


        public async Task<ResultContext<NoResult>> CreateUserAnswersSelfAssessmentAsync(List<UserAnswerModel> userAnswerModels, SurveyResultVM surveyResult)
        {
            var resultContext = new ResultContext<NoResult>();
            try
            {



                var userAnswersOpen = await this.repository.AllReadonly<UserAnswerOpen>(x => x.IdSurveyResult == surveyResult.IdSurveyResult).ToListAsync();



                if (userAnswersOpen.Any())
                {

                    foreach (var userAnswerOpen in userAnswersOpen)
                    {
                        var userAnswers = await this.repository.AllReadonly<UserAnswer>(x => x.IdUserAnswerOpen == userAnswerOpen.IdUserAnswerOpen).ToListAsync();
                        this.repository.HardDeleteRange<UserAnswer>(userAnswers);
                        await this.repository.SaveChangesAsync();
                    }

                    this.repository.HardDeleteRange<UserAnswerOpen>(userAnswersOpen);
                    await this.repository.SaveChangesAsync();
                }




                foreach (var userAnswerModel in userAnswerModels)
                {
                    var idUserAnswerOpen = await this.CreateUserAnswerOpenAsync(userAnswerModel.IdQuestion, surveyResult.IdSurveyResult, userAnswerModel.OpenAnswerText);

                    //След маркирване запис и отмаркирване запис AnswerIds става NULL
                    if (userAnswerModel.AnswerIds != null) 
                    {
                        foreach (var idAnswer in userAnswerModel.AnswerIds)
                        {
                            await this.CreateUserAnswerAsync(idUserAnswerOpen, userAnswerModel.IdQuestion, idAnswer);
                        }

                        if (!userAnswerModel.AnswerIds.Any())
                        {
                            await this.CreateUserAnswerAsync(idUserAnswerOpen, userAnswerModel.IdQuestion, null);
                        }
                    }
                    
                }

                //resultContext.AddMessage("Анкетата е изпратена успешно към НАПОО!");
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

        public async Task<bool> CheckIsSelfAssessmentReportExist(int idCandidateProvider, int year)
        {

            var selfAssessmentReports = await this.repository.AllReadonly<SelfAssessmentReport>(
                x => x.IdCandidateProvider == idCandidateProvider &&
                     x.Year == year
                ).ToListAsync();

            if (selfAssessmentReports.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }


        }
        #endregion
    }
}
