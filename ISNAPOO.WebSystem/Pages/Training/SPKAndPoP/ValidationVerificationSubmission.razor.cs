using Data.Models.Data.DOC;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class ValidationVerificationSubmission : BlazorBaseComponent
    {
        private SfGrid<ValidationClientDocumentVM> documentsGrid = new SfGrid<ValidationClientDocumentVM>();
        private SubmissionCommentModal submissionCommentModal = new SubmissionCommentModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();

        private List<ValidationClientDocumentVM> documentsSource = new List<ValidationClientDocumentVM>();
        private KeyValueVM kvDocumentStatusNotSubmitted = new KeyValueVM();
        private KeyValueVM kvDocumentStatusReturned = new KeyValueVM();
        private List<ValidationClientDocumentVM> selectedDocs;

        [Parameter]
        public ValidationClientVM ClientVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            kvDocumentStatusNotSubmitted = await DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
            kvDocumentStatusReturned = await DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
            await LoadDocumentsDataAsync();
        }

        private async Task LoadDocumentsDataAsync()
        {
            documentsSource = (await TrainingService.GetValidationClientDocumentsByIdValidationClientAsync(ClientVM.IdValidationClient)).ToList();
            StateHasChanged();
        }

        private async Task FileInForVerificationBtn()
        {
            var selectedDocs = await documentsGrid.GetSelectedRecordsAsync();
            if (!selectedDocs.Any())
            {
                await ShowErrorAsync("Моля, изберете поне един ред от списъка преди да подадете за проверка към НАПОО!");
                return;
            }

            if (!selectedDocs.All(x => x.IdDocumentStatus == kvDocumentStatusReturned.IdKeyValue || x.IdDocumentStatus == kvDocumentStatusNotSubmitted.IdKeyValue))
            {
                await ShowErrorAsync($"Моля, изберете само редове от списъка, които са на статус '{kvDocumentStatusNotSubmitted.Name}'/'{kvDocumentStatusReturned.Name}', за да можете да подадете за проверка към НАПОО!");
                return;
            }

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да подадете избраните документи за проверка към НАПОО?");
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    this.selectedDocs = selectedDocs.ToList();
                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_FILE_IN);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OpenFileWithDocumentBtn(ValidationClientDocumentVM model)
        {
            SpinnerShow();

            if (loading)
            {
                return;
            }
            try
            {
                loading = true;

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
            finally
            {
                loading = false;
            }

            SpinnerHide();
        }

        private async Task OpenStatusHistoryBtn(ValidationClientDocumentVM model)
        {
            SpinnerShow();

            if (loading)
            {
                return;
            }
            try
            {
                loading = true;

                await documentStatusModal.OpenModal(model.IdValidationClientDocument, "Validation");
            }
            finally
            {
                loading = false;
            }

            SpinnerHide();
        }

        private async Task UpdateDocumentsDataAfterDocumentSubmissionAsync()
        {
            await LoadDocumentsDataAsync();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();

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
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await documentsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = SetGridColumnsForExport();
                await documentsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "DocumentRegNo", HeaderText = "Регистрационен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentDateAsStr", HeaderText = "Дата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentSerialNumber.SerialNumber", HeaderText = "Фабричен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.DocTypeName", HeaderText = "Вид на документа", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ValidationClient.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentStatusValue", HeaderText = "Статус на документа", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
