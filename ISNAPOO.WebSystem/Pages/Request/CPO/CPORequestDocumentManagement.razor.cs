using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.DocIO.DLS;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.CPO
{
    public partial class CPORequestDocumentManagement : BlazorBaseComponent
    {
        private SfGrid<RequestDocumentManagementVM> requestDocumentsGrid = new SfGrid<RequestDocumentManagementVM>();
        private CPORequestDocumentManagementModal cpoRequestDocumentManagementModal = new CPORequestDocumentManagementModal();
        private UploadRequestProtocolModal uploadRequestProtocolModal = new UploadRequestProtocolModal();

        private List<RequestDocumentManagementVM> requestDocumentsSource = new List<RequestDocumentManagementVM>();
        private IEnumerable<KeyValueVM> kvDocumentRequestReceiveTypeSource = new List<KeyValueVM>();
        private IEnumerable<DocumentSeriesVM> documentSeriesSource = new List<DocumentSeriesVM>();
        private IEnumerable<TypeOfRequestedDocumentVM> typeOfRequestedDocuments = new List<TypeOfRequestedDocumentVM>();

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.requestDocumentsSource = (await this.ProviderDocumentRequestService.GetAllRequestDocumentManagementsByIdCandidateProviderAndDocumentOperationReceivedAsync(this.UserProps.IdCandidateProvider)).OrderByDescending(x => x.IdRequestDocumentManagement).OrderByDescending(x => x.DocumentDate).ToList();
                this.kvDocumentRequestReceiveTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("DocumentRequestReceiveType");
                this.documentSeriesSource = await this.ProviderDocumentRequestService.GetAllDocumentSeriesAsync();
                this.typeOfRequestedDocuments = await this.ProviderDocumentRequestService.GetAllTypesOfRequestedDocumentAsync();
                await this.SetDataForGridEntries();
            }
        }

        private async Task SetDataForGridEntries()
        {
            this.SpinnerShow();

            foreach (var requestDocManagement in this.requestDocumentsSource)
            {
                var docSeries = this.documentSeriesSource.FirstOrDefault(x => x.IdTypeOfRequestedDocument == requestDocManagement.IdTypeOfRequestedDocument && x.Year == requestDocManagement.ReceiveDocumentYear);
                if (docSeries is not null)
                {
                    requestDocManagement.DocumentSeriesName = docSeries.SeriesName;
                }

                var receiveType = this.kvDocumentRequestReceiveTypeSource.FirstOrDefault(x => x.IdKeyValue == requestDocManagement.IdDocumentRequestReceiveType);
                if (receiveType is not null)
                {
                    requestDocManagement.DocumentRequestReceiveTypeName = receiveType.Name;
                }
            }

            await this.requestDocumentsGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerHide();
        }

        private async Task EditRequest(RequestDocumentManagementVM requestDocumentManagementVM)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.cpoRequestDocumentManagementModal.OpenModal(requestDocumentManagementVM, this.kvDocumentRequestReceiveTypeSource, this.documentSeriesSource);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OpenAddNewModal()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var candidateProvider = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);
                await this.cpoRequestDocumentManagementModal.OpenModal(new RequestDocumentManagementVM() { IdCandidateProvider = this.UserProps.IdCandidateProvider, CandidateProvider = candidateProvider }, this.kvDocumentRequestReceiveTypeSource, this.documentSeriesSource);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OpenUploadProtocolModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedRows = await this.requestDocumentsGrid.GetSelectedRecordsAsync();
                if (!selectedRows.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един ред, за да прикачите протокол!");
                    this.loading = false;
                    this.SpinnerHide();
                    return;
                }

                await this.uploadRequestProtocolModal.OpenModal(selectedRows);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadProtocolBtn(RequestDocumentManagementVM requestDocumentManagement)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var uploadedFile = requestDocumentManagement.RequestDocumentManagementUploadedFiles.FirstOrDefault();
                if (uploadedFile is not null && !string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<RequestDocumentManagementUploadedFile>(uploadedFile.IdRequestDocumentManagementUploadedFile);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<RequestDocumentManagementUploadedFile>(uploadedFile.IdRequestDocumentManagementUploadedFile);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, documentStream.MS!.ToArray());
                        }
                    }
                    else
                    {
                        var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                        await this.ShowErrorAsync(msg);
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterModalSubmit()
        {
            this.requestDocumentsSource = (await this.ProviderDocumentRequestService.GetAllRequestDocumentManagementsByIdCandidateProviderAndDocumentOperationReceivedAsync(this.UserProps.IdCandidateProvider)).OrderByDescending(x => x.IdRequestDocumentManagement).ToList();
            await this.SetDataForGridEntries();
        }

        private void CellInfoHandler(QueryCellInfoEventArgs<RequestDocumentManagementVM> args)
        {
            var requestDocumentManagement = this.requestDocumentsSource.FirstOrDefault(x => x.IdRequestDocumentManagement == args.Data.IdRequestDocumentManagement);
            var typeOfDoc = this.typeOfRequestedDocuments.FirstOrDefault(x => x.IdTypeOfRequestedDocument == requestDocumentManagement.IdTypeOfRequestedDocument);
            if (typeOfDoc is not null && typeOfDoc.HasSerialNumber)
            {
                if (!requestDocumentManagement.DocumentSerialNumbers.Any() || requestDocumentManagement.DocumentCount != requestDocumentManagement.DocumentSerialNumbers.Count)
                {
                    args.Cell.AddClass(new string[] { "row-red" });
                }
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = requestDocumentsGrid.PageSettings.PageSize;
                requestDocumentsGrid.PageSettings.PageSize = requestDocumentsSource.Count();
                await requestDocumentsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentCount", HeaderText = "Брой", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceiveDocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата на получаване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentRequestReceiveTypeName", HeaderText = "Получен от", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderRequestDocument.RequestNumber", HeaderText = "По заявка №", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPartner.ProviderOwner", HeaderText = "От друго ЦПО", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = Syncfusion.Blazor.Grids.PageOrientation.Landscape;
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
                ExportProperties.FileName = $"RequestDocumentManagement_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.requestDocumentsGrid.ExportToPdfAsync(ExportProperties);
                requestDocumentsGrid.PageSettings.PageSize = temp;
                await requestDocumentsGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", Width = "50", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentCount", HeaderText = "Брой", Width = "50", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceiveDocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата на получаване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentRequestReceiveTypeName", HeaderText = "Получен от", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderRequestDocument.RequestNumber", HeaderText = "По заявка №", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProviderPartner.ProviderOwner", HeaderText = "От друго ЦПО", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"RequestDocumentManagement_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.requestDocumentsGrid.ExportToExcelAsync(ExportProperties);
            }
        }
    }
}
