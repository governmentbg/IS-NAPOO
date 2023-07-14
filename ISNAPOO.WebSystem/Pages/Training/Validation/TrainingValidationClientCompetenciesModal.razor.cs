using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientCompetenciesModal : BlazorBaseComponent
    {
        [Parameter]
        public ValidationClientVM ClientVM { get; set; }
        ValidationClientCompetenciesModal modal;
        private SfGrid<ValidationCompetencyVM> Grid = new SfGrid<ValidationCompetencyVM>();

        [Inject]
        public ITrainingService TrainingService { get; set; }
        [Parameter]
        public EventCallback<int?> CallbackAfterSubmit { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;
        private async Task EditCompetencyBtn(ValidationCompetencyVM validationCompetency)
        {
            if (this.loading) return;
            try
            {
                this.loading = true;
                await this.modal.OpenModal(validationCompetency);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task AddCompetencyBtn()
        {
            if (this.loading) return;
            try
            {
                this.loading = true;
                var validationCompetency = new ValidationCompetencyVM()
                {
                    IdValidationClient = ClientVM.IdValidationClient
                };

                await this.modal.OpenModal(validationCompetency);
            }
            finally
            {
                this.loading = false;
            }

        }

        private async Task RemoveCompetency()
        {
            if (this.loading) return;
            try
            {
                this.loading = true;

                var selected = await this.Grid.GetSelectedRecordsAsync();
                if (selected.Any())
                {
                    bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
                    if (isConfirmed)
                    {
                        var result = await TrainingService.DeleteValidationCompetencies(selected);

                        await CallbackAfterSubmit.InvokeAsync();
                    }
                }
                else
                {
                    await ShowErrorAsync("Моля, изберете поне един ред от добавените компетентности!");
                }
            }
            finally
            {
                this.loading = false;
            }
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
                ExportProperties.FileName = $"CourseProtocolList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.Grid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CourseProtocolList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();
                await this.Grid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "CompetencyNumber", HeaderText = "№ на компетентност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Competency", HeaderText = "Компетентност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "IsCompetencyRecognizedStr", HeaderText = "Компетентност", TextAlign = TextAlign.Left, Format = "dd.MM.yyyy" });

            return ExportColumns;
        }

        public async Task updateClient()
        {
            await CallbackAfterSubmit.InvokeAsync();
        }
    }
}
