using Data.Models.Data.Common;
using Data.Models.Data.SqlView.Archive;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive
{
    public partial class AnnualTrainingValidationClientModal : BlazorBaseComponent
    {
        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        private List<AnnualTrainingValidationClientCourse> annualTrainingValidationClientCourses = new List<AnnualTrainingValidationClientCourse>();
        private SfGrid<AnnualTrainingValidationClientCourse> sfGrid = new SfGrid<AnnualTrainingValidationClientCourse>();

        public AnnualTrainingCourseTitle AnnualTrainingCourseTitle { get; set; } = new AnnualTrainingCourseTitle();

        private SfDialog sfDialog = new SfDialog();
        public async Task OpenModal(List<CandidateProviderVM> listCandidateProviderVM, string year)
        {
            this.isVisible = true;
                    this.annualTrainingValidationClientCourses = await this.ArchiveService.GetTrainingValidationClientAsync(listCandidateProviderVM, year);

            this.StateHasChanged();
        }

        private async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("sfGrid_pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;



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
                ExportProperties.FileName = $"AnnualTrainingValidationClientModal_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("sfGrid_excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualTrainingValidationClientModal_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;


                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }



    }
}
