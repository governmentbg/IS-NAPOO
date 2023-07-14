using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class DocumentStatusModal : BlazorBaseComponent
    {
        private SfGrid<DocumentStatusVM> statusesGrid = new SfGrid<DocumentStatusVM>();

        private IEnumerable<DocumentStatusVM> statusesSource = new List<DocumentStatusVM>();

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public async Task OpenModal(int idEntity, string type)
        {
            this.statusesSource = await this.TrainingService.GetDocumentStatusesByIdAsync(idEntity, type);

            this.isVisible = true;
            this.StateHasChanged();
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
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.statusesGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.statusesGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "StatusValue", HeaderText = "Статус", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "StatusDate", HeaderText = "Дата на промяна", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "PersonName", HeaderText = "Потребител", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "StatusComment", HeaderText = "Коментар", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
