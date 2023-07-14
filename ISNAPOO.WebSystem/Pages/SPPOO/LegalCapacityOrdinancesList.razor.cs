using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.SPPOO.Modals.LegalCapacityOrdinance;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.SPPOO
{
    partial class LegalCapacityOrdinancesList : BlazorBaseComponent
    {
        private ToastMsg toast;
        private SfGrid<LegalCapacityOrdinanceUploadedFileVM> sfGrid = new SfGrid<LegalCapacityOrdinanceUploadedFileVM>();
        private LegalCapacityOrdinanceModal legalCapacityOrdinanceModal = new LegalCapacityOrdinanceModal();
        private List<LegalCapacityOrdinanceUploadedFileVM> ordinancesSource = new List<LegalCapacityOrdinanceUploadedFileVM>();
        private LegalCapacityOrdinanceUploadedFileVM orderToDelete = new LegalCapacityOrdinanceUploadedFileVM();

        protected override async Task OnInitializedAsync()
        {
            this.ordinancesSource = (await this.LegalCapacityOrdinanceService.GetAllOrdinancesAsync()).ToList();
            this.StateHasChanged();
        }

        private async Task AddNew()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }
            await this.legalCapacityOrdinanceModal.OpenModal(new LegalCapacityOrdinanceUploadedFileVM());
        }

        private async Task SelectedRow(LegalCapacityOrdinanceUploadedFileVM model)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }
            await this.legalCapacityOrdinanceModal.OpenModal(model);
        }

        private async Task OnDownloadClick(LegalCapacityOrdinanceUploadedFileVM model)
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
                    var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<LegalCapacityOrdinanceUploadedFile>(model.IdLegalCapacityOrdinanceUploadedFile);
                    if (hasFile)
                    {
                        var document = await this.UploadFileService.GetUploadedFileAsync<LegalCapacityOrdinanceUploadedFile>(model.IdLegalCapacityOrdinanceUploadedFile);
                        if (document.MS is not null)
                        {
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
        private async Task UpdateAfterSave()
        {
            this.ordinancesSource = (await this.LegalCapacityOrdinanceService.GetAllOrdinancesAsync()).ToList();
            this.StateHasChanged();
        }
        private async Task DeleteDocument(LegalCapacityOrdinanceUploadedFileVM legalCapacityOrdinanceUploadedFileVM)
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }
            this.orderToDelete = legalCapacityOrdinanceUploadedFileVM;
            this.ConfirmDialog.showDeleteConfirmDialog = !this.ConfirmDialog.showDeleteConfirmDialog;

        }

        public async void ConfirmDeleteCallback()
        {
            var result = await this.LegalCapacityOrdinanceService.DeleteOrdinanceAsync(this.orderToDelete);

            if (result > 0)
            {
                this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                await this.toast.sfSuccessToast.ShowAsync();

                this.ordinancesSource = (await this.LegalCapacityOrdinanceService.GetAllOrdinancesAsync()).ToList();

                this.StateHasChanged();
            }

            this.StateHasChanged();
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = ordinancesSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LegalCapacityOrdinanceTypeName", HeaderText = "Наредба за правоспособност", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "UploadedFileName", HeaderText = "Прикачен файл", Width = "80", Format = "d", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"Naredbi_za_pravosposobnost_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {

                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"Naredbi_za_pravosposobnost_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", result.ToArray());
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<LegalCapacityOrdinanceUploadedFileVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdLegalCapacityOrdinanceUploadedFile).Result.ToString();
            }
        }
        public MemoryStream CreateExcelCurriculumValidationErrors()
        {
            using (ExcelEngine excelEngine = new ExcelEngine())
            {
                IApplication application = excelEngine.Excel;
                application.DefaultVersion = ExcelVersion.Excel2016;

                IWorkbook workbook = application.Workbooks.Create(1);
                IWorksheet sheet = workbook.Worksheets[0];

                sheet.Range["A1"].ColumnWidth = 120;
                sheet.Range["A1"].Text = "Наредба за правоспособност";
                sheet.Range["B1"].ColumnWidth = 10;
                sheet.Range["B1"].Text = "Прикачен файл";

                IRange range = sheet.Range["A1"];
                IRichTextString boldText = range.RichText;
                IFont boldFont = workbook.CreateFont();

                boldFont.Bold = true;
                boldText.SetFont(0, sheet.Range["A1"].Text.Length, boldFont);

                IRange range2 = sheet.Range["B1"];
                IRichTextString boldText2 = range2.RichText;
                IFont boldFont2 = workbook.CreateFont();

                boldFont2.Bold = true;
                boldText2.SetFont(0, sheet.Range["B1"].Text.Length, boldFont2);

                


                var rowCounter = 2;
                ordinancesSource = ordinancesSource.OrderBy(d => d.LegalCapacityOrdinanceTypeName).ToList();
                foreach (var item in ordinancesSource)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.LegalCapacityOrdinanceTypeName;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 20;
                    sheet.Range[$"B{rowCounter}"].Text = item.UploadedFileName;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }
    }
}
