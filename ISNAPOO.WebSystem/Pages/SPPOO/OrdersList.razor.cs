using Data.Models;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.Order;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.SplitButtons;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.SPPOO
{
    public partial class OrdersList : BlazorBaseComponent
    {
        [Inject]
        IOrderService OrderService { get; set; }

        [Inject]
        IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IUploadFileService uploadService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        ToastMsg toast;
        private bool IsVisibleAddModal { get; set; } = false;
        SfDialog sfFilter;
        private string dialogClass = "";
        OrderVM model = new OrderVM();
        private bool showSerialNumberConfirmDialog = false;
        private bool serialNumberDeleteConfirmed = false;
        OrderVM orderToDelete = new OrderVM();
        OrderModal orderModal = new OrderModal();

        IEnumerable<OrderVM> orders;
        SfGrid<OrderVM> ordersGrid;
        List<OrderVM> selectedOrdersList;
        List<string> Export = new List<string>(){ "PDF", "EXCEL" };
        string ComboboxValue = "Export";
        protected override async Task OnInitializedAsync()
        {
           // this.sfFilter = new SfDialog();

            this.orders = await this.OrderService.GetAllOrdersAsync(model);

            this.selectedOrdersList = new List<OrderVM>();

            //this.ordersGrid = new SfGrid<OrderVM>();
        }

        private async Task SelectedRow(OrderVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ViewOrderData", false);
            if (!hasPermission) { return; }

            this.orderModal.OpenModal(_model);
        }

        private async Task<Task> UpdateAfterSave(OrderVM _model)
        {
            this.orders = await this.OrderService.GetAllOrdersAsync(model);

            return Task.CompletedTask;
        }
        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }

            this.orderModal.OpenModal(new OrderVM() { OrderDate = null });
        }
        private async void EX()
        {
            ExcelExportProperties ExportProperties = new ExcelExportProperties();
            ExportProperties.FileName = $"DOC_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";

            await this.ordersGrid.ExcelExport(ExportProperties);
        }
        private async void PDF()
        {
            PdfExportProperties ExportProperties = new PdfExportProperties();
            PdfTheme Theme = new PdfTheme();
            PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
            {
                Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
            };

            PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
            {
                Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String },
            };
            Theme.Record = RecordThemeStyle;
            Theme.Header = HeaderThemeStyle;

            ExportProperties.Theme = Theme;
            ExportProperties.FileName = $"DOC_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

            await this.ordersGrid.PdfExport(ExportProperties);
        }

        private async Task ValueChangeHandler(ChangeEventArgs<string, string> args)
        {
            if (args.ItemData == "EXCEL")
            {

                    ExcelExportProperties ExportProperties = new ExcelExportProperties();
                    ExportProperties.FileName = $"DOC_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";

                    await this.ordersGrid.ExcelExport(ExportProperties);
                
            }
            if (args.ItemData == "PDF")
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Regular, FontFamily = FontFamilyPDF.fontFamilyBase64String }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont() { IsTrueType = true, FontStyle = PdfFontStyle.Bold, FontFamily = FontFamilyPDF.fontFamilyBase64String },
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName = $"DOC_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.ordersGrid.PdfExport(ExportProperties);
            }
        }

        private async Task Search()
        {
            this.IsVisibleAddModal = false;
            this.orders = await this.OrderService.GetAllOrdersAsync(model);
        }

        private void ShowFilter()
        {
            this.IsVisibleAddModal = true;
        }

        private void ClearFilter()
        {
            this.model = new OrderVM();
        }

        private void Cancel()
        {
            this.IsVisibleAddModal = false;
        }

        private void RowSelected(RowSelectEventArgs<OrderVM> selectArgs)
        {
            this.selectedOrdersList.Add(selectArgs.Data);
        }

        private async Task RowDeselected(RowDeselectEventArgs<OrderVM> selectArgs)
        {
            OrderVM template = selectArgs.Data;

            this.selectedOrdersList.Remove(template);
        }

        public async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "ordersGrid_pdfexport")
            {
                int temp = ordersGrid.PageSettings.PageSize;
                ordersGrid.PageSettings.PageSize = orders.Count();
                await ordersGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "OrderNumber", HeaderText = "Номер на заповед", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OrderDate", HeaderText = "Дата на заповед", Width = "80", Format = "d", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
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
                ExportProperties.FileName = $"Zapovedi_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";
                //this.Grid.PdfExport(ExportProperties);

                await this.ordersGrid.PdfExport(ExportProperties);
                ordersGrid.PageSettings.PageSize = temp;
                await ordersGrid.Refresh();
            }
            else if (args.Item.Id == "ordersGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Zapovedi_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";

                await this.ordersGrid.ExcelExport(ExportProperties);
            }
            else
            {
                await this.ordersGrid.CsvExport();
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<OrderVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(ordersGrid, args.Data.IdOrder).Result.ToString();
            }
        }
        private async Task DeleteSelected(OrderVM _model)
        {
            bool hasPermission = await CheckUserActionPermission("ManageOrderData", false);
            if (!hasPermission) { return; }
            this.orderToDelete = _model;
            this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;
        
        }
        private async Task OnDownloadClick(OrderVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.uploadService.CheckIfExistUploadedFileAsync<SPPOOOrder>(model.IdOrder);
                if (hasFile)
                {
                    var documentStream = await this.uploadService.GetUploadedFileAsync<SPPOOOrder>(model.IdOrder);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.MS!.ToArray());
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
        public async void ConfirmDeleteCallback()
        {
            this.serialNumberDeleteConfirmed = false;
            await OrderService.DeleteOrderAsync(orderToDelete);
            toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
            await toast.sfSuccessToast.ShowAsync();
            this.orders = await this.OrderService.GetAllOrdersAsync(model);
            selectedOrdersList.Clear();
            this.StateHasChanged();
        }
    }
}

