using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate.ChangeLicenzing;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.ExcelExport;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;
using Border = Syncfusion.Blazor.Grids.Border;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class LicensingProcedure
    {
        private SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();
        private ProcedureModal procedureModal = new ProcedureModal();

        private ChangeProcedureModal changeProcedureModal = new ChangeProcedureModal();
        private IEnumerable<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersOriginalDataSource = new List<CandidateProviderVM>();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.candidateProviders = await CandidateProviderService.GetAllInаctiveCandidateProvidersByIdActiveCandidateProviderAsync(CandidateProviderVM.IdCandidate_Provider);

                await this.sfGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private async Task SelectedRowProcedure(CandidateProviderVM candidateProviderVM)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                if (!hasPermission) { return; }

                var kvChangeLicenzing = await DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "ChangeLicenzing");

                await this.procedureModal.OpenModal(candidateProviderVM, candidateProviderVM.IdTypeApplication == kvChangeLicenzing.IdKeyValue);
            }
            finally
            {
                this.loading = false;
            }
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
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
                ExportProperties.FileName = $"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.Columns = this.SetGridColumnsForExport();
                ExcelTheme Theme = new ExcelTheme();
                Border border = new Border();
                border.LineStyle = LineStyle.Thin;
                border.Color = "#000000";
                ExcelStyle HeaderThemeStyle = new ExcelStyle()
                {
                    BackColor = "#00BCD4",
                    FontSize = 12,
                    Bold = true,
                    FontColor = "#FFFFFF",
                    Borders = border,
                    WrapText = true,
                    VAlign = ExcelVerticalAlign.Top
                };

                ExcelStyle RecordThemeStyle = new ExcelStyle()
                {
                    Borders = border,
                    WrapText = true,
                    VAlign = ExcelVerticalAlign.Top
                };
                Theme.Header = HeaderThemeStyle;
                Theme.Record = RecordThemeStyle;

                ExportProperties.Theme = Theme;
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            else if (args.Item.Id == "sfGrid_csvexport")
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ApplicationNumberDate", HeaderText = "Заявление №", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TypeApplication", HeaderText = "Процедура", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ApplicationStatus", HeaderText = "Статус", TextAlign = TextAlign.Left });

            return ExportColumns;
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
                sheet.Range["A1"].Text = "Заявление №";
                sheet.Range["B1"].ColumnWidth = 120;
                sheet.Range["B1"].Text = "Процедура";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Статус";

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
       
                var rowCounter = 2;
                foreach (var item in candidateProviders)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.ApplicationNumberDate;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.TypeApplication;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.ApplicationStatus;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }
    }
}
