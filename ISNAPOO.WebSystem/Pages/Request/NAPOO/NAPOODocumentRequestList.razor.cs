using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOODocumentRequestList : BlazorBaseComponent
    {
        private SfGrid<ProviderRequestDocumentVM> sfGrid = new SfGrid<ProviderRequestDocumentVM>();
        private NAPOOSummarizeRequestsModal napooSummarizeRequestsModal = new NAPOOSummarizeRequestsModal();

        private IEnumerable<ProviderRequestDocumentVM> providerRequestSource = new List<ProviderRequestDocumentVM>();
        private IEnumerable<KeyValueVM> kvRequestDocumetStatusSource = new List<KeyValueVM>();
        private string initialFilterValue = "Подадена";
        private List<ProviderRequestDocumentVM> selectedRequestsSource = new List<ProviderRequestDocumentVM>();

        [Inject]
        public IProviderDocumentRequestService ProdiverDocumentRequestService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                this.selectedRequestsSource.Clear();
                this.kvRequestDocumetStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RequestDocumetStatus");

                await this.SetProviderRequestData();

                await this.sfGrid.Refresh();
                this.StateHasChanged();

                this.SpinnerHide();
            }
        }

        private async Task SetProviderRequestData()
        {
            this.providerRequestSource = await this.ProdiverDocumentRequestService.GetAllDocumentRequestsAsync();
            
            foreach (var providerRequest in this.providerRequestSource)
            {
                if (providerRequest.IdStatus != 0)
                {
                    providerRequest.RequestStatus = this.kvRequestDocumetStatusSource.FirstOrDefault(x => x.IdKeyValue == providerRequest.IdStatus)?.Name;
                }
            }
        }

        private async Task RequestDeselected(RowDeselectEventArgs<ProviderRequestDocumentVM> args)
        {
            await this.HandleRowSelection();
        }

        private async Task RequestSelected(RowSelectEventArgs<ProviderRequestDocumentVM> args)
        {
            await this.HandleRowSelection();
        }

        private async Task SummarizeRequests()
        {
            bool hasPermission = await CheckUserActionPermission("ManageNAPOODocumentRequestData", false);
            if (!hasPermission) { return; }

            if (!this.selectedRequestsSource.Any())
            {
                await this.ShowErrorAsync("Моля, изберете заявка/заявки!");
                return;
            }

            if (this.selectedRequestsSource.Any(x => x.RequestStatus != "Подадена"))
            {
                await this.ShowErrorAsync("Моля, изберете заявка/заявки, които са на статус \"Подадена\"!");
                return;
            }

            foreach (var request in this.selectedRequestsSource)
            {
                if (this.selectedRequestsSource.Any(x => x.CurrentYear != request.CurrentYear))
                {
                    await this.ShowErrorAsync("Моля, изберете заявка/заявки, които се отнасят за една и съща година!");
                    return;
                }
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.napooSummarizeRequestsModal.OpenModal(this.selectedRequestsSource, this.providerRequestSource.ToList(), new NAPOORequestDocVM(), this.kvRequestDocumetStatusSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task HandleRowSelection()
        {
            this.selectedRequestsSource.Clear();
            this.selectedRequestsSource = await this.sfGrid.GetSelectedRecordsAsync();
        }

        private async Task UpdateAfterSummarizeSubmit()
        {
            this.SpinnerShow();

            await this.SetProviderRequestData();
            await this.sfGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = providerRequestSource.Count();
                await sfGrid.Refresh();
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
                ExportProperties.FileName = $"DocumentRequest_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "RequestNumber", HeaderText = "Заявка №", Width = "40", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestDate", HeaderText = "Дата", Width = "100", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Отговорно лице", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Position", HeaderText = "Длъжност", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentYear", HeaderText = "Година", Width = "70", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "NAPOORequestDoc.NAPOORequestNumber", HeaderText = "Обобщена заявка №", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequestStatus", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.FileName = $"DocumentRequest_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ProviderRequestDocumentVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdProviderRequestDocument).Result.ToString();
            }
        }
    }
}
