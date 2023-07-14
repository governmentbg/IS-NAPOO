using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.ViewModels.Common;
using Data.Models.Common;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Common.Constants;
using Syncfusion.PdfExport;
using ISNAPOO.Core.Contracts.Common;
using Data.Models.Data.Common;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class EventLogList : BlazorBaseComponent
    {
        private IEnumerable<EventLogVM> eventLogs;
        private SfGrid<EventLogVM> eventLogsGrid = new SfGrid<EventLogVM>();
        private EventLogFilterModal eventLogFilterModal = new EventLogFilterModal();

        [Inject]
        public IEventLogService eventLogService { get; set; }

        //[Inject]
        //public IJSRuntime JsRuntime { get; set; }

        //[Inject]
        //public IDataSourceService DataSourceService { get; set; }

        //[Parameter]
        //public EventCallback<bool> CallbackAfterSubmit { get; set; }


        protected override async Task OnInitializedAsync()
        {

        }

        private async Task LoadEventLogsDataAsync()
        {
            this.eventLogs = await this.eventLogService.GetAllEventLogsAsync();

            this.StateHasChanged();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                await this.LoadEventLogsDataAsync();
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<EventLogVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(eventLogsGrid, args.Data.idEventLog).Result.ToString();
            }
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "eventLogsGrid_pdfexport")
            {
                int temp = eventLogsGrid.PageSettings.PageSize;
                eventLogsGrid.PageSettings.PageSize = eventLogs.Count();
                await eventLogsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "EventDate", HeaderText = "Дата на действието", Format = "d/M/yyyy г. HH:mm:ss", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "", HeaderText = "Модул", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EventAction", HeaderText = "Действие", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EntityName", HeaderText = "Таблица", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonName", HeaderText = "Потребител", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EntityID", HeaderText = "Обект на действието", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EventMessage", HeaderText = "Допълнителна информация", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "IP", HeaderText = "IP адрес", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "BrowserInformation", HeaderText = "Браузър", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentMenu", HeaderText = "Меню", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentUrl", HeaderText = "Линк", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"EventLogs_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".pdf";

                await this.eventLogsGrid.ExportToPdfAsync(ExportProperties);
                eventLogsGrid.PageSettings.PageSize = temp;
                await eventLogsGrid.Refresh();
            }
            else if (args.Item.Id == "eventLogsGrid_excelexport")
            {
                this.eventLogsGrid.AllowExcelExport = true;
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "EventDate", HeaderText = "Дата на действието", Format = "d/M/yyyy г. HH:mm:ss", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "", HeaderText = "Модул", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EventAction", HeaderText = "Действие", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EntityName", HeaderText = "Таблица", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonName", HeaderText = "Потребител", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EntityID", HeaderText = "Обект на действието", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "EventMessage", HeaderText = "Допълнителна информация", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "IP", HeaderText = "IP адрес", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "BrowserInformation", HeaderText = "Браузър", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentMenu", HeaderText = "Меню", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CurrentUrl", HeaderText = "Линк", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
       
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"EventLogs_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}" + ".xlsx";
                await this.eventLogsGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        private async Task OpenFilterEventModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.eventLogFilterModal.OpenModal();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        private async Task OnFilterModalSubmit(List<EventLogVM> eventLogs)
        {


            this.eventLogs = eventLogs.ToList();           
            this.eventLogsGrid.DataSource = this.eventLogs;
            StateHasChanged();
        }
    }
}
