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
   

    public partial class AnnualTrainingCourseModal : BlazorBaseComponent
    {
        [Inject]
        public IArchiveService ArchiveService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        private List<AnnualTrainingCourse> annualTrainingCourses = new List<AnnualTrainingCourse>();
        private SfGrid<AnnualTrainingCourse> sfGrid = new SfGrid<AnnualTrainingCourse>();

        public AnnualTrainingCourseTitle AnnualTrainingCourseTitle { get; set; } = new AnnualTrainingCourseTitle();

        private SfDialog sfDialog = new SfDialog();
        public async Task OpenModal(List<CandidateProviderVM> listCandidateProviderVM, string frameworkProgramValue, string year)
        {
            this.isVisible = true; 

            //var kv = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", frameworkProgramValue);

            switch (frameworkProgramValue)
            {
                case ("SPK"):
                    AnnualTrainingCourseTitle.HeaderTitle = "Проведени курсове за придобиване на степен на професионална квалификация, записани курсисти и курсисти придобили степен на ПК или свидетелство за правоспособност";
                    AnnualTrainingCourseTitle.CountIncludedTitle = "Записани курсисти за придобиване на степен на ПК - брой";
                    AnnualTrainingCourseTitle.CountCertificateTitle = "Курсисти,придобили степен на ПК - брой";
                    this.annualTrainingCourses = await this.ArchiveService.GetTrainingCourseAsync(listCandidateProviderVM, frameworkProgramValue, year);
                    break;
                case ("PartProfession"):
                    AnnualTrainingCourseTitle.HeaderTitle = "Проведени курсове по част от професията, записани курсисти и курсисти, получили удостоверение или свидетелство за правоспособност";
                    AnnualTrainingCourseTitle.CountIncludedTitle = "Записани курсисти в курсове по част от професията";
                    AnnualTrainingCourseTitle.CountCertificateTitle = "Курсисти, получили удостоверение за професионално обучение - брой";
                    this.annualTrainingCourses = await this.ArchiveService.GetTrainingCourseAsync(listCandidateProviderVM, frameworkProgramValue, year);
                    break;     
                case ("OtherCourses"):
                    AnnualTrainingCourseTitle.HeaderTitle = "Проведени курсове(други)";
                    AnnualTrainingCourseTitle.CountIncludedTitle = "Записани курсисти в курсове";
                    AnnualTrainingCourseTitle.CountCertificateTitle = "Курсисти получили удостоверение/свидетелство/сертификат за професионално обучение - брой";
                    this.annualTrainingCourses = await this.ArchiveService.GetTrainingCourseAsync(listCandidateProviderVM, frameworkProgramValue, year);
                    break;
                case ("CourseRegulation1And7"):
                    AnnualTrainingCourseTitle.HeaderTitle = "Проведени курсове за придобиване на степен на професионална квалификация, записани курсисти и курсисти придобили степен на ПК или свидетелство за правоспособност";
                    AnnualTrainingCourseTitle.CountIncludedTitle = "Записани курсисти за придобиване на степен на ПК - брой";
                    AnnualTrainingCourseTitle.CountCertificateTitle = "Курсисти,придобили степен на ПК - брой";
                    this.annualTrainingCourses = await this.ArchiveService.GetTrainingCourseAsync(listCandidateProviderVM, frameworkProgramValue, year);
                    break;
                default:
                    break;
            }

           // this.annualTrainingCourses = await this.ArchiveService.GetTrainingCourseAsync(listCandidateProviderVM, frameworkProgramValue, year);

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
                ExportProperties.FileName = $"AnnualTrainingCourseModal_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("sfGrid_excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"AnnualTrainingCourseModal_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

       
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

       

    }

    public class AnnualTrainingCourseTitle
    {
        public string HeaderTitle { get; set; }
        public string CountIncludedTitle { get; set; }
        public string CountCertificateTitle { get; set; }
    }
}
