using ISNAPOO.Common.Constants;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Training.LegalCapacity
{
    public partial class LegalCapacityCurrentCourseClients : BlazorBaseComponent
    {
        private SfGrid<ClientCourseVM> clientsGrid = new SfGrid<ClientCourseVM>();
        private LegalCapacityCurrentCourseClientModal currentCourseClientModal = new LegalCapacityCurrentCourseClientModal();
        private CourseClientFinishedDataModal courseClientFinishedDataModal = new CourseClientFinishedDataModal();
        private CourseClientsImportModal courseClientsImportModal = new CourseClientsImportModal();

        private List<ClientCourseVM> clientsSource = new List<ClientCourseVM>();
        private ClientCourseVM clientToDelete = new ClientCourseVM();
        private IEnumerable<KeyValueVM> courseStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> finishedTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> courseRequiredDocuments = new List<KeyValueVM>();
        private KeyValueVM kvCourseFinished = new KeyValueVM();

        [Parameter]
        public CourseVM CourseVM { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
            this.kvCourseFinished = this.courseStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "CourseStatusFinished");
            this.finishedTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            this.courseRequiredDocuments = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ClientCourseDocumentType"))
                .Where(x => x.DefaultValue3 != null && x.DefaultValue3.Contains("CPO") && x.DefaultValue1 != null && x.DefaultValue1.Contains("Required_CPO")).ToList();
            await this.LoadClientsDataAsync();
        }

        private async Task LoadClientsDataAsync()
        {
            this.clientsSource = (await this.TrainingService.GetCourseClientsWithProtocolsAndDocsForDownloadByIdCourseAsync(this.CourseVM.IdCourse)).ToList();
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
                var isDocPresent = await this.TrainingService.IsDocumentPresentAsync(clientCourse.IdClientCourse);

                ConcurrencyInfo concurrencyInfoValue = null;
                if (this.GetUserRoles().Any(x => x.Contains("CPO")))
                {
                    concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(clientCourse.IdClientCourse, "TrainingClientCourse");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(clientCourse.IdClientCourse, "TrainingClientCourse");
                    }
                }

                await this.currentCourseClientModal.OpenModal(clientCourse, this.CourseVM, this.clientsSource, this.courseStatusSource, isDocPresent, true, concurrencyInfoValue);
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
            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
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
