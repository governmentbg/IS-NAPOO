using System;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.Courses
{
    public partial class CourseList : BlazorBaseComponent
    {
        private List<CourseVM> courses = new List<CourseVM>();

        private SfGrid<CourseVM> sfGrid = new SfGrid<CourseVM>();

        private CourseFilter filterModal;
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        private CourseCheckingsList courseCheckingsList = new CourseCheckingsList();

        [Inject] 
        public ITrainingService TrainingService { get; set; }

        [Inject] 
        public IJSRuntime JsRuntime { get; set; }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            this.SpinnerShow();

            if (args.Item.Id.Contains("pdfexport"))
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = courses.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "CandidateProvider.LicenceNumber",
                    HeaderText = "Лицензия",
                    Width = "180",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "CandidateProvider.CPONameOwnerGrid",
                    HeaderText = "ЦПО",
                    Width = "80",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "Program.Speciality.Profession.CodeAndName",
                    HeaderText = "Професия",
                    Width = "80",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "Program.Speciality.CodeAndNameAndVQS",
                    HeaderText = "Специалност",
                    Width = "80",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "TrainingCourseTypeName",
                    HeaderText = "Вид на обучение",
                    Width = "80",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "CourseName",
                    HeaderText = "Наименование на курс",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "Location.LocationName",
                    HeaderText = "Населено място",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "AssignTypeName",
                    HeaderText = "Основен източник на финансиране",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "FormEducationName",
                    HeaderText = "Форма на обучение",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "MandatoryHours",
                    HeaderText = "Задължителни уч. ч.",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "SelectableHours",
                    HeaderText = "Избираеми уч. ч.",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "StatusName",
                    HeaderText = "Статус на курса",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfTheme Theme = new PdfTheme();
                PdfThemeStyle RecordThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont()
                    {
                        IsTrueType = true,
                        FontStyle = PdfFontStyle.Regular,
                        FontFamily = FontFamilyPDF.fontFamilyBase64String
                    }
                };

                PdfThemeStyle HeaderThemeStyle = new PdfThemeStyle()
                {
                    Font = new PdfGridFont()
                    {
                        IsTrueType = true,
                        FontStyle = PdfFontStyle.Bold,
                        FontFamily = FontFamilyPDF.fontFamilyBase64String
                    }
                };
                Theme.Record = RecordThemeStyle;
                Theme.Header = HeaderThemeStyle;

                ExportProperties.Theme = Theme;
                ExportProperties.FileName =
                    $"Courses_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excelexport"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "CandidateProvider.LicenceNumber",
                    HeaderText = "Лицензия",
                    Width = "100",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "CandidateProvider.CPONameOwnerGrid",
                    HeaderText = "ЦПО",
                    Width = "150",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "Program.Speciality.Profession.CodeAndName",
                    HeaderText = "Професия",
                    Width = "200",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "Program.Speciality.CodeAndNameAndVQS",
                    HeaderText = "Специалност",
                    Width = "200",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "TrainingCourseTypeName",
                    HeaderText = "Вид на обучение",
                    Width = "200",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "CourseName",
                    HeaderText = "Наименование на курс",
                    Width = "200",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "Location.LocationName",
                    HeaderText = "Населено място",
                    Width = "100",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "AssignTypeName",
                    HeaderText = "Основен източник на финансиране",
                    Width = "200",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "FormEducationName",
                    HeaderText = "Форма на обучение",
                    Width = "200",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "MandatoryHours",
                    HeaderText = "Задължителни уч. ч.",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "SelectableHours",
                    HeaderText = "Избираеми уч. ч.",
                    Width = "60",
                    TextAlign = TextAlign.Left
                });
                ExportColumns.Add(new GridColumn()
                {
                    Field = "StatusName",
                    HeaderText = "Статус на курса",
                    Width = "80",
                    TextAlign = TextAlign.Left
                });
#pragma warning restore BL0005

                ExportProperties.Columns = ExportColumns;
                ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.FileName =
                    $"Courses_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs(
                    $"Courses_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
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
                sheet.Range["B1"].Text = "ЦПО";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Професия";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Специалност";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Вид на обучение";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Наименование на курс";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "Населено място";
                sheet.Range["H1"].ColumnWidth = 120;
                sheet.Range["H1"].Text = "Основен източник на финансиране";
                sheet.Range["I1"].ColumnWidth = 120;
                sheet.Range["I1"].Text = "Форма на обучение";
                sheet.Range["J1"].ColumnWidth = 120;
                sheet.Range["J1"].Text = "Учебни часове";
                sheet.Range["K1"].ColumnWidth = 120;
                sheet.Range["K1"].Text = "Статус на курса";


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

                IRange rangeJ = sheet.Range["J1"];
                IRichTextString boldTextJ = rangeI.RichText;
                IFont boldFontJ = workbook.CreateFont();

                boldFontI.Bold = true;
                boldTextI.SetFont(0, sheet.Range["J1"].Text.Length, boldFontJ);

                IRange rangeK = sheet.Range["K1"];
                IRichTextString boldTextK = rangeI.RichText;
                IFont boldFontK = workbook.CreateFont();

                boldFontI.Bold = true;
                boldTextI.SetFont(0, sheet.Range["K1"].Text.Length, boldFontK);



                var rowCounter = 2;
                foreach (var item in courses)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.CandidateProvider.LicenceNumber;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.CandidateProvider.CPONameOwnerGrid;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.Program.Speciality.Profession.CodeAndName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.Program.Speciality.CodeAndNameAndVQS;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.TrainingCourseTypeName;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.CourseName;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.Location.LocationName;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.AssignTypeName;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"I{rowCounter}"].Text = item.FormEducationName;
                    sheet.Range[$"I{rowCounter}"].WrapText = true;
                    sheet.Range[$"I{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"J{rowCounter}"].Text = item.MandatoryHours.ToString();
                    sheet.Range[$"J{rowCounter}"].WrapText = true;
                    sheet.Range[$"J{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"K{rowCounter}"].Text = item.StatusName;
                    sheet.Range[$"K{rowCounter}"].WrapText = true;
                    sheet.Range[$"K{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CourseVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdCourse).Result.ToString();
            }
        }

        public async Task openModal(CourseVM course)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }

            try
            {
                this.loading = true;

                var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(course.IdCourse);

                await this.currentTrainingCourseModal.OpenModal(courseFromDb, null, false);

            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenChecking(CourseVM args)
        {
            courseCheckingsList.OpenModal(args.IdCourse, args.CourseName);
        }

        public async Task FilterGrid()
        {
            await filterModal.openModal();
        }

        public void Filter(List<CourseVM> filteredCourses)
        {
            this.courses = filteredCourses.ToList();
            this.StateHasChanged();
        }
    }
}

