using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterLegalCapacity
{
    public partial class RegisterLegalCapacityList : BlazorBaseComponent
    {
        SfGrid<ClientCourseDocumentVM> sfGrid;

        RegisterLegalCapacityModal modal;
        

        List<ClientCourseDocumentVM> documents = new List<ClientCourseDocumentVM>();

        [Inject]
        ITrainingService trainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        IDataSourceService dataSourceService { get; set; }

        [Inject]
        IProviderDocumentRequestService providerDocumentRequestService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            //3-114 е docTypeOfficialNumber от таблица Request_TypeOfRequestedDocument по него филтрираме списъка
            var typeOfRequestedDocument1 = await this.providerDocumentRequestService.GetTypeOfRequestedDocumentAsyncByDocTypeOfficialNumber("3-114");
            var typeOfRequestedDocument2 = await this.providerDocumentRequestService.GetTypeOfRequestedDocumentAsyncByDocTypeOfficialNumber("3-116");
            
            documents = await trainingService.GetAllDocumentsByIdTypeOfRequestedDocument(new int?[] {typeOfRequestedDocument1.IdTypeOfRequestedDocument, typeOfRequestedDocument2.IdTypeOfRequestedDocument } );
        }
        public async Task openModal(ClientCourseDocumentVM clientCourseDocumentVM)
        {
            if (loading) return;

            try
            {
                loading = true;
                await this.modal.openModal(clientCourseDocumentVM);
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
                sfGrid.PageSettings.PageSize = documents.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                PdfTheme Theme = new PdfTheme();

                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FullName", HeaderText = "Име на курсист", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS", HeaderText = "Специалност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.CourseName", HeaderText = "Курс", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.timeSpan", HeaderText = "Период на провеждане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Location.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.CourseType.Name", HeaderText = "Вид на обучение", Width = "80", TextAlign = TextAlign.Left });

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
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "90", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FullName", HeaderText = "Име на курсист", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS", HeaderText = "Специалност", Width = "300", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.CourseName", HeaderText = "Курс", Width = "400", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.timeSpan", HeaderText = "Период на провеждане", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Location.LocationName", HeaderText = "Населено място", Width = "90", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.CourseType.Name", HeaderText = "Вид на обучение", Width = "300", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());

            }

            this.SpinnerHide();
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
                sheet.Range["B1"].Text = "Юридическо лице";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Име на курсист";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Професия";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Специалност";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Курс";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "Период на провеждане";
                sheet.Range["H1"].ColumnWidth = 120;
                sheet.Range["H1"].Text = "Населено място";
                sheet.Range["I1"].ColumnWidth = 120;
                sheet.Range["I1"].Text = "Вид на обучение";

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

                IRange rangeI = sheet.Range["I1"];
                IRichTextString boldTextI = rangeI.RichText;
                IFont boldFontI = workbook.CreateFont();

                boldFontI.Bold = true;
                boldTextI.SetFont(0, sheet.Range["I1"].Text.Length, boldFontI);


                var rowCounter = 2;
                foreach (var item in documents)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.ClientCourse.Client.CandidateProvider.LicenceNumberWithDate;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = $"ЦПО {item.ClientCourse.Client.CandidateProvider.ProviderName} към {item.ClientCourse.Client.CandidateProvider.ProviderOwner}";
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.ClientCourse.FullName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.ClientCourse.Course.Program.Speciality.Profession.CodeAndName;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.ClientCourse.Course.CourseName;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.ClientCourse.Course.timeSpan;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.ClientCourse.Course.Location.LocationName;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"I{rowCounter}"].Text = item.ClientCourse.Course.Program.CourseType.Name;
                    sheet.Range[$"I{rowCounter}"].WrapText = true;
                    sheet.Range[$"I{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ClientCourseDocumentVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdClientCourseDocument).Result.ToString();
            }
        }

    }
}
