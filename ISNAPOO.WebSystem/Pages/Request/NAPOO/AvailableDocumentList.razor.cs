using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Data;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class AvailableDocumentList : BlazorBaseComponent
    {
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private SfGrid<RequestDocumentManagementControlModel> requestDocumentManagementsGrid = new SfGrid<RequestDocumentManagementControlModel>();
        private NAPOOTypeOfRequestedDocumentReferenceModal napooTypeOfRequestedDocumentReferenceModal = new NAPOOTypeOfRequestedDocumentReferenceModal();

        private List<RequestDocumentManagementControlModel> requestDocumentManagementsSource = new List<RequestDocumentManagementControlModel>();
        private IEnumerable<KeyValueVM> kvDocumentOperationsSource = new List<KeyValueVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private int? idCandidateProvider = null;

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.kvDocumentOperationsSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");

                if (this.UserProps.IdCandidateProvider == 0)
                {
                    this.candidateProvidersSource = (await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO")).ToList();
                }
                else
                {
                    this.requestDocumentManagementsSource = (await this.ProviderDocumentRequestService.GetDocumentsControlDataAsync(this.UserProps.IdCandidateProvider)).OrderBy(x => x.DocumentYear).ThenBy(d => d.TypeOfRequestedDocument.DocTypeOfficialNumber).ToList();
                    await this.requestDocumentManagementsGrid.Refresh();
                }

                this.StateHasChanged();
            }
        }

        private async Task OpenDocumentSerialNumberModal(RequestDocumentManagementControlModel requestDocumentManagement)
        {
            bool hasPermission = await CheckUserActionPermission("ViewNAPOOControlDocumentData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var serialNumbersForModal = this.GetAvailableFabricNumbers(requestDocumentManagement.DocumentSerialNumbers);
                requestDocumentManagement.DocumentSerialNumbers = serialNumbersForModal;

                await this.napooTypeOfRequestedDocumentReferenceModal.OpenModal(requestDocumentManagement);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private List<DocumentSerialNumberVM> GetAvailableFabricNumbers(List<DocumentSerialNumberVM> serialNumbers)
        {
            var list = new List<DocumentSerialNumberVM>();
            var kvReceived = this.kvDocumentOperationsSource.FirstOrDefault(x => x.KeyValueIntCode == "Received").IdKeyValue;
            foreach (var serialNumber in serialNumbers)
            {
                if (serialNumber.IdDocumentOperation != kvReceived)
                {
                    continue;
                }

                var repeatedSerialNumbers = serialNumbers.Where(x => x.SerialNumber == serialNumber.SerialNumber);
                if (repeatedSerialNumbers.Count() > 1)
                {
                    continue;
                }

                list.Add(serialNumber);
            }

            return list;
        }

        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ProviderJoinedInformation", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }

        private async Task LoadDataForCandidateProviderBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.idCandidateProvider.HasValue)
                {
                    await this.ShowErrorAsync("Моля, изберете ЦПО от списъка преди да заредите данни за налични свободни документи!");
                }
                else
                {
                    this.requestDocumentManagementsSource = (await this.ProviderDocumentRequestService.GetDocumentsControlDataAsync(this.idCandidateProvider.Value)).OrderBy(x => x.DocumentYear).ThenBy(d => d.TypeOfRequestedDocument.DocTypeOfficialNumber).ToList();

                    this.idCandidateProvider = null;

                    await this.requestDocumentManagementsGrid.Refresh();
                    this.StateHasChanged();
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
                ExportColumns.Add(new GridColumn() { Field = "Provider.LicenceNumber", HeaderText = "Лицензия", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Provider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AvailableCount", HeaderText = "Свободни", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"ControlDocument_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.requestDocumentManagementsGrid.ExportToPdfAsync(ExportProperties);
                requestDocumentManagementsGrid.PageSettings.PageSize = temp;
                await requestDocumentManagementsGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "Provider.LicenceNumber", HeaderText = "Лицензия", Width = "100", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Provider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AvailableCount", HeaderText = "Свободни", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"ControlDocument_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.requestDocumentManagementsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<RequestDocumentManagementControlModel> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(requestDocumentManagementsGrid, args.Data.EntityId).Result.ToString();
            }
        }
    }
}
