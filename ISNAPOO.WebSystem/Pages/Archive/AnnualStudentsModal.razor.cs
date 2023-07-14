using Data.Models.Data.Common;
using Data.Models.Data.SqlView.Archive;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualStudentsModal : BlazorBaseComponent
    {

        private SfGrid<AnnualStudents> sfGrid = new SfGrid<AnnualStudents>();
        List<CandidateProviderVM> candidateProviders;

        List<AnnualStudents> dataGridSource = new List<AnnualStudents>();

        [Inject]
        public IArchiveService ArchiveService { get; set; }
        public async Task OpenModal(List<CandidateProviderVM> candidateProvidersVM)
        {
            this.candidateProviders = candidateProvidersVM;
            this.dataGridSource = await this.ArchiveService.GetStudentsAsync(this.candidateProviders);
            dataGridSource.Add(new AnnualStudents() {BirthDate = "Общо"});
            foreach (var year in dataGridSource.Where(x => x.BirthDate != "Общо"))
            {
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedMan_I_VQS += year.CountIncludedMan_I_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedMan_II_VQS += year.CountIncludedMan_II_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedMan_III_VQS += year.CountIncludedMan_III_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedMan_IV_VQS += year.CountIncludedMan_IV_VQS;

                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedWomen_I_VQS += year.CountIncludedWomen_I_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedWomen_II_VQS += year.CountIncludedWomen_II_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedWomen_III_VQS += year.CountIncludedWomen_III_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountIncludedWomen_IV_VQS += year.CountIncludedWomen_IV_VQS;


                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateMan_I_VQS += year.CountCertificateMan_I_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateMan_II_VQS += year.CountCertificateMan_II_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateMan_III_VQS += year.CountCertificateMan_III_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateMan_IV_VQS += year.CountCertificateMan_IV_VQS;

                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateWomen_I_VQS += year.CountCertificateWomen_I_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateWomen_II_VQS += year.CountCertificateWomen_II_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateWomen_III_VQS += year.CountCertificateWomen_III_VQS;
                dataGridSource.First(x => x.BirthDate == "Общо").CountCertificateWomen_IV_VQS += year.CountCertificateWomen_IV_VQS;
            }
            this.isVisible = true;
            this.StateHasChanged();
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
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
                ExportProperties.FileName = $"AnnualStudents_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualStudents_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }
    }
}
