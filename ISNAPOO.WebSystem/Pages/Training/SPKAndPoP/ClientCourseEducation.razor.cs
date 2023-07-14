using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.Training;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.Contracts.Common;
using Microsoft.JSInterop;
using ISNAPOO.WebSystem.Resources;
using Syncfusion.PdfExport;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using Data.Models.Data.Training;
using Syncfusion.Blazor.Inputs;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    partial class ClientCourseEducation : BlazorBaseComponent
    {
        private ToastMsg toast;
        private SfGrid<ClientRequiredDocumentVM> sfGrid = new SfGrid<ClientRequiredDocumentVM>();
        private IEnumerable<ClientRequiredDocumentVM> documentsSource;
        private ClientCourseEducationModal clientCourseEducationModal = new ClientCourseEducationModal();
        private ClientRequiredDocumentVM documentToDelete = new ClientRequiredDocumentVM();

        [Parameter]
        public EventCallback CallbackAfterDocUpload { get; set; }

        [Parameter]
        public ClientCourseVM ClientCourseVM { get; set; }

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter]
        public bool IsEditEnabled { get; set; }

        #region Inject
        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }
        #endregion

        protected override async Task OnInitializedAsync()
        {
            await this.LoadDataAsync();
        }

        public async Task LoadDataAsync()
        {
            this.documentsSource = await this.TrainingService.GetAllClientRequiredDocumentsByIdClientCourse(this.ClientCourseVM.IdClientCourse);
            this.StateHasChanged();
        }
        private void OpenNewModal()
        {
            clientCourseEducationModal.OpenModal(new ClientRequiredDocumentVM() { IdClientCourse = ClientCourseVM.IdClientCourse, IdCourse = CourseVM.IdCourse });
        }
        private void EditDocumentBtn(ClientRequiredDocumentVM model)
        {
            clientCourseEducationModal.OpenModal(model);
        }
        private async Task CallBackAfterSubmit()
        {
            this.documentsSource = await this.TrainingService.GetAllClientRequiredDocumentsByIdClientCourse(this.ClientCourseVM.IdClientCourse);
            this.StateHasChanged();

            await this.CallbackAfterDocUpload.InvokeAsync();
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = documentsSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CourseRequiredDocumentTypeName", HeaderText = "Вид на документа", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Description", HeaderText = "Описание на документа", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "UploadedFileName", HeaderText = "Прикачен файл", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreationDate", HeaderText = "Дата на прикачване", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreatePersonName", HeaderText = "Прикачено от", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"ClientCourseEducation_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "CourseRequiredDocumentTypeName", HeaderText = "Вид на документа", Width = "250", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Description", HeaderText = "Описание на документа", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "UploadedFileName", HeaderText = "Прикачен файл", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreationDate", HeaderText = "Дата на прикачване", Width = "120", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreatePersonName", HeaderText = "Прикачено от", Width = "150", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"ClientCourseEducation_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ClientRequiredDocumentVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdClientRequiredDocument).Result.ToString();
            }
        }
        private async Task DeleteDocument(ClientRequiredDocumentVM clientRequiredDocumentVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.documentToDelete = clientRequiredDocumentVM;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {

                var resultContext = await this.TrainingService.DeleteClientRequiredDocumentAsync(this.documentToDelete);

                if (resultContext.HasErrorMessages)
                {
                    this.toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                    await this.toast.sfErrorToast.ShowAsync();
                }
                else
                {
                    this.toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                    await this.toast.sfSuccessToast.ShowAsync();

                    this.documentsSource = await this.TrainingService.GetAllClientRequiredDocumentsByIdClientCourse(this.ClientCourseVM.IdClientCourse);


                    this.StateHasChanged();

                    await this.CallbackAfterDocUpload.InvokeAsync();
                }
            }
        }
        private async Task OnDownloadClick(ClientRequiredDocumentVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(model.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ClientRequiredDocument>(model.IdClientRequiredDocument);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<ClientRequiredDocument>(model.IdClientRequiredDocument);
                        if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, model.FileName, document.MS!.ToArray());
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

    }
}
