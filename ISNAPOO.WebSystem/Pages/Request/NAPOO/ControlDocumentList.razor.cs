using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class ControlDocumentList : BlazorBaseComponent
    {
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private SfGrid<RequestDocumentManagementControlModel> requestDocumentManagementsGrid = new SfGrid<RequestDocumentManagementControlModel>();
        private NAPOOTypeOfRequestedDocumentReferenceModal napooTypeOfRequestedDocumentReferenceModal = new NAPOOTypeOfRequestedDocumentReferenceModal();

        private List<RequestDocumentManagementControlModel> requestDocumentManagementsSource = new List<RequestDocumentManagementControlModel>();
        private IEnumerable<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private int? idCandidateProvider = null;

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                if (this.UserProps.IdCandidateProvider == 0)
                {
                    this.candidateProvidersSource = (await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO")).ToList();
                }
                else
                {
                    this.requestDocumentManagementsSource = (await this.ProviderDocumentRequestService.GetDocumentsControlDataAsync(this.UserProps.IdCandidateProvider)).OrderBy(d => d.DocumentYear).ThenBy(d => d.TypeOfRequestedDocument.DocTypeOfficialNumber).ToList();

                    await this.requestDocumentManagementsGrid.Refresh();
                }

                this.StateHasChanged();
            }
        }

        private async Task OpenDocumentSerialNumberModal(RequestDocumentManagementControlModel requestDocumentManagement)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.napooTypeOfRequestedDocumentReferenceModal.OpenModal(requestDocumentManagement);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
                    await this.ShowErrorAsync("Моля, изберете ЦПО от списъка преди да заредите данни за налични документи!");
                }
                else
                {
                    this.requestDocumentManagementsSource = (await this.ProviderDocumentRequestService.GetDocumentsControlDataAsync(this.idCandidateProvider.Value)).OrderBy(d => d.DocumentYear).ThenBy(d => d.TypeOfRequestedDocument.DocTypeOfficialNumber).ToList();

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
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceivedCount", HeaderText = "Получени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "HandedOverCount", HeaderText = "Предадени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PrintedCount", HeaderText = "Издадени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CancelledCount", HeaderText = "Анулирани", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DestroyedCount", HeaderText = "Унищожени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AvailableCount", HeaderText = "Налични", Width = "80", TextAlign = TextAlign.Left });
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
                ExportColumns.Add(new GridColumn() { Field = "Provider.LicenceNumber", HeaderText = "Лицензия", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Provider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceivedCount", HeaderText = "Получени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "HandedOverCount", HeaderText = "Предадени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PrintedCount", HeaderText = "Издадени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CancelledCount", HeaderText = "Анулирани", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DestroyedCount", HeaderText = "Унищожени", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AvailableCount", HeaderText = "Налични", Width = "80", TextAlign = TextAlign.Left });
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
