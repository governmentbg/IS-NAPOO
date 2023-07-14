using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive.CIPO
{
    public partial class AnnualConsulting
    {
        [Inject]
        public IArchiveService archiveService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public ITrainingService trainingService { get; set; }

        [Inject]
        public IUploadFileService uploadFileService { get; set; }

        SfGrid<Consultings> sfGrid = new SfGrid<Consultings>();
        List<Consultings> consultingListForGrid = new List<Consultings>();

        private string TypeOfCheking = "";
        public async Task OpenModal(List<CandidateProviderVM> candidateProviderList, string TypeOfChecking, int year)
        {
            await this.LoadConsultantsData(candidateProviderList, TypeOfChecking, year);
            this.isVisible = true;
            this.StateHasChanged();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = consultingListForGrid.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "TypeOfConsulting", HeaderText = "Вид на отчет", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Count", HeaderText = "Брой", Width = "40", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
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
                ExportProperties.FileName = $"ConsultingInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                this.sfGrid.AllowExcelExport = true;
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "TypeOfConsulting", HeaderText = "Вид на отчет", Width = "300", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Count", HeaderText = "Брой", Width = "40", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"ConsultingInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public async Task LoadConsultantsData(List<CandidateProviderVM> candidateProviderList, string TypeOfChecking, int year)
        {
            this.TypeOfCheking = TypeOfChecking;           
            
            List<Consultings> tempList = new List<Consultings>();
            List<KeyValueVM> kvConsultings = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType", false)).ToList();
            foreach (var consulting in kvConsultings)
            {
                tempList.Add(new Consultings() { kvTypeOfCheking = consulting.IdKeyValue, TypeOfConsulting = consulting.Name, Count = 0});
            }

            foreach (var provider in candidateProviderList)
            {
                var consultings = await this.candidateProviderService.GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(provider.IdCandidate_Provider);
                foreach (var consulting in consultings)
                {
                    tempList.First(x => x.kvTypeOfCheking == consulting.IdConsultingType).Count ++;
                }
            }
            this.consultingListForGrid = tempList;
        }
    }
    public class Consultings
    {
        public int kvTypeOfCheking { get; set; }
        public string TypeOfConsulting { get; set; }
        public int Count { get; set; }
    }
}
