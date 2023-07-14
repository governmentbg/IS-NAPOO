using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.VocationalQualificationValidationCertificates
{
    public partial class VocationalQualificationValidationCertificateList
    {
        private SfGrid<ValidationClientVM> sfGrid = new SfGrid<ValidationClientVM>();

        VocationalQualificationValidationCertificateListModal modal;
        VocationalQualificationValidationCertificateListFilter filterModal;

        List<ValidationClientVM> validationClients = new List<ValidationClientVM>();

        private KeyValueVM kvPartOfProfession = new KeyValueVM();

        private KeyValueVM kvSPK = new KeyValueVM();

        private List<KeyValueVM> typeFrameworkProgramSource = new List<KeyValueVM>();
        private int IdTypeCourse = 0;

        private string header;

        [Inject]
        ITrainingService trainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        IDataSourceService dataSourceService { get; set; }
        
        [Inject]
        ICommonService commonService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            //this.validationClients = (await this.trainingService.getAllValidationClients()).ToList();
            this.typeFrameworkProgramSource = (await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeFrameworkProgram")).ToList();
            this.kvPartOfProfession = this.typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfPartOfProfession");
            this.kvSPK = this.typeFrameworkProgramSource.FirstOrDefault(x => x.KeyValueIntCode == "ValidationOfProfessionalQualifications");
            // validationClients = (await this.trainingService.getAllValidationClients()).Where(x => x.IdCourseType == this.IdTypeCourse && x.IdCandidateProvider == UserProps.IdCandidateProvider&& x.IdCandidateProvider == UserProps.IdCandidateProvider && x.ValidationClientDocuments.Count() != 0).ToList();
            // validationClients = (await this.trainingService.getAllValidationClients()).Where(x => x.IdCourseType == this.IdTypeCourse && x.ValidationClientDocuments.Count() != 0).ToList();

            await HandleTokenData();
        }
        public async Task openModal(ValidationClientVM validationClientVM)
        {
            if (loading) return;

            try
            {
                loading = true;
                await this.modal.openModal(validationClientVM);
            }
            finally
            {
                loading = false;
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            this.SpinnerShow();

            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = validationClients.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                PdfTheme Theme = new PdfTheme();

                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "90", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FullName", HeaderText = "Име на лицето", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Speciality.CodeAndNameAndVQS", HeaderText = "Специалност", Width = "300", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.Location.LocationName", HeaderText = "Населено място", Width = "90", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
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
                ExportProperties.FileName = $"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "90", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FullName", HeaderText = "Име на лицето", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Speciality.CodeAndNameAndVQS", HeaderText = "Специалност", Width = "300", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CandidateProvider.Location.LocationName", HeaderText = "Населено място", Width = "90", TextAlign = TextAlign.Left });
      
                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelValidationClientValidationErrors();
                await this.JsRuntime.SaveAs($"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());

            }

            this.SpinnerHide();
        }
        public MemoryStream CreateExcelValidationClientValidationErrors()
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
                sheet.Range["B1"].Text = "ЦПО";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Име на лицето";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Професия";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Специалност";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Населено място";
     

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

                
                var rowCounter = 2;
                foreach (var item in validationClients)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.CandidateProvider.LicenceDate != null ? item.CandidateProvider.LicenceNumberWithDate : item.CandidateProvider.LicenceNumber;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = $"ЦПО {item.CandidateProvider.ProviderName} към {item.CandidateProvider.ProviderOwner}";
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.FullName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.Speciality.Profession.CodeAndName;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.Speciality.CodeAndNameAndVQS;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ValidationClientVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdValidationClient).Result.ToString();
            }
        }

        public async Task FilterGrid()
        {
            await this.filterModal.openModal();
        }
        public async Task HandleTokenData()
        {
            header = string.Empty;
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.commonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    var type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();

                    if (type!.Equals("SPK"))
                    {
                        header = "Регистър на Свидетелства за валидиране на професионална квалификация";
                        this.IdTypeCourse = this.kvSPK.IdKeyValue;
                    }
                    else if (type.Equals("PP"))
                    {
                        header = "Регистър на Удостоверения за валидиране на професионална квалификация по част от професия";
                        this.IdTypeCourse = this.kvPartOfProfession.IdKeyValue;
                    }
                    else
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                    }

                }
                else
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }
        public async Task Filter(ValidationClientFIlterVM model)
        {
            //validationClients = trainingService.FilterClientCourseDocuments(model);
            validationClients = (await trainingService.FilterValidationClients(model)).Where(x => x.IdCourseType == this.IdTypeCourse && x.ValidationClientDocuments.Count() != 0).ToList();
        }
    }
}
