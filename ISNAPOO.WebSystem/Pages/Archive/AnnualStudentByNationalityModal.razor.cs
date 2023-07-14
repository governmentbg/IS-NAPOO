using ISNAPOO.WebSystem.Pages.Framework;
using Data.Models.Data.SqlView.Archive;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualStudentByNationalityModal : BlazorBaseComponent
    {
        private SfGrid<AnnualStudentsByNationality> sfGrid = new SfGrid<AnnualStudentsByNationality>();
        List<CandidateProviderVM> candidateProviders;

        List<AnnualStudentsByNationality> dataGridSource = new List<AnnualStudentsByNationality>();
        List<AnnualStudentsByNationality> dataSource = new List<AnnualStudentsByNationality>();

        [Inject]
        public IArchiveService ArchiveService { get; set; }
        public async Task OpenModal(List<CandidateProviderVM> candidateProvidersVM)
        {
            this.dataGridSource = new List<AnnualStudentsByNationality>();
            this.candidateProviders = candidateProvidersVM;
            this.dataSource = await this.ArchiveService.GetStudentsByNationalityAsync(this.candidateProviders);
            if (dataGridSource.Any())
            {
                dataGridSource.Add(new AnnualStudentsByNationality() { Nationality = "Общо", IdNationality = this.dataGridSource.Last().IdNationality + 1 });
            }
            foreach (var year in dataGridSource.Where(x => x.Nationality != "Общо"))
            {
                dataGridSource.First(x => x.Nationality == "Общо").CountIncludedMen += year.CountIncludedMen;
                dataGridSource.First(x => x.Nationality == "Общо").CountCertifiedMen += year.CountCertifiedMen;
                dataGridSource.First(x => x.Nationality == "Общо").CountProfessionallyCertifiedMen += year.CountProfessionallyCertifiedMen;
                dataGridSource.First(x => x.Nationality == "Общо").CountIncludedPartOfProfessionMen += year.CountIncludedPartOfProfessionMen;
            }
            this.dataGridSource = dataSource;
            this.isVisible = true;
            this.StateHasChanged();
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
                List<GridColumn> FinalExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "IdNationality", HeaderText = "№", AutoFit=true });
                ExportColumns.Add(new GridColumn() { Field = "Nationality", HeaderText = "Държава", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CountIncludedMen", HeaderText = "Записани курсисти за придобиване на степен на ПК", TextAlign = TextAlign.Left, AutoFit = true });
                ExportColumns.Add(new GridColumn() { Field = "CountCertifiedMen", HeaderText = "Курсисти, придобили степен на ПК", TextAlign = TextAlign.Left, AutoFit = true });
                ExportColumns.Add(new GridColumn() { Field = "CountIncludedPartOfProfessionMen", HeaderText = "Записани курсисти в курсове по част от професия", TextAlign = TextAlign.Left, AutoFit = true });
                ExportColumns.Add(new GridColumn() { Field = "CountProfessionallyCertifiedMen", HeaderText = "Курсисти, получили удостоверение за професионално обучение", TextAlign = TextAlign.Left, AutoFit = true });
                FinalExportColumns.Add(new GridColumn() { HeaderText = "Курсисти в ЦПО по гражданство", TextAlign = TextAlign.Left, Columns = ExportColumns, AutoFit = true });
#pragma warning restore BL0005
                ExportProperties.Columns = FinalExportColumns;
                ExportProperties.PageOrientation = Syncfusion.Blazor.Grids.PageOrientation.Landscape;
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

                ExportProperties.FileName = $"AnnualStudentsByNattionality_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualStudentsByNattionality_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }
    }
}
