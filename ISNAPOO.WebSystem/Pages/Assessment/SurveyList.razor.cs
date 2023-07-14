using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Assessment;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Assessment;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Assessment
{
    public partial class SurveyList : BlazorBaseComponent
    {
        private SfGrid<SurveyVM> surveyGrid = new SfGrid<SurveyVM>();
        private SurveyModal surveyModal = new SurveyModal();

        private IEnumerable<SurveyVM> surveySource = new List<SurveyVM>();
        private int idSurveyType;
        private string title = String.Empty;
        private bool isNAPOOEntry = true;
        private ImportSurveyModal importSurveyModal = new ImportSurveyModal();

        [Inject]
        public IAssessmentService AssessmentService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await this.HandleTokenData();
        }

        private async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    var type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();
                    this.SetTitle(type!);
                    if (!string.IsNullOrEmpty(type))
                    {
                        if (type == GlobalConstants.TOKEN_SURVEYLIST_GRADUATES_REALIZATION || type == GlobalConstants.TOKEN_SURVEYLIST_MEASUREMENT_SATISFACTION)
                        {
                            this.idSurveyType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyType", type)).IdKeyValue;

                            await this.LoadSurveyDataForNAPOOAsync();
                        }
                        else
                        {
                            this.isNAPOOEntry = false;

                            var typeFromCPOEntry = type.Substring(3, type.Length - 3);
                            this.idSurveyType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyType", typeFromCPOEntry)).IdKeyValue;

                            await this.LoadSurveyDataForCPOAsync();
                        }
                    }
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private void SetTitle(string type)
        {
            this.title = (type == GlobalConstants.TOKEN_SURVEYLIST_GRADUATES_REALIZATION || type == GlobalConstants.TOKEN_CPO_SURVEYLIST_GRADUATES_REALIZATION) ? "Анкети за проследяване реализацията на завършилите ПО" : "Aнкети за измерване на степента на удовлетвореност";
        }

        private async Task LoadSurveyDataForNAPOOAsync()
        {
            this.surveySource = await this.AssessmentService.GetAllSurveysByIdSurveyTypeAsync(this.idSurveyType);
            this.StateHasChanged();
        }

        private async Task LoadSurveyDataForCPOAsync()
        {
            this.surveySource = await this.AssessmentService.GetAllSurveysByIdSurveyTypeAndIdCandidateProviderAsync(this.idSurveyType, this.UserProps.IdCandidateProvider);
            this.StateHasChanged();
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

                var kvStudentTarget = await this.DataSourceService.GetKeyValueByIntCodeAsync("SurveyTarget", "ForClients");
                var model = new SurveyVM()
                {
                    IdSurveyTarget = kvStudentTarget.IdKeyValue,
                    IdSurveyТype = this.idSurveyType
                };

                await this.surveyModal.OpenModal(model, this.isNAPOOEntry);
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
                if (this.isNAPOOEntry)
                {
                    var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(survey.IdSurvey, "Survey");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(survey.IdSurvey, "Survey");
                    }

                    await this.surveyModal.OpenModal(surveyFromDb, this.isNAPOOEntry, concurrencyInfoValue);
                }
                else
                {
                    await this.surveyModal.OpenModal(surveyFromDb, this.isNAPOOEntry);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteSurveyBtn(SurveyVM survey)
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (confirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var result = await this.AssessmentService.DeleteSurveyByIdAsync(survey.IdSurvey);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                        await this.LoadSurveyDataForNAPOOAsync();
                    }
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task UpdateAfterSurveyModalSubmitAsync()
        {
            await this.LoadSurveyDataForNAPOOAsync();
        }

        private async Task OnReportBtnSelectHandler(MenuEventArgs args)
        {
            var selectedSurveyRows = await this.surveyGrid.GetSelectedRecordsAsync();
            if (!selectedSurveyRows.Any())
            {
                await this.ShowErrorAsync("Моля, изберете анкета, за която да генерирате справка!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedSurvey = selectedSurveyRows.FirstOrDefault();
                var btnText = args.Item.Text;
                var resultContext = new ResultContext<List<SurveyResultVM>>();
                switch (btnText)
                {
                    case ("Обобщена справка за получените резулатати"):
                        resultContext.ResultContextObject = await this.AssessmentService.GetSurveyResultsForReportWithIncludesByIdSurveyAsync(selectedSurvey!.IdSurvey, this.isNAPOOEntry);
                        if (resultContext.ResultContextObject == null || resultContext.ResultContextObject.Count() == 0)
                        {
                            await this.ShowErrorAsync("Няма данни за получени резултати по тази анкета!");
                            return;
                        }

                        var summarizedReport = await this.AssessmentService.CreateExcelSummarizedReportWithSurveyResultsAsync(resultContext);
                        await this.JsRuntime.SaveAs($"Obobshtena_spravka_za_polucheni_rezultati_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", summarizedReport.ToArray());
                        break;
                    case ("Детайлна справка за получените резултати"):
                        resultContext.ResultContextObject = await this.AssessmentService.GetSurveyResultsForReportWithSurveyResultIncludesByIdSurveyAsync(selectedSurvey!.IdSurvey, this.isNAPOOEntry);
                        if (resultContext.ResultContextObject == null || resultContext.ResultContextObject.Count() == 0) 
                        {
                            await this.ShowErrorAsync("Няма данни за получени резултати по тази анкета!");
                            return;
                        }

                        var detailedReport = await this.AssessmentService.CreateExcelDetailedReportWithSurveyResultsAsync(resultContext);
                        await this.JsRuntime.SaveAs($"Detailna_spravka_za_polucheni_rezultati_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", detailedReport.ToArray());
                        break;
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task RowSelectingHandler()
        {
            var selectedRows = await this.surveyGrid.GetSelectedRecordsAsync();
            if (selectedRows.Any())
            {
                await this.surveyGrid.ClearRowSelectionAsync();
            }
        }

        private async Task GenerateExcelTemplateBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedRow = (await this.surveyGrid.GetSelectedRecordsAsync()).FirstOrDefault();
                if (selectedRow is null)
                {
                    await this.ShowErrorAsync("Моля, изберете анкета, за да генерирате шаблон за импорт!");
                    this.loading = false;
                    this.SpinnerHide();
                    return;
                }

                var excelStream = await this.AssessmentService.GetSurveyTemplateWithQuestionsFilledByIdSurveyAsync(selectedRow.IdSurvey);
                if (!excelStream.IsMacroIncluded)
                {
                    await this.JsRuntime.SaveAs($"Import_otgovori_anketa_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", excelStream.MS.ToArray());
                }
                else
                {
                    await this.JsRuntime.SaveAs($"Import_otgovori_anketa_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsm", excelStream.MS.ToArray());
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenImportSurveyModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedRow = (await this.surveyGrid.GetSelectedRecordsAsync()).FirstOrDefault();
                if (selectedRow is null)
                {
                    await this.ShowErrorAsync("Моля, изберете анкета, за да импортирате попълнени отговори!");
                    this.loading = false;
                    this.SpinnerHide();
                    return;
                }

                await this.importSurveyModal.OpenModal(selectedRow);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
            ExportColumns.Add(new GridColumn() { Field = "SurveysSentCount", HeaderText = "Брой изпратени анкети", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FiledOutCount", HeaderText = "Брой отговорили", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SurveyExpirationValue", HeaderText = "Статус", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
