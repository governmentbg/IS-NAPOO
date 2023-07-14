using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOOSummarizedRequestDocumentList : BlazorBaseComponent
    {
        private SfGrid<NAPOORequestDocVM> napooRequestDocumentsGrid = new SfGrid<NAPOORequestDocVM>();
        private NAPOOSummarizeRequestsModal napooSummarizeRequestsModal = new NAPOOSummarizeRequestsModal();
        private ToastMsg toast = new ToastMsg();

        private List<NAPOORequestDocVM> napooRequestDocumentsSource = new List<NAPOORequestDocVM>();
        private List<ProviderRequestDocumentVM> providerRequestSource = new List<ProviderRequestDocumentVM>();
        //private List<ProviderRequestDocumentVM> providerRequestSource = new List<ProviderRequestDocumentVM>();
        private IEnumerable<KeyValueVM> kvRequestDocumetStatusSource = new List<KeyValueVM>();

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
                this.napooRequestDocumentsSource = (await this.ProviderDocumentRequestService.GetAllNAPOORequestDocumentsAsync()).OrderByDescending(x => x.RequestDate).ToList();
                this.kvRequestDocumetStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus");
                await this.SetProviderRequestData();

                await this.napooRequestDocumentsGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private async Task UpdateAfterSummarizeSubmit()
        {
            this.SpinnerShow();

            this.napooRequestDocumentsSource = (await this.ProviderDocumentRequestService.GetAllNAPOORequestDocumentsAsync()).OrderByDescending(x => x.RequestDate).ToList();
            await this.SetProviderRequestData();
            await this.napooRequestDocumentsGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerHide();
        }

        private async Task EditRequest(NAPOORequestDocVM nAPOORequestDocVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewSummarizedRequestDocumentData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                foreach (var doc in nAPOORequestDocVM.ProviderRequestDocuments)
                {
                    doc.CandidateProvider = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(doc.IdCandidateProvider);
                }

                await this.napooSummarizeRequestsModal.OpenModal(nAPOORequestDocVM.ProviderRequestDocuments.ToList(), this.providerRequestSource, nAPOORequestDocVM, this.kvRequestDocumetStatusSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SetProviderRequestData()
        {
            this.providerRequestSource = (await this.ProviderDocumentRequestService.GetAllDocumentRequestsAsync()).ToList();
            foreach (var providerRequest in this.providerRequestSource)
            {
                providerRequest.RequestStatus = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.IdKeyValue == providerRequest.IdStatus)?.Name;
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                int temp = napooRequestDocumentsGrid.PageSettings.PageSize;
                napooRequestDocumentsGrid.PageSettings.PageSize = napooRequestDocumentsSource.Count();
                await napooRequestDocumentsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "NAPOORequestNumber", HeaderText = "Номер на обобщена заявка", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestDate", HeaderText = "Дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "IsSentAsStr", HeaderText = "Изпратена към печатница", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "IsNotificationSentAsStr", HeaderText = "Изпратени писма", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"SummarizedRequestDocument_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.napooRequestDocumentsGrid.ExportToPdfAsync(ExportProperties);
                napooRequestDocumentsGrid.PageSettings.PageSize = temp;
                await napooRequestDocumentsGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "NAPOORequestNumber", HeaderText = "№ на обобщена заявка", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestDate", HeaderText = "Дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "IsSentAsStr", HeaderText = "Изпратена към печатница", Width = "40", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "IsNotificationSentAsStr", HeaderText = "Изпратени писма", Width = "40", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"SummarizedRequestDocument_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.napooRequestDocumentsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<NAPOORequestDocVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(napooRequestDocumentsGrid, args.Data.IdNAPOORequestDoc).Result.ToString();
            }
        }
    }
}
