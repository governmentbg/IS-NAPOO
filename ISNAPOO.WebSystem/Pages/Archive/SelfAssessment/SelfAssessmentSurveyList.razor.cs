using Data.Models.Data.Assessment;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.SPPOO;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive.SelfAssessment
{
    public partial class SelfAssessmentSurveyList : BlazorBaseComponent
    {
        private SfGrid<SurveyVM> surveyGrid = new SfGrid<SurveyVM>();
        private SelfAssessmentSurveyModal surveyModal = new SelfAssessmentSurveyModal();

        private IEnumerable<SurveyVM> surveySource = new List<SurveyVM>();
        private bool showDeleteConfirmDialog = false;
        private SurveyVM surveyToDelete = new SurveyVM();

        private string surveyTarget { get; set; }
        private string SurveyTargetName { get; set; }



        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) { await this.LoadSurveyDataAsync(); }
        }

        private async Task LoadSurveyDataAsync()
        {
            if (Token != null)
            {
                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                tokenContext.ResultContextObject.Token = Token;
                tokenContext = BaseHelper.GetDecodeToken(tokenContext);
                this.surveyTarget = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "SelfAssessmentSurveyTarget").Value.ToString();


                this.surveySource = await this.AssessmentService.GetAllSelfAssessmentSurveyAsync(this.surveyTarget);


                if (this.surveyTarget == "ForCPO") {
                    this.SurveyTargetName = "ЦПО";
                }
                else if (this.surveyTarget == "ForCIPO")
                {
                    this.SurveyTargetName = "ЦИПО";
                }

                this.StateHasChanged();
            }
            
            
        }

        private async Task AddSurveyBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var kvStudentTarget = await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyTarget", this.surveyTarget);
                var kvSurveyType = await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyType", "SelfAssessment");


                
                DateTime now = DateTime.Now;

                var model = new SurveyVM()
                {
                    IdSurveyTarget = kvStudentTarget.IdKeyValue,
                    IdSurveyТype = kvSurveyType.IdKeyValue,
                    TrainingPeriodFrom = new DateTime(now.Year, 1, 1),
                    TrainingPeriodTo = new DateTime(now.Year, 12, 31),
                    StartDate = new DateTime(now.Year, 1, 1),
                    EndDate = new DateTime(now.Year, 12, 31),
                    Year = now.Year,
                    Excellent = 91,
                    Good = 66,
                    Satisfactory = 45

                };

                await this.surveyModal.OpenModal(model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditSurveyBtn(SurveyVM survey)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var surveyFromDb = await this.AssessmentService.GetSurveyByIdAsync(survey.IdSurvey);
                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(survey.IdSurvey, "Survey");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(survey.IdSurvey, "Survey");
                }

                await this.surveyModal.OpenModal(surveyFromDb, concurrencyInfoValue);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        
        public async void ConfirmDeleteCallback()
        {

            this.showDeleteConfirmDialog = false;

            var result = await this.AssessmentService.DeleteSurveyByIdAsync(surveyToDelete.IdSurvey);
            if (result.HasErrorMessages)
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
            }
            else
            {
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                await this.LoadSurveyDataAsync();
            }
            this.StateHasChanged();
        }
        private async Task DeleteSurveyBtn(SurveyVM survey)
        {
            this.showDeleteConfirmDialog = !this.showDeleteConfirmDialog;
            this.surveyToDelete = survey;
            this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
           
        }

        private async Task UpdateAfterSurveyModalSubmitAsync()
        {
            await this.LoadSurveyDataAsync();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
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
                ExportProperties.FileName = $"SurveyList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.surveyGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"SurveyList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.surveyGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Наименование", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SurveyTypeValue", HeaderText = "Тип на анкетата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingPeriodFromAsStr", HeaderText = "Период на обучение от", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingPeriodToAsStr", HeaderText = "Период на обучение до", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TrainingCourseTypeValue", HeaderText = "Вид на курса", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "StartDateAsStr", HeaderText = "Дата на активност от", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "EndDateAsStr", HeaderText = "Дата на активност до", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
