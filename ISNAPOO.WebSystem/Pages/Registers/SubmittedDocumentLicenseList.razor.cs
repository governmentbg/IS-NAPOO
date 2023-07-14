using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers
{
    partial class SubmittedDocumentLicenseList : BlazorBaseComponent
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }


        [Inject]
        public ILocationService LocationService { get; set; }



        List<CandidateProviderVM> candidateProviderVMs;
        SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();
        ApplicationModal applicationModal = new ApplicationModal();
        CIPOApplicationModal cipoApplicationModal = new CIPOApplicationModal();
        ToastMsg toast = new ToastMsg();

        public string Header { get; set; }
        private string fileName = "";
        private string LicensingType = string.Empty;
        private CandidateProviderVM filterCandidateProviderVM = new CandidateProviderVM();

        protected override async Task OnInitializedAsync()
        {


        }

        protected override async Task OnAfterRenderAsync(bool firstRender)

        {
            if (firstRender)
            {
                ISNAPOO.Common.Framework.ResultContext<TokenVM> tokenContext = new ISNAPOO.Common.Framework.ResultContext<TokenVM>();
                tokenContext.ResultContextObject.Token = Token;

                try
                {
                    tokenContext = BaseHelper.GetDecodeToken(tokenContext);
                }
                catch (Exception)
                {

                    await this.ShowErrorAsync("Грешен линк");
                    return;
                }

                this.LicensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "LicensingType").Value.ToString();
                if (this.LicensingType == "LicensingCPO")
                {
                    Header = "Подали документи за лицензиране на ЦПО";
                    fileName = $"DocumentForLicense_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                    filterCandidateProviderVM.IdTypeLicense = (await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO")).IdKeyValue;
                }
                else
                {
                    Header = "Подали документи за лицензиране на ЦИПО";
                    fileName = $"DocumentForLicense_CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                    filterCandidateProviderVM.IdTypeLicense = (await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO")).IdKeyValue;
                }

                this.candidateProviderVMs = (await this.candidateProviderService.GetAllActiveProceduresForRegisterAsync(this.LicensingType == "LicensingCPO")).ToList();

                var locations = await LocationService.GetAllLocationsAsync();

                foreach (var entry in this.candidateProviderVMs)
                {
                    if (entry.IdLocationCorrespondence != null)
                    {
                        entry.Location = locations.First(x => x.idLocation == entry.IdLocationCorrespondence);
                    }
                }

                await this.sfGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private async Task OpenApplicationModalBtn(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider }, false, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();

        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = candidateProviderVMs.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Location.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationNumber", HeaderText = "Номер на заявление", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationStatus", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = fileName + ".pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Location.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationNumber", HeaderText = "Номер на заявление", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationStatus", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = fileName + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.IncludeTemplateColumn = true;

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Location.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationNumber", HeaderText = "Номер на заявление", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ApplicationStatus", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs(fileName + ".csv", result.ToArray()); ;
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
                sheet.Range["A1"].Text = "Юридическо лице";
                sheet.Range["B1"].ColumnWidth = 120;
                sheet.Range["B1"].Text = "Населено място";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Номер на заявление";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Статус";

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




                var rowCounter = 2;
                foreach (var item in candidateProviderVMs)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.ProviderOwner;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.Location != null ? item.Location.LocationName : String.Empty;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.ApplicationNumber;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.ApplicationStatus;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CandidateProviderVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdCandidate_Provider).Result.ToString();
            }
        }
    }
}
