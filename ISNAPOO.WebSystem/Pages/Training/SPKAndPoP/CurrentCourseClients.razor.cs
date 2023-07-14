using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
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
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CurrentCourseClients : BlazorBaseComponent
    {
        private SfGrid<ClientCourseVM> clientsGrid = new SfGrid<ClientCourseVM>();
        private CurrentCourseClientModal currentCourseClientModal = new CurrentCourseClientModal();
        private CourseClientFinishedDataModal courseClientFinishedDataModal = new CourseClientFinishedDataModal();
        private CourseClientsImportModal courseClientsImportModal = new CourseClientsImportModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();

        private List<ClientCourseVM> clientsSource = new List<ClientCourseVM>();
        private ClientCourseVM clientToDelete = new ClientCourseVM();
        private IEnumerable<KeyValueVM> courseStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> courseRequiredDocuments = new List<KeyValueVM>();
        private KeyValueVM kvCourseFinished = new KeyValueVM();
        private KeyValueVM kvEnteredInRegsiterStatusValue = new KeyValueVM();
        private KeyValueVM kvSubmittedStatusValue = new KeyValueVM();
        private KeyValueVM kvSPK = new KeyValueVM();

        [Parameter]
        public List<int> ClientCourseIds { get; set; }

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool EntryFromOldArchivedCourses { get; set; }

        [Parameter] public bool IsEditable { get; set; } = true;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
            this.kvCourseFinished = this.courseStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusFinished");
            this.finishedTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            this.courseRequiredDocuments = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType"))
                .Where(x => x.DefaultValue3 != null && x.DefaultValue3.Contains("CPO") && x.DefaultValue1 != null && x.DefaultValue1.Contains("Required_CPO")).ToList();

            this.kvEnteredInRegsiterStatusValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "EnteredInTheRegister");
            this.kvSubmittedStatusValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("ClientDocumentStatusType", "Submitted");

            this.kvSPK = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                await this.LoadClientsDataAsync();

                await this.clientsGrid.Refresh();
                this.StateHasChanged();

                this.SpinnerHide();
            }
        }

        private async Task LoadClientsDataAsync()
        {
            if (this.ClientCourseIds is null)
            {
                this.clientsSource = (await this.TrainingService.GetCourseClientsWithProtocolsAndDocsForDownloadByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
            }
            else
            {
                this.clientsSource = (await this.TrainingService.GetCourseClientsByListIdsAsync(this.ClientCourseIds)).ToList();
            }

            foreach (var client in this.clientsSource)
            {
                if (client.IdFinishedType.HasValue)
                {
                    var finishedType = this.finishedTypeSource.FirstOrDefault(x => x.IdKeyValue == client.IdFinishedType);
                    if (finishedType is not null)
                    {
                        client.FinishedTypeName = finishedType.Name;
                    }
                }
            }
        }

        private async Task AddClientBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.currentCourseClientModal.OpenModal(new ClientCourseVM() { IdCourse = this.CourseVM.IdCourse, Course = this.CourseVM, IdAssignType = this.CourseVM.IdAssignType }, this.CourseVM, this.clientsSource, this.courseStatusSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditClientBtn(ClientCourseVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clientCourse = await this.TrainingService.GetTrainingClientCourseByIdAsync(model.IdClientCourse);
                var isDocPresent = false;
                if (this.EntryFromOldArchivedCourses)
                {
                    isDocPresent = true;
                }
                else
                {
                    isDocPresent = await this.TrainingService.IsDocumentPresentAsync(clientCourse.IdClientCourse);
                }

                ConcurrencyInfo concurrencyInfoValue = null;
                if (this.GetUserRoles().Any(x => x.Contains("CPO")))
                {
                    concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(clientCourse.IdClientCourse, "TrainingClientCourse");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(clientCourse.IdClientCourse, "TrainingClientCourse");
                    }
                }

                await this.currentCourseClientModal.OpenModal(clientCourse, this.CourseVM, this.clientsSource, this.courseStatusSource, isDocPresent, concurrencyInfoValue, this.IsEditable);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteClientBtn(ClientCourseVM model)
        {
            this.clientToDelete = model;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteTrainingClientCourseByIdAsync(model.IdClientCourse);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    await this.LoadClientsDataAsync();

                    await this.clientsGrid.Refresh();
                    this.StateHasChanged();
                }
            }
        }

        private async Task DownloadClientUploadedDocumentAsync(int idUploadedDocument)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var uploadedFile = await this.TrainingService.GetClientRequiredDocumentById(idUploadedDocument);
                if (uploadedFile is null || string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;
                    await this.ShowErrorAsync(msg);
                }
                else
                {
                    if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                    {
                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ClientRequiredDocument>(idUploadedDocument);
                        if (hasFile)
                        {
                            var document = await this.UploadFileService.GetUploadedFileAsync<ClientRequiredDocument>(idUploadedDocument);
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
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DownloadProtocolUploadedDocumentAsync(int idProtocol)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var document = await this.TrainingService.GetCourseProtocolWithoutIncludesByIdAsync(idProtocol);
                if (document is not null && !string.IsNullOrEmpty(document.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseProtocol>(document.IdCourseProtocol);
                    if (hasFile)
                    {
                        var documentStrean = await this.UploadFileService.GetUploadedFileAsync<CourseProtocol>(document.IdCourseProtocol);
                        if (!string.IsNullOrEmpty(documentStrean.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStrean.FileNameFromOldIS, documentStrean.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileName, documentStrean.MS!.ToArray());
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

        private async Task UpdateAfterCourseClientModalSubmitAsync()
        {
            await this.LoadClientsDataAsync();

            await this.clientsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task FinishedDataBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedClients = await this.clientsGrid.GetSelectedRecordsAsync();
                if (!selectedClients.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един курсист от списъка!");
                    return;
                }

                this.courseClientFinishedDataModal.OpenModal(selectedClients, this.finishedTypeSource, this.CourseVM);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task CourseClientsTemplateDownloadBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var documentStream = this.TrainingService.GetCourseClientsTemplate();
                var fileName = "Kursisti-CPO.xlsx";

                await FileUtils.SaveAs(this.JsRuntime, fileName, documentStream.ToArray());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task CourseClientsImportBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.courseClientsImportModal.OpenModal(this.CourseVM, this.clientsSource.ToList());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void CheckForRequiredDocuments(QueryCellInfoEventArgs<ClientCourseVM> args)
        {
            if (args.Data is not null)
            {
                var client = this.clientsSource.FirstOrDefault(x => x.IdClientCourse == args.Data.IdClientCourse);
                if (client is not null)
                {
                    foreach (var document in this.courseRequiredDocuments)
                    {
                        if (!client.ClientRequiredDocuments.Any(x => x.IdCourseRequiredDocumentType == document.IdKeyValue && !string.IsNullOrEmpty(x.UploadedFileName)))
                        {
                            args.Cell.AddClass(new string[] { "color-elements" });
                        }
                    }
                }
            }
        }

        private async Task OpenFileWithDocumentBtn(ClientCourseVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var docs = model.ClientCourseDocuments.SelectMany(x => x.CourseDocumentUploadedFiles).ToList();
                if (!docs.Any())
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowErrorAsync(msg);
                }
                else
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
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenStatusHistoryBtn(ClientCourseVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clientCourseDoc = model.ClientCourseDocuments.FirstOrDefault();
                await this.documentStatusModal.OpenModal(clientCourseDoc.IdClientCourseDocument, "Course");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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
                ExportProperties.FileName = $"TrainingCourseClientsList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.clientsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"TrainingCourseClientsList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.clientsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "Indent", HeaderText = "ЕГН/ЛНЧ/ИДН", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "SecondName", HeaderText = "Презиме", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "FinishedTypeName", HeaderText = "Статус на завършване", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
