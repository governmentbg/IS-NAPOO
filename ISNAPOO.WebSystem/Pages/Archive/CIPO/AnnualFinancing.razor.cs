using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Archive;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Archive.CIPO
{
    public partial class AnnualFinancing
    {
        [Inject]
        public IArchiveService archiveService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }
        [Inject]
        public ITrainingService trainingService { get; set; }

        SfGrid<ConsultedClients> sfGrid = new SfGrid<ConsultedClients>();
        List<ConsultedClients> consultedClientsGridSource = new List<ConsultedClients>();

        private string TypeOfCheking = "";
        public async Task OpenModal(List<CandidateProviderVM> candidateProviderList, string TypeOfChecking, int year)
        {
            if (TypeOfChecking == "Източници на финансиране")
            {
                await this.LoadConsultantsData(candidateProviderList, TypeOfChecking, year);
            }
            this.isVisible = true;
            this.StateHasChanged();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = consultedClientsGridSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "Identifier", HeaderText = "Вид на отчет", Width = "120", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"ClientsInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

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
                ExportColumns.Add(new GridColumn() { Field = "Identifier", HeaderText = "Вид на отчет", Width = "640", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Count", HeaderText = "Брой", Width = "40", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"ClientsInfo_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private void CellInfoHandler(QueryCellInfoEventArgs<ConsultedClients> args)
        {

            if (!(args.Data.Identifier == "Средна стойност на предоставените услуги в български лева" 
                || args.Data.Identifier == "Брой клиенти, самофинансирали използваните услуги"
                || args.Data.Identifier == "Брой клиенти, чиито услуги са били финансирани от работодател"
                || args.Data.Identifier == "Брой клиенти, чиито услуги са били финансирани от публични източници"))
            {
                args.Cell.AddStyle(new string[] { "color: grey" });
            }
        }

        public async Task LoadConsultantsData(List<CandidateProviderVM> candidateProviderList, string TypeOfChecking, int year)
        {
            this.TypeOfCheking = TypeOfChecking;
            this.consultedClientsGridSource = new List<ConsultedClients>();
            ConsultedClients cc = new ConsultedClients();
            cc.Identifier = $"Средна стойност на предоставените услуги в български лева";
            var consultingsCount = 0;
            foreach (var candidate in candidateProviderList)
            {                                           
                var consultings = await this.trainingService.GetAllConsultingByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var consulting in consultings)
                {
                    cc.Count += (decimal)(consulting.Cost != null ? consulting.Cost : 0);
                }
                consultingsCount += consultings.Count();
            }
            if (consultingsCount > 0)
            {
                cc.Count = Math.Round(cc.Count / consultingsCount, 2);
            }
            consultedClientsGridSource.Add(cc);

            List<KeyValueVM> kvConsultings = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType", false)).ToList();
            foreach (var kvConsulting in kvConsultings)
            {
                cc = new ConsultedClients();
                cc.Identifier = kvConsulting.Name;
                consultingsCount = 0;
                foreach (var candidate in candidateProviderList)
                {
                    var consultings = (await this.trainingService.GetAllConsultingByIdCandidateProviderAsync(candidate.IdCandidate_Provider)).Where(x => x.IdConsultingType == kvConsulting.IdKeyValue).ToList();
                    foreach (var consulting in consultings)
                    {
                        cc.Count += (decimal)(consulting.Cost != null ? consulting.Cost : 0);
                    }
                    consultingsCount += consultings.Count;
                }
                if (consultingsCount > 0)
                {
                    Math.Round(cc.Count / consultingsCount, 2);
                }
                consultedClientsGridSource.Add(cc);
            }

            cc = new ConsultedClients();
            cc.Identifier = "Брой клиенти, самофинансирали използваните услуги";
            var kvAssignType = await this.dataSourceService.GetKeyValueByIntCodeAsync("AssignType", "Type11");
            foreach (var candidate in candidateProviderList)
            {
                var ConsultingClients = (await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider));
                foreach (var consultingClient in ConsultingClients.Where(x => x.IdAssignType == kvAssignType.IdKeyValue))
                {
                    cc.Count += 1;
                }               
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Брой клиенти, чиито услуги са били финансирани от работодател";
            kvAssignType = await this.dataSourceService.GetKeyValueByIntCodeAsync("AssignType", "Type12");
            foreach (var candidate in candidateProviderList)
            {
                var ConsultingClients = (await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider));
                foreach (var consultingClient in ConsultingClients.Where(x => x.IdAssignType == kvAssignType.IdKeyValue))
                {
                    cc.Count += 1;
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Брой клиенти, чиито услуги са били финансирани от публични източници";
            kvAssignType = await this.dataSourceService.GetKeyValueByIntCodeAsync("AssignType", "Type13");
            foreach (var candidate in candidateProviderList)
            {
                var ConsultingClients = (await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider));
                foreach (var consultingClient in ConsultingClients.Where(x => x.IdAssignType == kvAssignType.IdKeyValue))
                {
                    cc.Count += 1;
                }
            }
            consultedClientsGridSource.Add(cc);
        }
    }
}

