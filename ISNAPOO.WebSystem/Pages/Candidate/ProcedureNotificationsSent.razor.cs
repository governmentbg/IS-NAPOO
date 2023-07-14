using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Common.Notifications;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ProcedureNotificationsSent : BlazorBaseComponent
    {
        private SfGrid<NotificationVM> notificationsGrid = new SfGrid<NotificationVM>();
        private NotificationModal notificationModal = new NotificationModal();

        private IEnumerable<NotificationVM> notificationsSource = new List<NotificationVM>();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

        [Inject]
        private INotificationService NotificationService { get; set; }

        public async Task LoadDataAsync()
        {
            this.notificationsSource = await this.NotificationService.GetAllNotificationsByIdStartedProcedureAsync(this.CandidateProviderVM.IdStartedProcedure.Value);

            await this.notificationsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task ReviewNotification(NotificationVM notification)
        {
            bool hasPermission = await CheckUserActionPermission("ViewNotificationData", false);
            if (!hasPermission) { return; }

            await this.notificationModal.OpenModal(notification);
        }

        private void CellInfoHandler(QueryCellInfoEventArgs<NotificationVM> args)
        {

            if (args.Data.StatusNotificationName.ToLower() == "непрочетено")
            {
                args.Cell.AddClass(new string[] { "text-danger" });
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdfexport"))
            {
                int temp = this.notificationsGrid.PageSettings.PageSize;
                this.notificationsGrid.PageSettings.PageSize = this.notificationsSource.Count();
                await this.notificationsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "About", HeaderText = "Относно", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "NotificationText", HeaderText = "Коментар", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SendDate", HeaderText = "Дата на изпращане", Width = "80", Format = "dd.MM.yyyy HH:mm:ss", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonFrom.FullName", HeaderText = "Изпратено от", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonTo.FullName", HeaderText = "Изпратено до", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusNotificationName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReviewDate", HeaderText = "Дата на преглед", Width = "80", Format = "dd.MM.yyyy HH:mm:ss", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"Notification_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.notificationsGrid.ExportToPdfAsync(ExportProperties);
                this.notificationsGrid.PageSettings.PageSize = temp;
                await this.notificationsGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Notification_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.notificationsGrid.ExportToExcelAsync(ExportProperties);
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<NotificationVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(this.notificationsGrid, args.Data.IdNotification).Result.ToString();
            }
        }
    }
}
