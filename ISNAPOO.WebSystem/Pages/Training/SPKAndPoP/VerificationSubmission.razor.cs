using Data.Models.Data.Training;
using Ionic.Zip;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class VerificationSubmission : BlazorBaseComponent
    {
        private SfGrid<ClientCourseDocumentVM> documentsGrid = new SfGrid<ClientCourseDocumentVM>();
        private SubmissionCommentModal submissionCommentModal = new SubmissionCommentModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();

        private List<ClientCourseDocumentVM> documentsSource = new List<ClientCourseDocumentVM>();
        private KeyValueVM kvDocumentStatusNotSubmitted = new KeyValueVM();
        private KeyValueVM kvDocumentStatusReturned = new KeyValueVM();

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

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
            this.kvDocumentStatusNotSubmitted = await this.DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "NotSubmitted");
            this.kvDocumentStatusReturned = await this.DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Returned");
            await this.LoadDocumentsDataAsync();
        }

        private async Task LoadDocumentsDataAsync()
        {
            this.documentsSource = (await this.TrainingService.GetClientCourseDocumentsByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
            this.StateHasChanged();
        }

        private async Task FileInForVerificationBtn()
        {
            var selectedDocs = await this.documentsGrid.GetSelectedRecordsAsync();
            if (!selectedDocs.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един ред от списъка преди да подадете за проверка към НАПОО!");
                return;
            }

            if (!selectedDocs.All(x => x.IdDocumentStatus == this.kvDocumentStatusReturned.IdKeyValue || x.IdDocumentStatus == this.kvDocumentStatusNotSubmitted.IdKeyValue))
            {
                await this.ShowErrorAsync($"Моля, изберете само редове от списъка, които са на статус '{this.kvDocumentStatusNotSubmitted.Name}'/'{this.kvDocumentStatusReturned.Name}', за да можете да подадете за проверка към НАПОО!");
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

                    this.submissionCommentModal.OpenModal(GlobalConstants.RIDPK_OPERATION_FILE_IN, selectedDocs);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OpenFileWithDocumentBtn(ClientCourseDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenStatusHistoryBtn(ClientCourseDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.documentStatusModal.OpenModal(model.IdClientCourseDocument, "Course");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateDocumentsDataAfterDocumentSubmissionAsync()
        {
            await this.LoadDocumentsDataAsync();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();

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

                await this.documentsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"DocumentList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.documentsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "DocumentRegNo", HeaderText = "Регистрационен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentDateAsStr", HeaderText = "Дата", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentSerialNumber.SerialNumber", HeaderText = "Фабричен номер", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TypeOfRequestedDocument.DocTypeName", HeaderText = "Вид на документа", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "DocumentStatusValue", HeaderText = "Статус на документа", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
