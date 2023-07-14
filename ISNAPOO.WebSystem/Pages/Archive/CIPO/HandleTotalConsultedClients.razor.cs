using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Common;
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
    public partial class HandleTotalConsultedClients
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
            if (TypeOfChecking == "Данни за клиентите на ЦИПО")
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

        public async Task LoadConsultantsData(List<CandidateProviderVM> candidateProviderList, string TypeOfChecking, int year)
        {
            this.TypeOfCheking = TypeOfChecking;
            this.consultedClientsGridSource = new List<ConsultedClients>();
            ConsultedClients cc = new ConsultedClients();
            cc.Identifier = $"Общо консултирани клиенти през текуща година {year}";
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        KeyValueVM temp = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((temp.KeyValueIntCode == "Type9" || temp.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }

                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            //cc = new ConsultedClients();
            //cc.Identifier = $"Общо предоставени услуги през текуща ${year}";
            //cc.Count = 0;
            //foreach (var candidate in candidateProviderList)
            //{
            //    var clients = await this.candidateProviderService.GetAllCandidateProviderConsultingsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
            //    foreach (var client in clients)
            //    {
            //        if (client.IdFinishedType != null)
            //        {
            //            var Country = await this.dataSourceService.GetKeyValueByIdAsync(client.IdNationality);
            //            var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
            //            if (FinishType.KeyValueIntCode == "Type1" && (Country.DefaultValue3 == "EU" && Country.KeyValueIntCode != "Bulgaria") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
            //            {
            //                cc.Count += 1;
            //            }
            //        }
            //    }
            //}
            //consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти с националност друга страна членка от Европейския съюз";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var Country = await this.dataSourceService.GetKeyValueByIdAsync(client.IdNationality);
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if (((FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") || FinishType.KeyValueIntCode == "Type10") && (Country.DefaultValue3 == "EU" && Country.KeyValueIntCode != "Bulgaria") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти с националност България";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var Country = await this.dataSourceService.GetKeyValueByIdAsync(client.IdNationality);
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && Country.KeyValueIntCode == "Bulgaria" && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти с националност трета страна";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var Country = await this.dataSourceService.GetKeyValueByIdAsync(client.IdNationality);
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && (Country.DefaultValue3 != "EU" && Country.KeyValueIntCode != "Bulgaria") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти - учащи";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((client.IsStudent == true) && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти – неучащи";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((client.IsStudent == false) && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти - заети лица";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((client.IsEmployedPerson == true) && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти – безработни лица";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((client.IsStudent == false && client.IsEmployedPerson == false) && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }

            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти с регистрация в Бюрото по труда";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((client.IdRegistrationAtLabourOfficeType != null) && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Консултирани клиенти без регистрация в Бюрото по труда";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if ((client.IdRegistrationAtLabourOfficeType == null) && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Клиенти, насочени към услугите на ЦИПО от работодател";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null && client.IdAimAtCIPOServicesType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        var AimAtCIPO = await this.dataSourceService.GetKeyValueByIdAsync(client.IdAimAtCIPOServicesType);
                        if ((AimAtCIPO != null && AimAtCIPO.KeyValueIntCode == "Employer") && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Клиенти, насочени към услугите на ЦИПО от училище, ЦПО, колеж, висше училище или друга обучаваща институция";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null && client.IdAimAtCIPOServicesType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        var AimAtCIPO = await this.dataSourceService.GetKeyValueByIdAsync(client.IdAimAtCIPOServicesType);
                        if ((AimAtCIPO != null &&  AimAtCIPO.KeyValueIntCode == "EducationAuthority") && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Клиенти, насочени към услугите на ЦИПО от Бюро по труда";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null && client.IdAimAtCIPOServicesType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        var AimAtCIPO = await this.dataSourceService.GetKeyValueByIdAsync(client.IdAimAtCIPOServicesType);
                        if ((AimAtCIPO != null && AimAtCIPO.KeyValueIntCode == "LabourOffice") && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Клиенти, насочени към услугите на ЦИПО от семейство и/или приятели";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null && client.IdAimAtCIPOServicesType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        var AimAtCIPO = await this.dataSourceService.GetKeyValueByIdAsync(client.IdAimAtCIPOServicesType);
                        if ((AimAtCIPO != null &&  AimAtCIPO.KeyValueIntCode == "FamilyOrFriends") && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Клиенти, взели сами решението да потърсят услугите на ЦИПО";
            cc.Count = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdFinishedType != null && client.IdAimAtCIPOServicesType != null)
                    {
                        var FinishType = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        var AimAtCIPO = await this.dataSourceService.GetKeyValueByIdAsync(client.IdAimAtCIPOServicesType);
                        if ((AimAtCIPO != null &&  AimAtCIPO.KeyValueIntCode == "OwnChoice") && (FinishType.KeyValueIntCode == "Type9" || FinishType.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                        {
                            cc.Count += 1;
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Средна възраст на консултираните клиенти";
            cc.Count = 0;
            int age = 0;
            int ClientCount = 0;
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                if (clients != null && clients.Count() != 0)
                {
                    foreach (var client in clients)
                    {
                        if (client.IdFinishedType != null)
                        {
                            var temp = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                            if ((temp.KeyValueIntCode == "Type9" || temp.KeyValueIntCode == "Type10") && int.Parse(client.EndDate.Value.ToString("yyyy")) == year)
                            {
                                age += DateTime.Now.Year - client.BirthDate.Value.Year;
                                ClientCount++;
                            }
                        }
                    }
                }
                else
                {
                 
                }

            }
            if (ClientCount > 0 && age > 0)
            {
                cc.Count = age / ClientCount;
            }
            consultedClientsGridSource.Add(cc);

            cc = new ConsultedClients();
            cc.Identifier = "Общ брой издадени документи, удостоверяващи предоставянето на услуга";
            foreach (var candidate in candidateProviderList)
            {
                var clients = await this.trainingService.GetAllConsultingClientsByIdCandidateProviderAsync(candidate.IdCandidate_Provider);
                foreach (var client in clients)
                {
                    if (client.IdConsultingClient != null && client.IdFinishedType != null)
                    {
                        var temp = await this.dataSourceService.GetKeyValueByIdAsync(client.IdFinishedType);
                        if (temp.KeyValueIntCode == "Type9" || temp.KeyValueIntCode == "Type10")
                        {
                            var tempdoc = await this.trainingService.GetConsultingClientDocumentUploadedFileById(client.IdConsultingClient);
                            if (tempdoc.Count != 0)
                            {
                                cc.Count += tempdoc.Count;
                            }
                        }
                    }
                }
            }
            consultedClientsGridSource.Add(cc);
        }
    }
    public class ConsultedClients
    {
        [Key]
        public int Id { get; set; }
        public string Identifier { get; set; }
        public decimal Count { get; set; } = 0;
    }
}
