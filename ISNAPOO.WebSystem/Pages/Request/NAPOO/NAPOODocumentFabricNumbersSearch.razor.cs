using Data.Models.Data.DOC;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Request;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Request.NAPOO
{
    public partial class NAPOODocumentFabricNumbersSearch : BlazorBaseComponent
    {
        private SfGrid<DocumentSerialNumberVM> documentSerialNumbersGrid = new SfGrid<DocumentSerialNumberVM>();
        private ToastMsg toast = new ToastMsg();
        private NAPOODocumentFabricNumbersSearchModal napooDocumentFabricNumbersSearchModal = new NAPOODocumentFabricNumbersSearchModal();

        private List<DocumentSerialNumberVM> documentSerialNumbersSource = new List<DocumentSerialNumberVM>();
        private IEnumerable<KeyValueVM> kvDocumentOperationSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvDocumentReceiveTypeSource = new List<KeyValueVM>();
        private int idReceiveFromMONPrinting = 0;
        private int idReceiveFromOtherCPO = 0;
        private int idDocumentOperation = 0;
        private int idPrinted = 0;
        private int idDestroyed = 0;
        private int idCancelled = 0;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProviderDocumentRequestService ProviderDocumentRequestService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.kvDocumentOperationSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ActionType");
            this.idDocumentOperation = this.kvDocumentOperationSource.FirstOrDefault(x => x.KeyValueIntCode == "Submitted").IdKeyValue;
            this.idPrinted = this.kvDocumentOperationSource.FirstOrDefault(x => x.KeyValueIntCode == "Printed").IdKeyValue;
            this.idDestroyed = this.kvDocumentOperationSource.FirstOrDefault(x => x.KeyValueIntCode == "Destroyed").IdKeyValue;
            this.idCancelled = this.kvDocumentOperationSource.FirstOrDefault(x => x.KeyValueIntCode == "Cancelled").IdKeyValue;
            this.kvDocumentReceiveTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("DocumentRequestReceiveType");
            this.idReceiveFromMONPrinting = this.kvDocumentReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "PrintingHouse").IdKeyValue;
            this.idReceiveFromOtherCPO = this.kvDocumentReceiveTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "OtherCPO").IdKeyValue;
        }

        private async Task FilterBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.napooDocumentFabricNumbersSearchModal.OpenModal();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterFilterSubmit(List<DocumentSerialNumberVM> documentSerialNumbers)
        {
            this.documentSerialNumbersSource = documentSerialNumbers.ToList();
            await this.SetDataForGrid();

            await this.documentSerialNumbersGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task SetDataForGrid()
        {
            foreach (var docSerialNumber in this.documentSerialNumbersSource)
            {
                var operationType = this.kvDocumentOperationSource.FirstOrDefault(x => x.IdKeyValue == docSerialNumber.IdDocumentOperation);
                if (operationType is not null)
                {
                    docSerialNumber.DocumentOperationName = operationType.Name;
                }

                if (docSerialNumber.TypeOfRequestedDocument.DocumentSeries.Any())
                {
                    var docSeries = docSerialNumber.TypeOfRequestedDocument.DocumentSeries.FirstOrDefault(x => x.Year == docSerialNumber.ReceiveDocumentYear);
                    if (docSeries is not null)
                    {
                        docSerialNumber.DocumentSeriesName = docSeries.SeriesName;
                    }
                }

                if (docSerialNumber.RequestDocumentManagement.IdDocumentRequestReceiveType.HasValue)
                {
                    if (docSerialNumber.RequestDocumentManagement.IdDocumentRequestReceiveType == this.idReceiveFromMONPrinting)
                    {
                        docSerialNumber.DocumentReceivedFrom = "печатница";
                    }
                    else if (docSerialNumber.RequestDocumentManagement.IdDocumentRequestReceiveType == this.idReceiveFromOtherCPO)
                    {
                        docSerialNumber.DocumentReceivedFrom = docSerialNumber.RequestDocumentManagement.CandidateProviderPartner.ProviderOwner;
                    }
                }

                if (docSerialNumber.IdDocumentOperation == this.idDocumentOperation)
                {
                    if (docSerialNumber.RequestDocumentManagement.CandidateProviderPartner is not null)
                    {
                        docSerialNumber.DocumentReceivedFrom = docSerialNumber.RequestDocumentManagement.CandidateProviderPartner.ProviderOwner;
                    }
                }
                else if (docSerialNumber.IdDocumentOperation == this.idPrinted)
                {
                    docSerialNumber.DocumentReceivedFrom = await this.ProviderDocumentRequestService.GetClientNameByIdDocumentSerialNumberAsync(docSerialNumber.IdDocumentSerialNumber);
                }

                if (docSerialNumber.ClientCourseDocuments.All(x => x.CourseDocumentUploadedFiles.Any() && x.CourseDocumentUploadedFiles.All(y => !string.IsNullOrEmpty(y.UploadedFileName)))
                    || docSerialNumber.ValidationClientDocuments.All(x => x.ValidationDocumentUploadedFiles.Any() && x.ValidationDocumentUploadedFiles.All(y => !string.IsNullOrEmpty(y.UploadedFileName)))
                    || docSerialNumber.RequestDocumentManagement.RequestDocumentManagementUploadedFiles.All(x => !string.IsNullOrEmpty(x.UploadedFileName))
                    || docSerialNumber.RequestReport.ReportUploadedDocs.All(x => !string.IsNullOrEmpty(x.UploadedFileName)))
                {
                    docSerialNumber.HasUploadedFile = true;
                }
            }
        }

        private async Task DownloadUploadedFileBtn(DocumentSerialNumberVM serialNumber)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (serialNumber.IdDocumentOperation == this.idPrinted)
                {
                    if (serialNumber.ValidationClientDocuments.Any())
                    {
                        var model = serialNumber.ValidationClientDocuments.FirstOrDefault();
                        if (model.OldId.HasValue)
                        {
                            if (!model.ValidationDocumentUploadedFiles.Any())
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                            else
                            {
                                var docs = model.ValidationDocumentUploadedFiles;
                                if (docs.Any())
                                {
                                    foreach (var doc in docs)
                                    {
                                        if (!string.IsNullOrEmpty(doc.UploadedFileName))
                                        {
                                            var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                                            if (hasFile)
                                            {
                                                var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                                                if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                                {
                                                    await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                                }
                                                else
                                                {
                                                    await FileUtils.SaveAs(this.JsRuntime, doc.FileName, document.MS!.ToArray());
                                                }
                                            }
                                            else
                                            {
                                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                                await this.ShowErrorAsync(msg);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                    await this.ShowErrorAsync(msg);
                                }
                            }
                        }
                        else
                        {
                            var uploadedFile = model.ValidationDocumentUploadedFiles.FirstOrDefault();
                            if (uploadedFile is not null)
                            {
                                if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                                {
                                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(uploadedFile.IdValidationDocumentUploadedFile);
                                    if (hasFile)
                                    {
                                        var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(uploadedFile.IdValidationDocumentUploadedFile);
                                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                        {
                                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                        }
                                        else
                                        {
                                            await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, document.MS!.ToArray());
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
                                    var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                    await ShowErrorAsync(msg);
                                }
                            }
                            else
                            {
                                var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await ShowErrorAsync(msg);
                            }
                        }
                    }
                    else if (serialNumber.ClientCourseDocuments.Any())
                    {
                        var model = serialNumber.ClientCourseDocuments.FirstOrDefault();
                        if (model.OldId.HasValue)
                        {
                            if (!model.CourseDocumentUploadedFiles.Any())
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                            else
                            {
                                var docs = model.CourseDocumentUploadedFiles;
                                if (docs.Any())
                                {
                                    foreach (var doc in docs)
                                    {
                                        if (!string.IsNullOrEmpty(doc.UploadedFileName))
                                        {
                                            var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                                            if (hasFile)
                                            {
                                                var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                                                if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                                {
                                                    await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                                }
                                                else
                                                {
                                                    await FileUtils.SaveAs(this.JsRuntime, doc.FileName, document.MS!.ToArray());
                                                }
                                            }
                                            else
                                            {
                                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                                await this.ShowErrorAsync(msg);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                    await this.ShowErrorAsync(msg);
                                }
                            }
                        }
                        else
                        {
                            var uploadedFile = model.CourseDocumentUploadedFiles.FirstOrDefault();
                            if (uploadedFile is not null)
                            {
                                if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                                {
                                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(uploadedFile.IdCourseDocumentUploadedFile);
                                    if (hasFile)
                                    {
                                        var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(uploadedFile.IdCourseDocumentUploadedFile);
                                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                        {
                                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                        }
                                        else
                                        {
                                            await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, document.MS!.ToArray());
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
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                        }
                    }
                }
                else if (serialNumber.IdDocumentOperation == this.idDestroyed || serialNumber.IdDocumentOperation == this.idCancelled)
                {
                    if (serialNumber.RequestReport.ReportUploadedDocs.Any())
                    {
                        var  document = serialNumber.RequestReport.ReportUploadedDocs.FirstOrDefault();
                        if (document != null)
                        {
                            var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ReportUploadedDoc>(document.IdReportUploadedDoc);
                            if (hasFile)
                            {
                                var documentStream = await this.UploadFileService.GetUploadedFileAsync<ReportUploadedDoc>(document.IdReportUploadedDoc);

                                if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                                {
                                    await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                                }
                                else
                                {
                                    await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStream.MS!.ToArray());
                                }
                            }
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload");

                                await this.ShowErrorAsync(msg);
                            }
                        }
                    }
                }
                else
                {
                    if (serialNumber.RequestDocumentManagement.RequestDocumentManagementUploadedFiles.Any())
                    {
                        var uploadedFile = serialNumber.RequestDocumentManagement.RequestDocumentManagementUploadedFiles.FirstOrDefault();
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
                int temp = documentSerialNumbersGrid.PageSettings.PageSize;
                documentSerialNumbersGrid.PageSettings.PageSize = documentSerialNumbersSource.Count();
                await documentSerialNumbersGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SerialNumber", HeaderText = "Фабричен номер", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceiveDocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentOperationName", HeaderText = "Операция", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentReceivedFrom", HeaderText = "от/на", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"DocumentFabricNumbers_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.documentSerialNumbersGrid.ExportToPdfAsync(ExportProperties);
                documentSerialNumbersGrid.PageSettings.PageSize = temp;
                await documentSerialNumbersGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.NumberWithName", HeaderText = "Вид на документа", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentSeriesName", HeaderText = "Серия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SerialNumber", HeaderText = "Фабричен номер", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReceiveDocumentYear", HeaderText = "Година", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentDate", HeaderText = "Дата", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentOperationName", HeaderText = "Операция", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DocumentReceivedFrom", HeaderText = "от/на", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"DocumentFabricNumbers_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.documentSerialNumbersGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<DocumentSerialNumberVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(documentSerialNumbersGrid, args.Data.IdDocumentSerialNumber).Result.ToString();
            }
        }
    }
}
