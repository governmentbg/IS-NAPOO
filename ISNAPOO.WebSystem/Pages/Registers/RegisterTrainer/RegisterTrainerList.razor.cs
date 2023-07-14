using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.Core.ViewModels.Register;
using Syncfusion.PdfExport;
using ISNAPOO.Common.HelperClasses;
using Microsoft.JSInterop;
using ISNAPOO.Core.HelperClasses;
using Syncfusion.XlsIO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.Contracts.Candidate;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer
{
    public partial class RegisterTrainerList : BlazorBaseComponent
    {
        private SfGrid<RegisterTrainerVM> trainersGrid = new SfGrid<RegisterTrainerVM>();
        private TrainerInformationModal trainerInformationModal = new TrainerInformationModal();
        private TrainerStatusModal trainerStatusModal = new TrainerStatusModal();
        private TrainerChecking trainerChecking = new TrainerChecking();
        private FilterTrainerModal filterTrainerModal = new FilterTrainerModal();

        private string title = string.Empty;
        private List<RegisterTrainerVM> trainersSource = new List<RegisterTrainerVM>();
        private string licensingType = string.Empty;
        private string cpoOrCipo = string.Empty;
        private bool isVisibleProfessionalDirectionsInfo = true;

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
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

            this.licensingType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "LicensingType").Value.ToString();
            if (this.licensingType == "LicensingCPO")
            {
                this.title = "Регистър на преподавателите в ЦПО";
                this.cpoOrCipo = "ЦПО";
                this.isVisibleProfessionalDirectionsInfo = true;
            }
            else
            {
                this.title = "Регистър на служителите в ЦИПО";
                this.cpoOrCipo = "ЦИПО";
                this.isVisibleProfessionalDirectionsInfo = false;
            }
        }

        private async Task ViewTrainerBtn(int idCandidateProviderTrainer)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.trainerInformationModal.OpenModal(idCandidateProviderTrainer, this.licensingType == "LicensingCPO");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                int temp = this.trainersGrid.PageSettings.PageSize;
                this.trainersGrid.PageSettings.PageSize = this.trainersSource.Count;
                await this.trainersGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenseNumber", HeaderText = "Лицензия", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SecondName", HeaderText = "Презиме", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OwnerAndProvider", HeaderText = this.cpoOrCipo, Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProfessionalDirections", HeaderText = "Професионални направления", Width = "180" });
                ExportColumns.Add(new GridColumn() { Field = "StatusValue", HeaderText = "Статус", Width = "180", TextAlign = TextAlign.Left });
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

                ExportProperties.FileName = $"Register_Trainers_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.trainersGrid.ExportToPdfAsync(ExportProperties);
                this.trainersGrid.PageSettings.PageSize = temp;
                await this.trainersGrid.Refresh();
            }

            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenseNumber", HeaderText = "Лицензия", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FirstName", HeaderText = "Име", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SecondName", HeaderText = "Презиме", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FamilyName", HeaderText = "Фамилия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "OwnerAndProvider", HeaderText = this.cpoOrCipo, Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProfessionalDirections", HeaderText = "Професионални направления", Width = "180" });
                ExportColumns.Add(new GridColumn() { Field = "StatusValue", HeaderText = "Статус", Width = "180", TextAlign = TextAlign.Left });
#pragma warning restore BL0005
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Register_Trainers_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.trainersGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"Register_Trainers_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
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
                sheet.Range["B1"].Text = "Име";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Презиме";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Фамилия";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = $"{cpoOrCipo}";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Професионални направления";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "Статус";

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


                var rowCounter = 2;
                foreach (var item in this.trainersSource)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.LicenseNumber;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.FirstName;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.SecondName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.FamilyName;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.OwnerAndProvider;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.ProfessionalDirections;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.StatusValue;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<RegisterTrainerVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(this.trainersGrid, args.Data.IdEntity).Result.ToString();
            }

        }

        private async Task OpenChecking(RegisterTrainerVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var fullName = !string.IsNullOrEmpty(model.SecondName)
                    ? $"{model.FirstName} {model.SecondName} {model.FamilyName}"
                    : $"{model.FirstName} {model.FamilyName}";

                await this.trainerChecking.OpenModal(model.IdEntity, fullName);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenFilterModal()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.filterTrainerModal.OpenModal(this.licensingType == "LicensingCPO");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenTrainerHistoryModalBtn(RegisterTrainerVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var typeTrainer = this.licensingType == "LicensingCPO"
                    ? "преподавател"
                    : "консултант";
                var trainerFromDb = await this.CandidateProviderService.GetCandidateProviderTrainerWithoutIncludesByIdAsync(model.IdEntity);
                await this.trainerStatusModal.OpenModal(trainerFromDb, typeTrainer);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task UpdateAfterFilterAsync(List<RegisterTrainerVM> trainers)
        {
            this.trainersSource = trainers.ToList();

            await this.trainersGrid.Refresh();
            this.StateHasChanged();
        }
    }
}
