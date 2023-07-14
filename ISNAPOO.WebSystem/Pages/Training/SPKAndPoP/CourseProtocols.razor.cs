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
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseProtocols : BlazorBaseComponent
    {
        private SfGrid<CourseProtocolVM> protocolsGrid = new SfGrid<CourseProtocolVM>();
        private CourseProtocolModal courseProtocolModal = new CourseProtocolModal();

        private IEnumerable<CourseProtocolVM> protocolsSource = new List<CourseProtocolVM>();
        private CourseProtocolVM protocolToDelete = new CourseProtocolVM();

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Parameter] 
        public bool IsEditable { get; set; } = true;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                this.protocolsSource = await this.TrainingService.GetAllCourseProtocolsByIdCourseAsync(this.CourseVM.IdCourse);

                await this.protocolsGrid.Refresh();
                this.StateHasChanged();

                this.SpinnerHide();
            }
        }

        private async Task AddProtocolBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var model = new CourseProtocolVM()
                {
                    IdCourse = this.CourseVM.IdCourse,
                    Course = this.CourseVM
                };

                await this.courseProtocolModal.OpenModal(model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditProtocolBtn(CourseProtocolVM courseProtocol)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(courseProtocol.IdCourseProtocol, "TrainingCourseProtocol");
                if (concurrencyInfoValue == null)
                {
                    await this.AddEntityIdAsCurrentlyOpened(courseProtocol.IdCourseProtocol, "TrainingCourseProtocol");
                }

                var protocol = await this.TrainingService.GetCourseProtocolByIdAsync(courseProtocol.IdCourseProtocol);

                await this.courseProtocolModal.OpenModal(protocol, concurrencyInfoValue, false, !courseProtocol.IsClientWithDocumentPresent);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task DeleteProtocolBtn(CourseProtocolVM courseProtocol)
        {
            this.protocolToDelete = courseProtocol;

            bool isConfirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да изтриете избрания запис?");
            if (isConfirmed)
            {
                var result = await this.TrainingService.DeleteCourseProtocolByIdAsync(courseProtocol.IdCourseProtocol);
                if (result.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, result.ListErrorMessages));
                }
                else
                {
                    await this.ShowSuccessAsync(string.Join(Environment.NewLine, result.ListMessages));

                    this.protocolsSource = await this.TrainingService.GetAllCourseProtocolsByIdCourseAsync(this.CourseVM.IdCourse);
                    this.StateHasChanged();
                }
            }
        }

        private async Task OnDownloadClick(string fileName)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var document = this.protocolsSource.FirstOrDefault(x => x.FileName == fileName);
                if (!string.IsNullOrEmpty(document.UploadedFileName))
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseProtocol>(document.IdCourseProtocol);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<CourseProtocol>(document.IdCourseProtocol);
                        if (!string.IsNullOrEmpty(document.UploadedFileName))
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
            this.protocolsSource = await this.TrainingService.GetAllCourseProtocolsByIdCourseAsync(this.CourseVM.IdCourse);
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
                ExportProperties.FileName = $"CourseProtocolList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.protocolsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"CourseProtocolList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;

                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.protocolsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "CourseProtocolTypeName", HeaderText = "Вид на протокола", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CourseProtocolNumber", HeaderText = "Номер на протокол", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CourseProtocolDate", HeaderText = "Дата", TextAlign = TextAlign.Left, Format = "dd.MM.yyyy" });

            return ExportColumns;
        }
    }
}
