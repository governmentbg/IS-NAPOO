using System;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Common.Notifications
{
    public partial class NotificationList : BlazorBaseComponent
    {

        [Inject]
        INotificationService notificationService { get; set; }
        [Inject]
        ICommonService CommonService { get; set; }

        IEnumerable<NotificationVM> dataSource;
        SfGrid<NotificationVM> currentGrid;
        NotificationVM model = new NotificationVM();


        int idPerson = 0;

        NotificationModal editModal = new NotificationModal();

        private string prevId = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            this.idPerson = this.UserProps.IdPerson;
            this.dataSource = await this.notificationService.GetAllNotificationsByPersonToAsync(this.idPerson, false);
        }

        protected override async void OnParametersSet()
        {
            if (!prevId.Equals(Token) && Token != null)
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                var id = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(x => x.Key.Equals("idNotification"));
                var vm = dataSource.Where(x => x.IdNotification == int.Parse(id.Value.ToString())).First();
                await SelectedRow(vm);
                prevId = Token;
            }
        }

        private async Task NotificationDoubleClickHandler(RecordDoubleClickEventArgs<NotificationVM> args)
        {
            bool hasPermission = await CheckUserActionPermission("ViewNotificationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.model = await this.notificationService.GetNotificationAndChangeStatusByIdAsync(args.RowData.IdNotification);
                this.dataSource = await this.notificationService.GetAllNotificationsByPersonToAsync(this.idPerson, false);
                await currentGrid.Refresh();

                await this.editModal.OpenModal(this.model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SelectedRow(NotificationVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewNotificationData", false);
            if (!hasPermission) { return; }

            this.model = await this.notificationService.GetNotificationAndChangeStatusByIdAsync(_model.IdNotification);
            this.dataSource = await this.notificationService.GetAllNotificationsByPersonToAsync(this.idPerson, false);

            this.StateHasChanged();

            await this.editModal.OpenModal(this.model);
        }

        private async void CreateNotification()
        {
            bool hasPermission = await CheckUserActionPermission("ManageNotificationData", false);
            if (!hasPermission) { return; }
            
            var ids = new List<int>();
            ids.Add(this.idPerson);

            await notificationService.CreateNotificationsAsync(ids, "Create about test", "Create notification text test");
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
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = currentGrid.PageSettings.PageSize;
                currentGrid.PageSettings.PageSize = dataSource.Count();
                await currentGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "About", HeaderText = "Относно", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "NotificationText", HeaderText = "Коментар", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SendDateAsStr", HeaderText = "Дата на изпращане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ReviewDateAsStr", HeaderText = "Дата на прочитане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonFrom.FullName", HeaderText = "Изпратено от", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonTo.FullName", HeaderText = "Изпратено до", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusNotificationName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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

                await this.currentGrid.ExportToPdfAsync(ExportProperties);
                currentGrid.PageSettings.PageSize = temp;
                await currentGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Notification_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.currentGrid.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<NotificationVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(currentGrid, args.Data.IdNotification).Result.ToString();
            }
        }
    }
}

