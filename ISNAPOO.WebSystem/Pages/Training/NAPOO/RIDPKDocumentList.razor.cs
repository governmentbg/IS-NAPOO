using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.WebSystem.Pages.Training.Validation;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.NAPOO
{
    public partial class RIDPKDocumentList : BlazorBaseComponent
    {
        private SfGrid<RIDPKVM> candidateProviderGrid = new SfGrid<RIDPKVM>();
        private RIDPKDocumentModal ridpkDocumentModal = new RIDPKDocumentModal();

        private IEnumerable<RIDPKVM> candidateProviderSource = new List<RIDPKVM>();
        private string title = string.Empty;
        private string type = string.Empty;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

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
                    this.type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();

                    this.SetTitle(this.type!);

                    await this.LoadDataAsync();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task LoadDataAsync()
        {
            this.candidateProviderSource = await this.TrainingService.GetListRIDPKVMOfSubmittedDocumentsForControlFromCPOAsync(this.type);
            this.StateHasChanged();
        }

        private void SetTitle(string type)
        {
            this.title = type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE ? "Проверка на издаваните от ЦПО документи за ПК" : "Проверка на издаваните от ЦПО документи за ПК, свързани с валидиране";
        }

        private async Task OpenRIDPKControlModalBtn(RIDPKVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.ridpkDocumentModal.OpenModal(this.type, model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateRIDPKDataAfterModalSubmit()
        {
            await this.LoadDataAsync();
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

                await this.candidateProviderGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"SurveyList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.candidateProviderGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", TextAlign = TextAlign.Left });

            if (this.type == GlobalConstants.TOKEN_RIDPK_DOCUMENTLIST_COURSE)
            {
                ExportColumns.Add(new GridColumn() { Field = "Course.CourseName", HeaderText = "Курс", TextAlign = TextAlign.Left });
            }

            ExportColumns.Add(new GridColumn() { Field = "FrameworkProgram.Name", HeaderText = "Рамкова програма", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Speciality.VQS_Name", HeaderText = "СПК", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Speciality.CodeAndName", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SubmittedDocumentCount", HeaderText = "Брой подадени документи за ПК", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SubmitDateAsStr", HeaderText = "Дата на подаване", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
