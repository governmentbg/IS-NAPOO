using System;
using Data.Models.Data.Request;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Request;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.ProfessionalEducationCertificates
{
    public partial class ProfessionalCertificateList : BlazorBaseComponent
    {
        SfGrid<ClientCourseDocumentVM> sfGrid;

        List<ClientCourseDocumentVM> documents = new List<ClientCourseDocumentVM>();

        DocumentsFromCPOFilter filterModal;

        DocumentsFromCPOModal modal;

        KeyValueVM keyValue;

        TypeOfRequestedDocumentVM typeOfRequestedDocument1;

        TypeOfRequestedDocumentVM typeOfRequestedDocument2;

        string Type;

        [Inject]
        ITrainingService trainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }
        [Inject]
        public IProviderDocumentRequestService providerDocumentRequestService { get; set; }
        [Inject]
        public ICommonService commonService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            keyValue = await dataSourceService.GetKeyValueByIntCodeAsync("TypeFrameworkProgram", "PartProfession");
            typeOfRequestedDocument1 = await this.providerDocumentRequestService.GetTypeOfRequestedDocumentAsyncByDocTypeOfficialNumber("3-114");
            typeOfRequestedDocument2 = await this.providerDocumentRequestService.GetTypeOfRequestedDocumentAsyncByDocTypeOfficialNumber("3-116");
            Type = string.Empty;
            await HandleTokenData();
            // documents = (await trainingService.GetAllDocuments()).Where(x => x.ClientCourse.Course.Program.IdCourseType == keyValue.IdKeyValue).ToList();

            //var mm = new ClientCourseDocumentVM();

            //mm.ClientCourse = new ClientCourseVM();
            //mm.ClientCourse.Course = new CourseVM() { IdCandidateProvider = 251764 };
            //mm.ClientCourse.Client = new ClientVM() { IdCandidateProvider = 251764 };


            //documents = (await trainingService.GetAllProfessionalCertificateDocuments(mm)).ToList();
        }
        public async Task HandleTokenData()
        {
            if (!string.IsNullOrEmpty(this.Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.commonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    var type = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.ENTRY_FROM).Value.ToString();

                    if (type!.Equals("SP"))
                    {
                        this.Type = type;
                    }
                    else if (type.Equals("PP"))
                    {
                        this.Type = type;
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
        public async Task openModal(ClientCourseDocumentVM clientCourseDocumentVM)
        {
            if (loading) return;

            try
            {
                loading = true;

                var model = new DocumentsFromCPORegisterVM()
                {
                    IsCourse = true,
                    IdEntity = clientCourseDocumentVM.IdClientCourseDocument,
                    LicenceNumber = clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.LicenceNumber ?? string.Empty,
                    CPONameOwnerGrid = !string.IsNullOrEmpty(clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderName) ? $"ЦПО {clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderName} към {clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderOwner}" : $"ЦПО към {clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderOwner}",
                    FullName = clientCourseDocumentVM.ClientCourse.FullName,
                    Profession = clientCourseDocumentVM.ClientCourse.Course.Program.Speciality.Profession.CodeAndName,
                    Speciality = clientCourseDocumentVM.ClientCourse.Course.Program.Speciality.CodeAndName,
                    CourseName = clientCourseDocumentVM.ClientCourse.Course.CourseName,
                    Period = clientCourseDocumentVM.ClientCourse.Course.StartDate.HasValue && clientCourseDocumentVM.ClientCourse.Course.EndDate.HasValue ? clientCourseDocumentVM.ClientCourse.Course.Period : string.Empty,
                    Location = clientCourseDocumentVM.ClientCourse.Course.IdLocation.HasValue ? (await this.LocationService.GetLocationByIdAsync(clientCourseDocumentVM.ClientCourse.Course.IdLocation.Value)).LocationName : string.Empty,
                    TrainingTypeName = clientCourseDocumentVM.ClientCourse.Course.IdTrainingCourseType.HasValue ? (await this.dataSourceService.GetKeyValueByIdAsync(clientCourseDocumentVM.ClientCourse.Course.IdTrainingCourseType.Value))?.Name ?? string.Empty : string.Empty,
                    Status = clientCourseDocumentVM.IdDocumentStatus.HasValue ? (await this.dataSourceService.GetKeyValueByIdAsync(clientCourseDocumentVM.IdDocumentStatus.Value))?.Name ?? string.Empty : string.Empty,
                    IdSex = clientCourseDocumentVM.ClientCourse.IdSex,
                    IdNationality = clientCourseDocumentVM.ClientCourse.IdNationality,
                    IdEducation = clientCourseDocumentVM.ClientCourse.IdEducation,
                    IdDocumentType = clientCourseDocumentVM.IdDocumentType,
                    FirstName = clientCourseDocumentVM.ClientCourse.FirstName,
                    SecondName = clientCourseDocumentVM.ClientCourse.SecondName ?? string.Empty,
                    FamilyName = clientCourseDocumentVM.ClientCourse.FamilyName,
                    ProviderPerson = clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.PersonNameCorrespondence ?? string.Empty,
                    ProviderPhone = clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderPhoneCorrespondence ?? string.Empty,
                    ProviderEmail = clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderEmailCorrespondence ?? string.Empty,
                    ProviderAddress = clientCourseDocumentVM.ClientCourse.Course.CandidateProvider.ProviderAddressCorrespondence ?? string.Empty,
                    Indent = clientCourseDocumentVM.ClientCourse.Indent ?? string.Empty,
                    DocumentRegNo = clientCourseDocumentVM.DocumentRegNo ?? string.Empty,
                    DocumentDateAsStr = clientCourseDocumentVM.DocumentDateAsStr
                };

                await this.modal.OpenModal(model);
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
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.LicenceNumberWithDate", HeaderText = "Лицензия", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FullName", HeaderText = "Име на курсист", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS", HeaderText = "Специалност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.CourseName", HeaderText = "Курс", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.timeSpan", HeaderText = "Период на провеждане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Location.LocationName", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.CourseType.Name", HeaderText = "Вид на обучение", Width = "80", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.IncludeTemplateColumn = true;
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
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.LicenceNumberWithDate", HeaderText = "Лицензия", Width = "200", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Client.CandidateProvider.CPONameOwnerGrid", HeaderText = "ЦПО", Width = "100", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.FullName", HeaderText = "Име на курсист", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.Profession.CodeAndName", HeaderText = "Професия", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.Speciality.CodeAndNameAndVQS", HeaderText = "Специалност", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.CourseName", HeaderText = "Курс", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.timeSpan", HeaderText = "Период на провеждане", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Location.LocationName", HeaderText = "Населено място", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ClientCourse.Course.Program.CourseType.Name", HeaderText = "Вид на обучение", Width = "200", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.IncludeTemplateColumn = true;
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
                foreach (var item in documents)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.ClientCourse.Client.CandidateProvider.LicenceNumberWithDate;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.ClientCourse.FullName; 
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = $"ЦПО {item.ClientCourse.Client.CandidateProvider.ProviderName} към {item.ClientCourse.Client.CandidateProvider.ProviderOwner}";
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

        public async Task FilterGrid()
        {
            await this.filterModal.OpenModal("ProfessionalCertificateList");
        }

        public async Task Filter(TrainedPeopleFilterVM model)
        {
            switch(this.Type)
            {
                case "PP":
                    documents = (await trainingService.GetAllProfessionalCertificateDocuments(model)).Where(x => x.ClientCourse.Course.Program.IdCourseType == keyValue.IdKeyValue).ToList();
                    break;
                case "SP":
                    model.TypeOfRequestDocuments = new List<int?>() {typeOfRequestedDocument1.IdTypeOfRequestedDocument, typeOfRequestedDocument2.IdTypeOfRequestedDocument };
                    documents = (await trainingService.GetAllProfessionalCertificateDocuments(model)).ToList();
                    break;
            }
        }
    }
}
