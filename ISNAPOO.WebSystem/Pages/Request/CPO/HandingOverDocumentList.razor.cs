using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class HandingOverDocumentList : BlazorBaseComponent
    {
        private SfGrid<RequestDocumentManagementVM> requestDocumentManagementsGrid = new SfGrid<RequestDocumentManagementVM>();
        private ToastMsg toast = new ToastMsg();
        private HandingOverDocumentModal handingOverDocumentModal = new HandingOverDocumentModal();

        private List<RequestDocumentManagementVM> requestDocumentManagementsSource = new List<RequestDocumentManagementVM>();
        private List<TypeOfRequestedDocumentVM> typeOfRequestedDocumentsSource = new List<TypeOfRequestedDocumentVM>();
        private List<DocumentSeriesVM> documentSeriesSource = new List<DocumentSeriesVM>();
        private List<RequestDocumentManagementVM> selectedReqDocManagements = new List<RequestDocumentManagementVM>();
        private List<CandidateProviderVM> providersSource = new List<CandidateProviderVM>();

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.documentSeriesSource = (await this.ProviderDocumentRequestService.GetAllDocumentSeriesAsync()).ToList();
            this.typeOfRequestedDocumentsSource = (await this.ProviderDocumentRequestService.GetAllValidTypesOfRequestedDocumentAsync()).ToList();
            this.providersSource = (await this.CandidateProviderService.GetAllActiveCandidateProvidersWithoutAnythingIncludedAsync(new CandidateProviderVM() { IsActive = true})).ToList();
            await this.SetRequestDocumentManagementDataForGrid();
        }

        private async Task SetRequestDocumentManagementDataForGrid()
        {
            this.requestDocumentManagementsSource = (await this.ProviderDocumentRequestService.GetAllRequestDocumentManagementsByDocumentOperationSubmittedAndByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider)).OrderByDescending(x => x.DocumentDate).ToList();
            foreach (var requestDocManagement in this.requestDocumentManagementsSource)
            {
                var provider = this.providersSource.FirstOrDefault(x => x.IdCandidate_Provider == requestDocManagement.IdCandidateProvider);
                requestDocManagement.CandidateProvider = provider;
                var typeOfDoc = this.typeOfRequestedDocumentsSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == requestDocManagement.IdTypeOfRequestedDocument);
                requestDocManagement.TypeOfRequestedDocument = typeOfDoc;

                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == requestDocManagement.IdTypeOfRequestedDocument);
                if (docSeries is not null)
                {
                    requestDocManagement.DocumentSeriesName = docSeries.SeriesName;
                }

                var providerPartner = this.providersSource.FirstOrDefault(x => x.IdCandidate_Provider == requestDocManagement.IdCandidateProviderPartner);
                if (providerPartner is not null)
                {
                    requestDocManagement.CandidateProviderPartner = providerPartner;
                }
            }
        }

        private async Task AddNewHandOverDocument()
        {
            bool hasPermission = await CheckUserActionPermission("ManageHandingOverDocumentData", false);
            if (!hasPermission) { return; }


            await this.handingOverDocumentModal.OpenModal(new RequestDocumentManagementVM() { IdCandidateProvider = this.UserProps.IdCandidateProvider }, this.documentSeriesSource);
        }

        private async Task EditHandOverDocumentModal(RequestDocumentManagementVM requestDocumentManagement)
        {
            bool hasPermission = await CheckUserActionPermission("ViewHandingOverDocumentData", false);
            if (!hasPermission) { return; }

            var requestDocManagement = await this.ProviderDocumentRequestService.GetRequestDocumentManagementByIdAsync(requestDocumentManagement);
            await this.handingOverDocumentModal.OpenModal(requestDocManagement, this.documentSeriesSource);
        }

        private async Task UpdateAfterModalSubmit()
        {
            await this.SetRequestDocumentManagementDataForGrid();
        }

        private async Task GenerateProtocol()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.selectedReqDocManagements.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете ред/редове от списъка!");
                    return;
                }

                foreach (var selectedDocumentManagement in this.selectedReqDocManagements)
                {
                    if (this.selectedReqDocManagements.Any(x => x.DocumentDate != selectedDocumentManagement.DocumentDate) || this.selectedReqDocManagements.Any(x => x.IdCandidateProviderPartner != selectedDocumentManagement.IdCandidateProviderPartner))
                    {
                        await this.ShowErrorAsync("Моля, изберете редове от списъка, за които има съответствие на данните за Предаден на и Дата на предаване!");
                        return;
                    }

                    if (this.selectedReqDocManagements.Any(x => x.TypeOfRequestedDocument.HasSerialNumber != selectedDocumentManagement.TypeOfRequestedDocument.HasSerialNumber))
                    {
                        await this.ShowErrorAsync("Моля, изберете редове от списъка, за които има съответствие на данните за наличие на фабричен номер!");
                        return;
                    }
                }

                var result = await this.ProviderDocumentRequestService.PrintHandingOverProtocolAsync(this.selectedReqDocManagements, this.typeOfRequestedDocumentsSource, this.documentSeriesSource);
                await FileUtils.SaveAs(this.JsRuntime, "PPP_za_dokumenti_s_fabrichni_nomera.docx", result.ToArray());
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task RequestDocumentManagementDeselected(RowDeselectEventArgs<RequestDocumentManagementVM> args)
        {
            this.selectedReqDocManagements = await this.requestDocumentManagementsGrid.GetSelectedRecordsAsync();
        }

        private async Task RequestDocumentManagementSelected(RowSelectEventArgs<RequestDocumentManagementVM> args)
        {
            this.selectedReqDocManagements = await this.requestDocumentManagementsGrid.GetSelectedRecordsAsync();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = requestDocumentManagementsGrid.PageSettings.PageSize;
                requestDocumentManagementsGrid.PageSettings.PageSize = requestDocumentManagementsSource.Count();
                await requestDocumentManagementsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderOwner", HeaderText = "Юридическо лице", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentCount", HeaderText = "Брой", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceiveDocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата на предаване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPartner.ProviderOwner", HeaderText = "Предадени на", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"HandingOverDocument_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.requestDocumentManagementsGrid.ExportToPdfAsync(ExportProperties);
                requestDocumentManagementsGrid.PageSettings.PageSize = temp;
                await requestDocumentManagementsGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.ProviderOwner", HeaderText = "Юридическо лице", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", Width = "50", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentCount", HeaderText = "Брой", Width = "50", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceiveDocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата на предаване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPartner.ProviderOwner", HeaderText = "Предадени на", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"HandingOverDocument_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.requestDocumentManagementsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<RequestDocumentManagementVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(requestDocumentManagementsGrid, args.Data.IdRequestDocumentManagement).Result.ToString();
            }
        }
    }
}
