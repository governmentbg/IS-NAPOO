using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Register;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer;

using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterMTB
{
    public partial class RegisterMTBList : BlazorBaseComponent
    {
        private SfGrid<RegisterMTBVM> mtbsGrid = new SfGrid<RegisterMTBVM>();
        private MTBInformationModal mTBInformationModal = new MTBInformationModal();

        private MTBStatusModal mTBStatusModal = new MTBStatusModal();
        private MTBChecking mTBCheckingModal = new MTBChecking();
        private FilterMTBModal filterMTB = new FilterMTBModal();
        private List<CandidateProviderTrainerVM> candidateProviderTrainersSource = new List<CandidateProviderTrainerVM>();
        private IEnumerable<RegisterMTBVM> mtbsSource = new List<RegisterMTBVM>();
        private List<RegisterMTBVM> originalMtbsSource = new List<RegisterMTBVM>();

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //this.mtbsSource = await this.CandidateProviderService.GetAllMTBsForActiveCandidateProvidersAsync();
            //this.originalMtbsSource = mtbsSource.ToList();
            await base.OnInitializedAsync();
        }

        private async Task OpenMTBInformationModalBtn(RegisterMTBVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.mTBInformationModal.OpenModal(model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenCheckingModalBtn(RegisterMTBVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.mTBCheckingModal.OpenModal(model.CandidateProviderPremises.IdCandidateProviderPremises, model.CandidateProviderPremises.PremisesName, model.CandidateProvider.IdCandidate_Provider);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenFilterModal()
        {
            await this.filterMTB.OpenModal();
        }

        private void OpenMTBStatusModalBtn(RegisterMTBVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.mTBStatusModal.OpenModal(model.CandidateProviderPremises);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void UpdateAfterFilterModalSubmit(List<RegisterMTBVM> mtbvms)
        {
            if (mtbvms != null)
            {
                this.mtbsSource = mtbvms;
            }
            else
            {
                this.mtbsSource = this.originalMtbsSource.ToList();
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
                sheet.Range["A1"].Text = "Лицензия";
                sheet.Range["B1"].ColumnWidth = 120;
                sheet.Range["B1"].Text = "Материално-техническа база";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Вид на лицензия";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Населено място";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Адрес";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Име на центъра";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "Форма на собственост";
                sheet.Range["H1"].ColumnWidth = 120;
                sheet.Range["H1"].Text = "Статус";

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

                IRange rangeC = sheet.Range["C1"];
                IRichTextString boldTextC = rangeC.RichText;
                IFont boldFontC = workbook.CreateFont();

                boldFontC.Bold = true;
                boldTextC.SetFont(0, sheet.Range["C1"].Text.Length, boldFontC);

                IRange rangeD = sheet.Range["D1"];
                IRichTextString boldTextD = rangeD.RichText;
                IFont boldFontD = workbook.CreateFont();

                boldFontD.Bold = true;
                boldTextD.SetFont(0, sheet.Range["D1"].Text.Length, boldFontD);

                IRange rangeE = sheet.Range["E1"];
                IRichTextString boldTextE = rangeE.RichText;
                IFont boldFontE = workbook.CreateFont();

                boldFontE.Bold = true;
                boldTextE.SetFont(0, sheet.Range["E1"].Text.Length, boldFontE);

                IRange rangeF = sheet.Range["F1"];
                IRichTextString boldTextF = rangeF.RichText;
                IFont boldFontF = workbook.CreateFont();

                boldFontF.Bold = true;
                boldTextF.SetFont(0, sheet.Range["F1"].Text.Length, boldFontF);

                IRange rangeG = sheet.Range["G1"];
                IRichTextString boldTextG = rangeG.RichText;
                IFont boldFontG = workbook.CreateFont();

                boldFontG.Bold = true;
                boldTextG.SetFont(0, sheet.Range["G1"].Text.Length, boldFontG);

                IRange rangeH = sheet.Range["H1"];
                IRichTextString boldTextH = rangeH.RichText;
                IFont boldFontH = workbook.CreateFont();

                boldFontH.Bold = true;
                boldTextH.SetFont(0, sheet.Range["H1"].Text.Length, boldFontH);


                var rowCounter = 2;
                foreach (var item in this.mtbsSource)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.CandidateProvider.LicenceNumber;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.CandidateProviderPremises.PremisesName;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.CandidateProvider.LicenceTypeValue;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.CandidateProvider.Location.LocationName;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.CandidateProviderPremises.ProviderAddress;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.CandidateProvider.ProviderNameAndOwnerForRegister;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.CandidateProviderPremises.OwnershipValue;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.CandidateProviderPremises.StatusValue;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = mtbsGrid.PageSettings.PageSize;
                mtbsGrid.PageSettings.PageSize = this.mtbsSource.Count();
                await mtbsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenseNumber", HeaderText = "Лиценз", Width = "180", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Материално-техническа база", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfLicense", HeaderText = "Вид на лицензия", Width = "180", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "City", HeaderText = "Населено място", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Address", HeaderText = "Адрес", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CpoAndOwner", HeaderText = "ЦПО", Width = "180", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfOwnership", HeaderText = "Форма на собственост", Width = "180", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Status", HeaderText = "Статус", Width = "180", Format = "C2", TextAlign = TextAlign.Left });
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

                ExportProperties.FileName = $"Register_MTB_CPO&CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.mtbsGrid.ExportToPdfAsync(ExportProperties);
                mtbsGrid.PageSettings.PageSize = temp;
                await mtbsGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "LicenseNumber", HeaderText = "Лицензия", Width = "60", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "Материално-техническа база", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfLicense", HeaderText = "Вид на лицензия", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "City", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Address", HeaderText = "Адрес", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CpoAndOwner", HeaderText = "ЦПО", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeOfOwnership", HeaderText = "Форма на собственост", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Status", HeaderText = "Статус", Width = "180", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Register_MTB_CPO&CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.mtbsGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"Register_MTB_CPO&CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
            }
        }
    }
}
