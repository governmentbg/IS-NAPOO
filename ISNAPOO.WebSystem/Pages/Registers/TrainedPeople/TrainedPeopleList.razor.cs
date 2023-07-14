using System;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Pdf.Grid;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.TrainedPeople
{
    public partial class TrainedPeopleList : BlazorBaseComponent
    {
        private SfGrid<ClientCourseVM> clientsGrid = new SfGrid<ClientCourseVM>();

        private DocumentsFromCPOModal documentsFromCPOModal = new DocumentsFromCPOModal();
        private DocumentsFromCPOFilter filterModal = new DocumentsFromCPOFilter();
        private CurrentCourseClientModal currentCourseClientModal = new CurrentCourseClientModal();
        private CurrentTrainingCourseModal currentTrainingCourseModal = new CurrentTrainingCourseModal();
        private List<ClientCourseVM> clientsSource = new List<ClientCourseVM>();

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
        }

        public async Task OpenViewClientBtn(ClientCourseVM clientCourse)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var clientCourseDocFromDb = await this.TrainingService.GetClientCourseDocumentByIdClientCourseAsync(clientCourse.IdClientCourse);
                
                var model = new DocumentsFromCPORegisterVM()
                {
                    IsCourse = true,
                    IdEntity = clientCourseDocFromDb.IdClientCourseDocument,
                    LicenceNumber = clientCourse.Course.CandidateProvider.LicenceNumber ?? string.Empty,
                    CPONameOwnerGrid = !string.IsNullOrEmpty(clientCourse.Course.CandidateProvider.ProviderName) ? $"ЦПО {clientCourse.Course.CandidateProvider.ProviderName} към {clientCourse.Course.CandidateProvider.ProviderOwner}" : $"ЦПО към {clientCourse.Course.CandidateProvider.ProviderOwner}",
                    FullName = clientCourse.FullName,
                    Profession = clientCourse.Course.Program.Speciality.Profession.CodeAndName,
                    Speciality = clientCourse.Course.Program.Speciality.CodeAndName,
                    CourseName = clientCourse.Course.CourseName,
                    Period = clientCourse.Course.StartDate.HasValue && clientCourse.Course.EndDate.HasValue ? clientCourse.Course.Period : string.Empty,
                    Location = clientCourse.Course.Location.LocationName,
                    TrainingTypeName = clientCourse.Course.IdTrainingCourseType.HasValue ? (await this.DataSourceService.GetKeyValueByIdAsync(clientCourse.Course.IdTrainingCourseType.Value))?.Name ?? string.Empty : string.Empty,
                    Status = clientCourseDocFromDb.IdDocumentStatus.HasValue ? (await this.DataSourceService.GetKeyValueByIdAsync(clientCourseDocFromDb.IdDocumentStatus.Value))?.Name ?? string.Empty : string.Empty,
                    IdSex = clientCourse.IdSex,
                    IdNationality = clientCourse.IdNationality,
                    IdEducation = clientCourse.IdEducation,
                    IdDocumentType = clientCourseDocFromDb.IdDocumentType,
                    FirstName = clientCourse.FirstName,
                    SecondName = clientCourse.SecondName ?? string.Empty,
                    FamilyName = clientCourse.FamilyName,
                    ProviderPerson = clientCourse.Course.CandidateProvider.PersonNameCorrespondence ?? string.Empty,
                    ProviderPhone = clientCourse.Course.CandidateProvider.ProviderPhoneCorrespondence ?? string.Empty,
                    ProviderEmail = clientCourse.Course.CandidateProvider.ProviderEmailCorrespondence ?? string.Empty,
                    ProviderAddress = clientCourse.Course.CandidateProvider.ProviderAddressCorrespondence ?? string.Empty,
                    Indent = clientCourse.Indent ?? string.Empty,
                    DocumentRegNo = clientCourseDocFromDb.DocumentRegNo ?? string.Empty,
                    DocumentDateAsStr = clientCourseDocFromDb.DocumentDateAsStr
                };

                await this.documentsFromCPOModal.OpenModal(model);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            this.SpinnerShow();
            
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = this.clientsGrid.PageSettings.PageSize;
                this.clientsGrid.PageSettings.PageSize = this.clientsSource.Count();
                await this.clientsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;

                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportProperties.Columns = ExportColumns;
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "Course.CandidateProvider.LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Course.CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FullName", HeaderText = "Име на курсист", Width = "180", TextAlign = TextAlign.Left });                
                ExportColumns.Add(new GridColumn() { Field = "Course.Program.Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Course.Program.Speciality.ComboBoxName", HeaderText = "Специалност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Course.CourseName", HeaderText = "Курс", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Course.timeSpan", HeaderText = "Период на провеждане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Course.Location.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Course.TrainingCourseTypeName", HeaderText = "Вид на обучение", Width = "80", TextAlign = TextAlign.Left });

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
                ExportProperties.FileName = $"Trained_People_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.clientsGrid.ExportToPdfAsync(ExportProperties);
                this.clientsGrid.PageSettings.PageSize = temp;
                await this.clientsGrid.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
               // ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.FileName = $"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.clientsGrid.ExportToExcelAsync(ExportProperties);
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
                sheet.Range["B1"].Text = "ЦПО";
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
                foreach (var item in this.clientsSource)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.Course.CandidateProvider.LicenceNumber; 
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.Course.CandidateProvider.CPONameOwnerGrid;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.FullName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.Course.Program.Speciality.Profession.CodeAndName;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.Course.Program.Speciality.ComboBoxName;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.Course.CourseName;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.Course.timeSpan;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.Course.Location.LocationName;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"I{rowCounter}"].Text = item.Course.TrainingCourseTypeName == null ? "" : item.Course.TrainingCourseTypeName;
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
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<ClientCourseVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(this.clientsGrid, args.Data.IdClientCourse).Result.ToString();
            }
        }
        public async Task FilterGrid()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.filterModal.OpenModal("TrainedPeopleList");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async void OnFilterModalSubmit(List<ClientCourseVM> filteredClients)
        {
            var courseFinishedTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseFinishedType");
            this.clientsSource = filteredClients.ToList();

            foreach (var client in clientsSource)
            {
                if (client.IdFinishedType.HasValue)
                {
                    var courseFinishedType = courseFinishedTypeSource.FirstOrDefault(x => x.IdKeyValue == client.IdFinishedType.Value);
                    if (courseFinishedType is not null)
                    {
                        client.FinishedTypeName = courseFinishedType.Name;
                    }
                }
            }

            this.StateHasChanged();
        }
        public async Task OpenViewClientDocumenttBtn(ClientCourseVM clientCourseVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var courseStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CourseStatus");
                await this.currentCourseClientModal.OpenModal(clientCourseVM, clientCourseVM.Course, new List<ClientCourseVM>(), courseStatusSource, true, null, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
        public async Task OpenViewClientCourseBtn(ClientCourseVM clientCourseVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }

            try
            {
                this.loading = true;

                var courseFromDb = await this.TrainingService.GetTrainingCourseByIdAsync(clientCourseVM.Course.IdCourse);

                var kvCourseTypeSPK = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "ProfessionalQualification");
                
                await this.currentTrainingCourseModal.OpenModal(courseFromDb, null, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
