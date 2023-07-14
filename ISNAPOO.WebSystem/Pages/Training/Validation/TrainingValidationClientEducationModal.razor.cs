using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class TrainingValidationClientEducationModal : BlazorBaseComponent
    {
        private SfGrid<ValidationClientRequiredDocumentVM> sfGrid = new SfGrid<ValidationClientRequiredDocumentVM>();
        private IEnumerable<ValidationClientRequiredDocumentVM> documentsSource;
        private ValidationClientDocumentModal validationEducationModal = new ValidationClientDocumentModal();
        private ValidationClientRequiredDocumentVM documentToDelete = new ValidationClientRequiredDocumentVM();


        [Parameter]
        public ValidationClientVM ClientVM { get; set; }
        [Parameter]
        public bool IsEditable { get; set; } = true;

        [Inject]
        ITrainingService TrainingService { get; set; }
        [Inject]
        IUploadFileService UploadFileService { get; set; }
        [Inject]
        IJSRuntime JsRuntime { get; set; }
        [Inject]
        ILocService LocService { get; set; }
        protected override async Task OnInitializedAsync()
        {
            this.documentsSource = await TrainingService.GetAllValidationRequiredDocumentsByIdClient(ClientVM.IdValidationClient);
        }
        private void OpenNewModal()
        {
            this.validationEducationModal.OpenModal(new ValidationClientRequiredDocumentVM() { IdValidationClient = ClientVM.IdValidationClient });
        }
        private void EditDocumentBtn(ValidationClientRequiredDocumentVM model)
        {
            this.validationEducationModal.OpenModal(model);
        }
        private async Task CallBackAfterSubmit()
        {
            this.documentsSource = await TrainingService.GetAllValidationRequiredDocumentsByIdClient(ClientVM.IdValidationClient);
            StateHasChanged();
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = this.sfGrid.PageSettings.PageSize;
                this.sfGrid.PageSettings.PageSize = this.documentsSource.Count();
                await this.sfGrid.Refresh();
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
                this.sfGrid.PageSettings.PageSize = temp;
                await this.sfGrid.Refresh();
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
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ValidationClientRequiredDocumentVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(this.sfGrid, args.Data.IdValidationClientRequiredDocument).Result.ToString();
            }
        }

        private async Task OnDownloadClick(ValidationClientRequiredDocumentVM ValidationClientRequiredDocumentVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!string.IsNullOrEmpty(ValidationClientRequiredDocumentVM.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationClientRequiredDocument>(ValidationClientRequiredDocumentVM.IdValidationClientRequiredDocument);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<ValidationClientRequiredDocument>(ValidationClientRequiredDocumentVM.IdValidationClientRequiredDocument);
                        if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                        {
                            await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                        }
                        else
                        {
                            await FileUtils.SaveAs(this.JsRuntime, ValidationClientRequiredDocumentVM.FileName, documentStream.MS!.ToArray());
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
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteDocument(ValidationClientRequiredDocumentVM ValidationClientRequiredDocumentVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.documentToDelete = ValidationClientRequiredDocumentVM;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {

                var resultContext = await this.TrainingService.DeleteValidationClientRequiredDocumentAsync(this.documentToDelete);

                if (resultContext.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

                    this.documentsSource = await this.TrainingService.GetAllValidationRequiredDocumentsByIdClient(this.ClientVM.IdValidationClient);


                    this.StateHasChanged();
                }
            }
        }

    }
}
