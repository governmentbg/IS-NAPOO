using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualCurriculumModal : BlazorBaseComponent
    {
        private SfGrid<AnnualCurriculumsVM> dataGrid = new SfGrid<AnnualCurriculumsVM>();

        private List<AnnualCurriculumsVM> dataGridSource = new List<AnnualCurriculumsVM>();

        [Inject]
        public IArchiveService ArchiveService { get; set; }

        public void OpenModal(List<CandidateProviderVM> candidateProviders, int year, string licensingType)
        {
            this.dataGridSource = this.ArchiveService.GetAnnualCurriculumsReport(candidateProviders, year, licensingType).ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task ToolbarClick(ClickEventArgs args)
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
                ExportProperties.FileName = $"AnnualCurriculums_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.dataGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualCurriculums_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.dataGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Id", HeaderText = "№", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CPOName", HeaderText = "ЦПО", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Profession", HeaderText = "Професия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Speciality", HeaderText = "Специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "UpdateReason", HeaderText = "Основна причина за актуализация на учебния план и програма", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Date", HeaderText = "Дата", TextAlign = TextAlign.Left, Format="dd.MM.yyyy" });

            return ExportColumns;
        }
    }
}
