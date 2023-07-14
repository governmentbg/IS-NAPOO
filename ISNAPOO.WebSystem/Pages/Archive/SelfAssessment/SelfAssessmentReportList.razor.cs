using System.Drawing.Text;
using Data.Models.Data.Assessment;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Services.Assessment;
using ISNAPOO.Core.Services.Candidate;
using ISNAPOO.Core.Services.Rating;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentReportList : BlazorBaseComponent
    {
        private SelfAssessmentReportListFilterVM selfAssReportListFilterVM = new SelfAssessmentReportListFilterVM();
        private SfGrid<SelfAssessmentReportVM> reportsGrid = new SfGrid<SelfAssessmentReportVM>();
        private SelfAssessmentReportModal selfAssessmentReportModal = new SelfAssessmentReportModal();
        private SelfAssessmentApproveRejectModal selfAssessmentApproveRejectModal = new SelfAssessmentApproveRejectModal();
        private SelfAssessmentReportFilterListModal selfAssessmentReportFilterListModal = new SelfAssessmentReportFilterListModal();
        private List<SelfAssessmentSummaryProfessionalTrainingVM> summarySource = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
        private List<SelfAssessmentSummaryProfessionalTrainingVM> newSummSource = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
        private List<List<SelfAssessmentSummaryProfessionalTrainingVM>> summSourceVM = new List<List<SelfAssessmentSummaryProfessionalTrainingVM>>();
        private List<UserAnswerModel> userAnswerModels = new List<UserAnswerModel>();
        private List<List<UserAnswerModel>> userAnswersListModels = new List<List<UserAnswerModel>>();
        private IEnumerable<SelfAssessmentReportVM> reportsSource = new List<SelfAssessmentReportVM>();
        private SelfAssessmentReportVM selfAssessmentReportVM = new SelfAssessmentReportVM();
        private List<SelfAssessmentReportVM> selectedRows = new List<SelfAssessmentReportVM>();
        private List<SelfAssessmentReportVM> justSubmitetReports = new List<SelfAssessmentReportVM>();
        private KeyValueVM keyValueSubmited = new KeyValueVM();
        private string cpoCipoHeaderTxt = string.Empty;
        private Decimal TotalPoint = 0;
        private Decimal MaxPoint = 0;
        public string LicensingType { get; set; }
        private bool isNapoo = false;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }


        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IRatingService ratingService { get; set; }

        bool showButtonCreateAssessmentReport { get; set; } = false;

       // private bool isVisibleCreateAssessmentReport { get; set; } = false;
        protected override async Task OnInitializedAsync()
        {
            if (this.UserProps.IdCandidateProvider != 0) 
            {
                showButtonCreateAssessmentReport = true;
            }
            this.LicensingType = string.Empty;
            this.keyValueSubmited = await this.DataSourceService.GetKeyValueByIntCodeAsync("SelfAssessmentReportStatus", "Submitted");
            await this.LoadReportsDataAsync(null, "FromOnInitialized");

        }

        private async Task LoadReportsDataAsync(SelfAssessmentReportListFilterVM selfAssessmentReportListFilter, string fromWhere)
        {
            if (this.UserProps.IdCandidateProvider != 0)
            {
                this.reportsSource = await this.ArchiveService.GetAllSelfAssessmentReportsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, selfAssessmentReportListFilter, fromWhere);
                this.reportsSource = await this.FindStatusComment(this.reportsSource);
                this.isNapoo = false;
                var kvLicenceTypeCPO = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
                
                CandidateProviderVM candidateProvider = null;
                if (!this.reportsSource.Any())
                {
                    candidateProvider = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);
                }
                else
                {
                    candidateProvider = this.reportsSource.FirstOrDefault()!.CandidateProvider;
                }

                this.cpoCipoHeaderTxt = candidateProvider.IdTypeLicense == kvLicenceTypeCPO!.IdKeyValue
                    ? "ЦПО"
                    : "ЦИПО";
            }
            else
            {
                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                this.isNapoo = true;
                tokenContext.ResultContextObject.Token = Token;
                tokenContext = BaseHelper.GetDecodeToken(tokenContext);
                this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "SelfAssessmentReportTypeLicense").Value.ToString();


                if (this.LicensingType == "LicensingCPO" || this.LicensingType == "LicensingCIPO")
                {
                    var data = await this.ArchiveService.GetAllSelfAssessmentReports(this.LicensingType, selfAssessmentReportListFilter, fromWhere);

                    data = data.Where(x => x.StatusIntCode != "Created").ToList();

                    this.reportsSource = data;
                    this.reportsSource = await this.FindStatusComment(this.reportsSource);
                }

                this.cpoCipoHeaderTxt = this.LicensingType == "LicensingCPO"
                    ? "ЦПО"
                    : "ЦИПО";
            }
            //foreach (var item in this.reportsSource)
            //{
            //    item.CandidateProvider.MixCPOandCIPONameOwner = item.CandidateProvider.CPONameOwnerGrid;
            //}
              this.reportsSource = this.reportsSource.OrderByDescending(x => x.FilingDate);
        }

        private async Task<IEnumerable<SelfAssessmentReportVM>> FindStatusComment(IEnumerable<SelfAssessmentReportVM> reportsSource)
        {
            foreach (var report in this.reportsSource)
            {
                if (report.SelfAssessmentReportStatuses.Any())
                {
                    var selfAssementStatus = report.SelfAssessmentReportStatuses.OrderByDescending(x => x.CreationDate).First();
                    report.CommentSelfAssessmentReportStatus = selfAssementStatus.Comment is not null ? selfAssementStatus.Comment : string.Empty;
                }
                else
                {
                    report.CommentSelfAssessmentReportStatus = string.Empty;
                }

            }
            return reportsSource;
        } 

        private async Task AddReportBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;             

                CandidateProviderVM candidateProviderVM = await this.CandidateProviderService.GetOnlyCandidateProviderByIdAsync(this.UserProps.IdCandidateProvider);


                this.LicensingType = (await this.DataSourceService.GetKeyValueByIdAsync(candidateProviderVM.IdTypeLicense)).KeyValueIntCode;

                SurveyResultVM surveyResult = new SurveyResultVM();

                surveyResult.Survey = await this.AssessmentService.GetSurveySelfAssessmentByYear(DateTime.Now.Year, this.LicensingType);


                await this.selfAssessmentReportModal.OpenModal(new SelfAssessmentReportVM() { IdCandidateProvider = this.UserProps.IdCandidateProvider, SurveyResult = surveyResult });
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditReportBtn(SelfAssessmentReportVM selfAssessmentReport)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(selfAssessmentReport.IdSelfAssessmentReport, "SelfAssessmentReport");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(selfAssessmentReport.IdSelfAssessmentReport, "SelfAssessmentReport");
                }

                var report = await this.ArchiveService.GetSelfAssessmentReportByIdAsync(selfAssessmentReport.IdSelfAssessmentReport);

                await this.selfAssessmentReportModal.OpenModal(report, concurrencyInfoValue);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterModalSubmit()
        {
            await this.LoadReportsDataAsync(this.selfAssReportListFilterVM, "FromUpdateAfterModalSubmit");
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();

                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"SelfAssessmentReportList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.reportsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"SelfAssessmentReportList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.reportsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();
            ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderOwner", HeaderText = this.cpoCipoHeaderTxt, TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Year", HeaderText = "Година", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FilingDateAsStr", HeaderText = "Дата на подаване", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Status", HeaderText = "Статус", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CommentSelfAssessmentReportStatus", HeaderText = "Коментар", TextAlign = TextAlign.Left });
            return ExportColumns;                                 
        }


        public async Task CreateAssessmentReport()
        {
            bool IsSelfAssessmentReportExist = await this.AssessmentService.CheckIsSelfAssessmentReportExist(this.UserProps.IdCandidateProvider, DateTime.Now.Year);

            if (IsSelfAssessmentReportExist)
            {
                await this.ShowErrorAsync($"Вече има създаден доклад за самооценка за {DateTime.Now.Year}");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да създадете отчета за самооценка?");
            if(isConfirmed)
            {
                await this.AddReportBtn();
            }
                this.StateHasChanged();
        }

        private async Task RowDeselectedHandler(RowDeselectEventArgs<SelfAssessmentReportVM> args)
        {
            this.selectedRows.Clear();
            this.selectedRows = await this.reportsGrid.GetSelectedRecordsAsync();
        }

        private async Task RowSelectedHandler(RowSelectEventArgs<SelfAssessmentReportVM> args)
        {
            this.selectedRows.Clear();
            this.selectedRows = await this.reportsGrid.GetSelectedRecordsAsync();
        }

        private async Task OpenRejectSelfAssModal()
        {
            
            if (!this.selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред/редове!");
                return;
            }

            this.justSubmitetReports = new List<SelfAssessmentReportVM>();       
            foreach (var report in this.selectedRows)
            {
                if (report.IdStatus != keyValueSubmited.IdKeyValue)
                {
                    this.justSubmitetReports.Add(report);
                }
            }
            if (this.justSubmitetReports.Any())
            {
                await this.ShowErrorAsync("Моля, изберете доклади със статус Подаден!");
                return;
            }

            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                this.loading = true;
                await this.selfAssessmentApproveRejectModal.OpenModal(null, "Reject", this.selectedRows);

            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task OpenApproveSelfAssModal()
        {
            if (!this.selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете ред/редове!");
                return;
            }

            this.justSubmitetReports = new List<SelfAssessmentReportVM>();
            foreach (var report in this.selectedRows) 
            {
                if (report.IdStatus != keyValueSubmited.IdKeyValue)
                {
                    justSubmitetReports.Add(report);
                }
            }
            if (this.justSubmitetReports.Any())
            {
                await this.ShowErrorAsync("Моля, изберете доклади със статус Подаден!");
                return;
            }
            if (this.loading)
            {
                this.SpinnerHide();
                return;
            }
            try
            {
                await this.selfAssessmentApproveRejectModal.OpenModal(null, "Approve", this.selectedRows);
            }
            finally
            {
                this.loading = false;
            }
        }
        private async Task OpenFilterBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                DateTime lastYear = DateTime.Now.AddYears(-1);
                this.selfAssessmentReportFilterListModal.OpenModal(lastYear.Year);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task OnFilterModalSubmit(SelfAssessmentReportListFilterVM selfAssessmentReportListFilterVM)
        {
            selfAssReportListFilterVM = selfAssessmentReportListFilterVM;
            await this.LoadReportsDataAsync(selfAssessmentReportListFilterVM, "FromFilterModalSubmit");
            StateHasChanged();
        }
        private async Task GenerateSelfAssessmentsReport()
        {
            
            if (!selectedRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред, за да генерирате справка!");
                return;
            }

            if (selectedRows.Count > 1)
            {
                int firstYear = selectedRows.Select(x => x.Year).First();
                
                foreach (var row in selectedRows)
                {
                    if (firstYear != row.Year)
                    {
                        await this.ShowErrorAsync("Моля, изберете доклади за самооценка, за една и съща година!");
                        return;
                    }
                }
            }
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                summSourceVM = new List<List<SelfAssessmentSummaryProfessionalTrainingVM>>();
                userAnswersListModels = new List<List<UserAnswerModel>>();
                var selectedAssessements = selectedRows;

                List<SurveyResultVM> listSurveyResultVM = new List<SurveyResultVM>();
                var surveyRes = new SurveyResultVM();
                foreach (var item in selectedAssessements)
                {
                    newSummSource = new List<SelfAssessmentSummaryProfessionalTrainingVM>();
                    if (this.cpoCipoHeaderTxt == "ЦПО")
                    {
                        newSummSource = await (LoadSummarySourceCPOAsync(item.IdCandidateProvider, item.Year));
                    }
                    else if(this.cpoCipoHeaderTxt == "ЦИПО")
                    {
                        newSummSource = await (LoadSummarySourceCIPOAsync(item.IdCandidateProvider, item.Year));
                    }
                    
                    this.summSourceVM.Add(newSummSource.ToList());

                    surveyRes = new SurveyResultVM();
                    surveyRes = await this.AssessmentService.GetSurveyResultsWithIncludesUserAnswerByIdAsync(item.IdSurveyResult, item.IdCandidateProvider);
                    if (surveyRes != null)
                    {

                        foreach (var q in surveyRes.Survey.Questions)
                        {
                            q.AreaSelfAssessment = await this.DataSourceService.GetKeyValueByIdAsync(q.IdAreaSelfAssessment);
                        }
                        listSurveyResultVM.Add(surveyRes);
                    }
                }

                if (listSurveyResultVM.Count > 0)
                {
                    foreach (var item in listSurveyResultVM)
                    {
                        var userAnswers = await GetUserAnswerPointsModelSource(item);
                        this.userAnswersListModels.Add(userAnswers);
                    }
                    var detailedReport = await this.AssessmentService.CreateExcelReportWithSurveyResultsAsync(selectedAssessements, listSurveyResultVM, this.cpoCipoHeaderTxt, this.summSourceVM, this.userAnswersListModels);
                    await this.JsRuntime.SaveAs($"Spravka_za_dokladi_za_samoocenka_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", detailedReport.ToArray());
                }
                else
                {
                    await this.ShowErrorAsync("Няма информация за справката!");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task<List<UserAnswerModel>> GetUserAnswerPointsModelSource(SurveyResultVM surveyResultVM)
        {
            bool hasPoints = true;
            this.userAnswerModels = new List<UserAnswerModel>();
            foreach (var areaSelfAssessment in surveyResultVM.Survey.Questions.GroupBy(x => x.AreaSelfAssessment).OrderBy(x => x.Key.Order))
            {
                foreach (var question in surveyResultVM.Survey.Questions.Where(q => q.IdAreaSelfAssessment == areaSelfAssessment.Key.IdKeyValue))
                {

                    if (question.Answers.Any(x => x.Points.HasValue))
                    {

                        decimal maxPoint = question.Answers.Max(x => x.Points).Value;
                        this.MaxPoint += maxPoint;
                        hasPoints = true;
                    }
                    else
                    {
                        hasPoints = false;
                    }


                    string OpenAnswerText = string.Empty;
                    var userAnswerOpens = surveyResultVM
                                     .UserAnswerOpens
                                     .Where(x => x.IdQuestion == question.IdQuestion);

                    List<int> answerIds = new List<int>();

                    Decimal points = decimal.Zero;

                    if (userAnswerOpens.Count() > 0)
                    {
                        answerIds = userAnswerOpens.SelectMany(x => x.UserAnswers.Where(y => y.IdAnswer.HasValue).Select(y => y.IdAnswer.Value)).ToList();

                        OpenAnswerText = userAnswerOpens.FirstOrDefault().Text;

                        points = (Decimal)userAnswerOpens.Sum(x => x.UserAnswers.Sum(x => x.Points));


                        this.TotalPoint += points;

                    }

                    var userAnswerModel = new UserAnswerModel()
                    {
                        IdQuestion = question.IdQuestion,
                        AnswerIds = answerIds,
                        OpenAnswerText = OpenAnswerText,
                        Points = points,
                        HasPoints = hasPoints
                    };

                    this.userAnswerModels.Add(userAnswerModel);
                }
            }
            return this.userAnswerModels;
        }

        private async Task<List<SelfAssessmentSummaryProfessionalTrainingVM>> LoadSummarySourceCPOAsync(int idCandidateProvider, int year)
        {

            summarySource.Clear();
            var count = await this.ratingService.CountDegreeProfessionalQualificationAsync(idCandidateProvider, year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 1,
                ProfessionalTrainingIndicatorName = "придобили степен на професионална квалификация",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountProfessionalQualificationPartProfession(idCandidateProvider, year);

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 2,
                ProfessionalTrainingIndicatorName = "придобили професионална квалификация по част от професията",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountDegreeProfessionalQualificationValidation(idCandidateProvider, year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 3,
                ProfessionalTrainingIndicatorName = "придобили степен на професионална квалификация чрез валидиране",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountProfessionalQualificationPartProfessionValidation(idCandidateProvider, year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 4,
                ProfessionalTrainingIndicatorName = "придобили професионална квалификация по част от професията чрез валидиране",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });


            ///TODO: Да се смята стойността на "получили правоспособност"
            count = await this.ratingService.CountCourseCompetenceUnderRegulationsOneAndSeven(idCandidateProvider, year);

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 5,
                ProfessionalTrainingIndicatorName = "получили правоспособност",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = count
            });



            count = await this.ratingService.CountProfessionalEducationFollowingNextYear(idCandidateProvider, year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {

                ProfessionalTrainingIndicatorId = 5,
                ProfessionalTrainingIndicatorName = "продължаващи професионално обучение през следващата година",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            count = await this.ratingService.CountNonSuccessfulVocationalTraining(idCandidateProvider, year);

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 6,
                ProfessionalTrainingIndicatorName = "незавършили успешно професионалното обучение:",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });


            count = await this.ratingService.CountAbandonedTrainingStarted(idCandidateProvider, year);


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 7,
                ProfessionalTrainingIndicatorName = "отказали се от започнатото професионално обучение:",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });
            return summarySource;
        }

        private async Task<List<SelfAssessmentSummaryProfessionalTrainingVM>> LoadSummarySourceCIPOAsync(int idCandidateProvider, int year)
        {

            summarySource.Clear();

            var count = 0;


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 1,
                ProfessionalTrainingIndicatorName = "Брой клиенти с националност България",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = 7
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 2,
                ProfessionalTrainingIndicatorName = "Брой клиенти с националност страна-членка от Европейския съюз",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = 2
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 3,
                ProfessionalTrainingIndicatorName = "Брой клиенти с националност трета страна",
                ProfessionalTrainingIndicatorGroup = "Group1",
                ProfessionalTrainingIndicatorCount = 1
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 1,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Информиране и самоинформиране“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 1
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 2,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Кариерно консултиране“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 3
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 3,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Оценка на случай“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 2
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 4,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Активиране и мотивиране“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 5,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Психологическо подпомагане“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 2
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 6,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Застъпничество“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 4
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 7,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Групи за взаимопомощ“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 8,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Управление на таланти“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = count
            });

            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 9,
                ProfessionalTrainingIndicatorName = "Брой предоставяния на услугата „Менторство“",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 1
            });


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 10,
                ProfessionalTrainingIndicatorName = "Средна възраст",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 42
            });


            summarySource.Add(new SelfAssessmentSummaryProfessionalTrainingVM()
            {
                ProfessionalTrainingIndicatorId = 11,
                ProfessionalTrainingIndicatorName = "Средна стойност на услугата в български лева",
                ProfessionalTrainingIndicatorGroup = "Group2",
                ProfessionalTrainingIndicatorCount = 642
            });
            return summarySource;
        }
    }
}
