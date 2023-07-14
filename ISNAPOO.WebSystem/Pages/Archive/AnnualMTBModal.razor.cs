using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualMTBModal : BlazorBaseComponent
    {
        private SfGrid<AnnualMTBVM> dataGrid = new SfGrid<AnnualMTBVM>();

        private List<AnnualMTBVM> dataGridSource = new List<AnnualMTBVM>();

        [Inject]
        public IArchiveService ArchiveService { get; set; }

        public async Task OpenModal(List<CandidateProviderVM> candidateProviders, int year, string licensingType)
        {
            this.dataGridSource = (await this.ArchiveService.GetAnnualMTBReportAsync(candidateProviders, year, licensingType)).ToList();

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
                ExportProperties.FileName = $"AnnualMTBReport_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.dataGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualMTBReport_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
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
            ExportColumns.Add(new GridColumn() { Field = "DOSCompatibility", HeaderText = "МТБ е в съответствие с ДОС по тази специалност", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "MTBOwnership", HeaderText = "Наличие на собствена база за практическо обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "MTBName", HeaderText = "Име на базата", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
