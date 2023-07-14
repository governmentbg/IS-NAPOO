using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.Common.Framework;
using Syncfusion.DocIO.DLS;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class CourseProtocolStateExamList : BlazorBaseComponent
    {
        private SfGrid<CourseProtocolVM> protocolsGrid = new SfGrid<CourseProtocolVM>();
        private CourseProtocolModal courseProtocolModal = new CourseProtocolModal();

        private IEnumerable<CourseProtocolVM> protocolsSource = new List<CourseProtocolVM>();
        private CourseProtocolVM protocolToDelete = new CourseProtocolVM();
        private string type = string.Empty;
        private string title = string.Empty;

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await this.HandleTokenData();
        }

        private async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();

                    await this.LoadProtocolsDataAsync();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        private async Task LoadProtocolsDataAsync()
        {
            this.title = "Държавни изпити";
            int idCourseType;
            switch (this.type)
            {
                case GlobalConstants.STATE_EXAM_SPK:
                    idCourseType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification")).IdKeyValue;
                    break;
                case GlobalConstants.STATE_EXAM_LC:
                    idCourseType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "CourseRegulation1And7")).IdKeyValue;
                    break;
                default:
                    this.title = "Изпити";
                    idCourseType = (await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession")).IdKeyValue;
                    break;
            }

            this.protocolsSource = await this.TrainingService.GetAllCourseProtocolsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
            this.protocolsSource = this.protocolsSource.Where(x => x.Course.IdTrainingCourseType == idCourseType).ToList();
        }

        private async Task AddCourseProtocolBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.courseProtocolModal.OpenModal(new CourseProtocolVM(), null, true);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task EditCourseProtocolBtn(CourseProtocolVM courseProtocol)
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

                await this.courseProtocolModal.OpenModal(protocol, concurrencyInfoValue, true, !courseProtocol.IsClientWithDocumentPresent);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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

                if (document != null)
                {
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseProtocol>(document.IdCourseProtocol);
                    if (hasFile)
                    {
                        var documentStream = await this.UploadFileService.GetUploadedFileAsync<CourseProtocol>(document.IdCourseProtocol);

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

                    await this.LoadProtocolsDataAsync();
                    this.StateHasChanged();
                }
            }
        }

        private async Task UpdateAfterModalSubmit()
        {
            await this.LoadProtocolsDataAsync();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = Syncfusion.Blazor.Grids.PageOrientation.Landscape;
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

            ExportColumns.Add(new GridColumn() { Field = "Course.CourseName", HeaderText = "Курс", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Course.Location.LocationName", HeaderText = "Населено място", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "Course.FormEducationName", HeaderText = "Форма на обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CoursePeriod", HeaderText = "Период на обучение", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CourseProtocolTypeName", HeaderText = "Вид на протокола", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CourseProtocolNumber", HeaderText = "Номер на протокол", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "CourseProtocolDate", HeaderText = "Дата", TextAlign = TextAlign.Left, Format = "dd.MM.yyyy" });

            return ExportColumns;
        }
    }
}
