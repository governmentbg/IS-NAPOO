using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ProcedurePriceList : BlazorBaseComponent
    {
        [Inject]
        public IProviderService providerService { get; set; }

        private ToastMsg toast = new ToastMsg();
        private ProcedurePriceModal priceModal = new ProcedurePriceModal();
        private SfGrid<ProcedurePriceVM> sfGridPrice = new SfGrid<ProcedurePriceVM>();
        private IEnumerable<ProcedurePriceVM> procedurePriceSource = new List<ProcedurePriceVM>();

        protected override async Task OnInitializedAsync()
        {
            this.procedurePriceSource = await this.providerService.GetAllProcedurePricesAsync();
        }

        private async Task OpenAddNewModal()
        {
            bool hasPermission = await CheckUserActionPermission("ManageProcedurePriceData", false);
            if (!hasPermission) { return; }

            var model = new ProcedurePriceVM();
            await this.priceModal.OpenModal(model);
        }

        private async Task SelectedRow(ProcedurePriceVM procedurePriceVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewProcedurePriceData", false);
            if (!hasPermission) { return; }

            var priceVM = await this.providerService.GetProcedurePriceByIdAsync(procedurePriceVM);
            await this.priceModal.OpenModal(priceVM);
        }

        private async Task OnApplicationSubmit(ResultContext<ProcedurePriceVM> resultContext)
        {
            if (resultContext.HasMessages)
            {
                toast.sfSuccessToast.Content = string.Join(Environment.NewLine, resultContext.ListMessages);
                await toast.sfSuccessToast.ShowAsync();
                this.procedurePriceSource = await this.providerService.GetAllProcedurePricesAsync();
                this.sfGridPrice.Refresh();
                resultContext.ListMessages.Clear();
            }
            else
            {
                toast.sfErrorToast.Content = string.Join(Environment.NewLine, resultContext.ListErrorMessages);
                await toast.sfErrorToast.ShowAsync();
                resultContext.ListErrorMessages.Clear();
            }
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGridPrice.PageSettings.PageSize;
                sfGridPrice.PageSettings.PageSize = procedurePriceSource.Count();
                await sfGridPrice.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Наименование на услугата", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Price", HeaderText = "Цена", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeApplicationName", HeaderText = "Вид на заявлението", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CountProfessionsFrom", HeaderText = "Брой професии от", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CountProfessionsTo", HeaderText = "Брой професии до", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ExpirationDateFrom", HeaderText = "Дата на валидност от", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ExpirationDateTo", HeaderText = "Дата на валидност до", Width = "80", Format="d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationStatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"Procedure_Price_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGridPrice.ExportToPdfAsync(ExportProperties);
                sfGridPrice.PageSettings.PageSize = temp;
                await sfGridPrice.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"Procedure_Price_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGridPrice.ExportToExcelAsync(ExportProperties);
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ProcedurePriceVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGridPrice, args.Data.IdProcedurePrice).Result.ToString();
            }
        }

    }
}
