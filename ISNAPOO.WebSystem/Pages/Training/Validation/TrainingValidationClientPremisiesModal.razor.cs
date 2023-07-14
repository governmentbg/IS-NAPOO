using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientPremisiesModal : BlazorBaseComponent
    {
        private SfGrid<ValidationPremisesVM> premisesGrid = new SfGrid<ValidationPremisesVM>();
        private SelectValidationPremisesModal selectValidationPremisesModal = new SelectValidationPremisesModal();
        private List<ValidationPremisesVM> premisesSource = new List<ValidationPremisesVM>();
        private ValidationPremisesVM premisesCourseToDelete = new ValidationPremisesVM();

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.premisesSource = (await TrainingService.GetAllValidationPremisiesByIdCourseAsync(ClientVM.IdValidationClient)).ToList();
        }

        private async Task AddPremisesBtn()
        {
            SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.selectValidationPremisesModal.OpenModal(ClientVM, this.premisesSource);
            }
            finally
            {
                this.loading = false;
            }

            SpinnerHide();
        }

        private async Task DeletePremisesBtn(ValidationPremisesVM model)
        {
            SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;           
                this.premisesCourseToDelete = model;

                bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
                if (isConfirmed)
                {

                    var result = new ResultContext<ValidationPremisesVM>();
                    result.ResultContextObject = model;
                    result = await TrainingService.DeleteValidationPremisesAsync(result);
                    if (result.HasErrorMessages)
                    {
                        await ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await ShowSuccessAsync(string.Join("", result.ListMessages));

                        this.premisesSource = (await TrainingService.GetAllValidationPremisiesByIdCourseAsync(ClientVM.IdValidationClient)).ToList();
                        await this.premisesGrid.Refresh();
                        StateHasChanged();
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            SpinnerHide();
        }

        private async Task UpdateAfterPremisesSelectAsync(Dictionary<int, List<CandidateProviderPremisesVM>> selectedPremises)
        {
            SpinnerShow();

            var resultContext = new ResultContext<List<CandidateProviderPremisesVM>>();
            resultContext.ResultContextObject = selectedPremises.FirstOrDefault().Value;

            resultContext = await TrainingService.CreateTrainingValidationPremisesByListCandidateProviderPremisesVMAsync(resultContext, ClientVM.IdValidationClient, selectedPremises.FirstOrDefault().Key);
            if (resultContext.HasErrorMessages)
            {
                await ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                return;
            }

            await ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

            this.premisesSource = (await TrainingService.GetAllValidationPremisiesByIdCourseAsync(ClientVM.IdValidationClient)).ToList();

            SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();

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
                ExportProperties.FileName = $"CoursePremisesList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.premisesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CoursePremisesList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();
                await this.premisesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "TrainingTypeName", HeaderText = "Вид на провежданото обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.PremisesName", HeaderText = "Материално-техническа база", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.Location.LocationName", HeaderText = "Населено място", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.ProviderAddress", HeaderText = "Адрес", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPremises.Phone", HeaderText = "Телефон", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
