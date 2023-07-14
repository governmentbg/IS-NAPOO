using Data.Models.Data.DOC;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Training.SPKAndPoP;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.DocIO.DLS;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers.DocumentsFromCPO
{
    public partial class DocumentsFromCPOList : BlazorBaseComponent
    {
        private ChangeRIDPKDocumentStatusModal changeRIDPKDocumentStatusModal = new ChangeRIDPKDocumentStatusModal();
        private DocumentStatusModal documentStatusModal = new DocumentStatusModal();
        private SfGrid<DocumentsFromCPORegisterVM> documentsGrid = new SfGrid<DocumentsFromCPORegisterVM>();

        private DocumentsFromCPOModal documentsFromCPOModal = new DocumentsFromCPOModal();
        private DocumentsFromCPOFilter filterModal = new DocumentsFromCPOFilter();

        private IEnumerable<DocumentsFromCPORegisterVM> documentsSource = new List<DocumentsFromCPORegisterVM>();
        private string initialFilterValue = "Вписан в Регистъра";

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService UploadFileService { get; set; }

        public async Task OpenViewClientBtn(DocumentsFromCPORegisterVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                
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

            if (args.Item.Id.Contains("pdf"))
            {
                int temp = this.documentsGrid.PageSettings.PageSize;
                this.documentsGrid.PageSettings.PageSize = this.documentsSource.Count();
                await this.documentsGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = Syncfusion.Blazor.Grids.PageOrientation.Landscape;
                PdfTheme Theme = new PdfTheme();

                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FullName", HeaderText = "Име на курсист", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Profession", HeaderText = "Професия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Speciality", HeaderText = "Специалност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CourseName", HeaderText = "Курс", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Period", HeaderText = "Период на провеждане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Location", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TrainingTypeName", HeaderText = "Вид на обучение", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Status", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });

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

                await this.documentsGrid.ExportToPdfAsync(ExportProperties);
                this.documentsGrid.PageSettings.PageSize = temp;
                await this.documentsGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();

                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CPONameOwnerGrid", HeaderText = "ЦПО", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "FullName", HeaderText = "Име на курсист", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Profession", HeaderText = "Професия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Speciality", HeaderText = "Специалност", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CourseName", HeaderText = "Курс", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Period", HeaderText = "Период на провеждане", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Location", HeaderText = "Населено място", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TrainingTypeName", HeaderText = "Вид на обучение", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Status", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = $"Documents_from_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                await this.documentsGrid.ExportToExcelAsync(ExportProperties);
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
                foreach (var item in this.documentsSource)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.LicenceNumber; 
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.CPONameOwnerGrid; 
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.FullName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.Profession;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.Speciality;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.CourseName;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.Period;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.Location;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"I{rowCounter}"].Text = item.TrainingTypeName;
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

        private async Task ChangeStatusBtn()
        {
            var selectedDocuments = await this.documentsGrid.GetSelectedRecordsAsync();
            if (!selectedDocuments.Any())
            {
                await this.ShowErrorAsync("Моля, изберете поне един документ преди да смените статуса!");
                return;
            }

            var isCourseEntry = selectedDocuments.FirstOrDefault(x => x.IsCourse);
            var isValidationEtry = selectedDocuments.FirstOrDefault(x => !x.IsCourse);
            if (isCourseEntry is not null && isValidationEtry is not null)
            {
                await this.ShowErrorAsync("Моля, изберете издадени документи от един и същи вид на обучение преди да смените статуса!");
                return;
            }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var listIds = selectedDocuments.Select(x => x.IdEntity).ToList();
                await this.changeRIDPKDocumentStatusModal.OpenModal(listIds, selectedDocuments.FirstOrDefault()!.IsCourse);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenFileWithDocumentBtn(DocumentsFromCPORegisterVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (model.IsCourse)
                {
                    var clientCourseDocFromDb = await this.TrainingService.GetClientCourseDocumentWithUploadedFilesByIdAsync(model.IdEntity);
                    if (clientCourseDocFromDb.OldId.HasValue)
                    {
                        if (!clientCourseDocFromDb.CourseDocumentUploadedFiles.Any())
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await this.ShowErrorAsync(msg);
                        }
                        else
                        {
                            var docs = clientCourseDocFromDb.CourseDocumentUploadedFiles;
                            if (docs.Any())
                            {
                                foreach (var doc in docs)
                                {
                                    if (!string.IsNullOrEmpty(doc.UploadedFileName))
                                    {
                                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                                        if (hasFile)
                                        {
                                            var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(doc.IdCourseDocumentUploadedFile);
                                            if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                            }
                                            else
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, doc.FileName, document.MS!.ToArray());
                                            }
                                        }
                                        else
                                        {
                                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                            await this.ShowErrorAsync(msg);
                                        }
                                        
                                    }
                                }
                            }
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                        }
                    }
                    else
                    {
                        var uploadedFile = clientCourseDocFromDb.CourseDocumentUploadedFiles.FirstOrDefault();
                        if (uploadedFile is not null)
                        {
                            if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                            {
                                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<CourseDocumentUploadedFile>(uploadedFile.IdCourseDocumentUploadedFile);
                                if (hasFile)
                                {
                                    var document = await this.UploadFileService.GetUploadedFileAsync<CourseDocumentUploadedFile>(uploadedFile.IdCourseDocumentUploadedFile);
                                    if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                    }
                                    else
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, document.MS!.ToArray());
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
                }
                else
                {
                    var validationClientDocFromDb = await this.TrainingService.GetValidationClientDocumentWithUploadedFilesByIdAsync(model.IdEntity);
                    if (validationClientDocFromDb.OldId.HasValue)
                    {
                        if (!validationClientDocFromDb.ValidationDocumentUploadedFiles.Any())
                        {
                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await this.ShowErrorAsync(msg);
                        }
                        else
                        {
                            var docs = validationClientDocFromDb.ValidationDocumentUploadedFiles;
                            if (docs.Any())
                            {
                                foreach (var doc in docs)
                                {
                                    if (!string.IsNullOrEmpty(doc.UploadedFileName))
                                    {
                                        var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                                        if (hasFile)
                                        {
                                            var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(doc.IdValidationDocumentUploadedFile);
                                            if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                            }
                                            else
                                            {
                                                await FileUtils.SaveAs(this.JsRuntime, doc.FileName, document.MS!.ToArray());
                                            }
                                        }
                                        else
                                        {
                                            var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                            await this.ShowErrorAsync(msg);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await this.ShowErrorAsync(msg);
                            }
                        }
                    }
                    else
                    {
                        var uploadedFile = validationClientDocFromDb.ValidationDocumentUploadedFiles.FirstOrDefault();
                        if (uploadedFile is not null)
                        {
                            if (!string.IsNullOrEmpty(uploadedFile.UploadedFileName))
                            {
                                var hasFile = await this.UploadFileService.CheckIfExistUploadedFileAsync<ValidationDocumentUploadedFile>(uploadedFile.IdValidationDocumentUploadedFile);
                                if (hasFile)
                                {
                                    var document = await this.UploadFileService.GetUploadedFileAsync<ValidationDocumentUploadedFile>(uploadedFile.IdValidationDocumentUploadedFile);
                                    if (!string.IsNullOrEmpty(document.FileNameFromOldIS))
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, document.FileNameFromOldIS, document.MS!.ToArray());
                                    }
                                    else
                                    {
                                        await FileUtils.SaveAs(this.JsRuntime, uploadedFile.FileName, document.MS!.ToArray());
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
                                var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                                await ShowErrorAsync(msg);
                            }
                        }
                        else
                        {
                            var msg = LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                            await ShowErrorAsync(msg);
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenStatusHistoryBtn(DocumentsFromCPORegisterVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.documentStatusModal.OpenModal(model.IdEntity, model.IsCourse ? "Course" : "Validation");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
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

                await this.filterModal.OpenModal("DocumentsFromCPOList");
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task UpdateAfterFilterAsync(List<DocumentsFromCPORegisterVM> documents)
        {
            this.documentsSource = documents.ToList();

            await this.documentsGrid.Refresh();
            this.StateHasChanged();
        }

        private async Task OnChangeStatusModalSubmitAsync(List<int> updatedDocIds)
        {
            var docsFromSource = this.documentsSource.Where(x => updatedDocIds.Contains(x.IdEntity));
            await this.TrainingService.SetDocumentStatusForListDocumentsFromCPORegisterVMAsync(docsFromSource);

            this.StateHasChanged();
        }
    }
}

