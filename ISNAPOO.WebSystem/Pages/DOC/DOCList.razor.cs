using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.Contracts.DOC;
using ISNAPOO.Core.ViewModels.DOC;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.DOC.NKPD;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using Syncfusion.Blazor.Grids;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.HelperClasses;

namespace ISNAPOO.WebSystem.Pages.DOC
{
    public partial class DOCList : BlazorBaseComponent
    {
        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDOCService docService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public INKPDService nkpdService { get; set; }

        ToastMsg toast;
        IEnumerable<DocVM> dataSource;
        IEnumerable<NKPDVM> nkpdSource;
        SfGrid<DocVM> sfGrid = new SfGrid<DocVM>();
        DocVM model = new DocVM();
        public bool IsFilterVisible { get; set; } = false;
        public bool IsSearchConfirmed { get; set; } = false;
        private FilterDOC filterModel = new FilterDOC();
        EditDOC editDOC = new EditDOC();
        ImportDOC importDOCmodal = new ImportDOC();
        private string fileName = $"DOS_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
        DocVM docToDelete = new DocVM();

        protected override async Task OnInitializedAsync()
        {

            this.dataSource = await this.docService.GetAllDocAsync();
            this.nkpdSource = await this.nkpdService.GetAllNKPDOnlyAsync();
        }

        private async void SelectedRow(DocVM _model)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                bool hasPermission = await CheckUserActionPermission("ViewDOCData", false);
                if (!hasPermission) { return; }

                this.model = await this.docService.GetDOCByIdAsync(_model);

                await this.editDOC.OpenModal(this.model);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OpenAddNewModal()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
                if (!hasPermission) { return; }

                this.model = new DocVM();
                var draftStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Draft");

                this.model.IdStatus = draftStatus.IdKeyValue;


                await this.editDOC.OpenModal(this.model);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task DeleteRowDOC(DocVM doc)
        {
            bool hasPermission = await CheckUserActionPermission("ManageDOCData", false);
            if (!hasPermission) { return; }

            this.docToDelete = doc;

            string msg = "Сигурни ли сте, че искате да изтриете избрания запис?";
            bool confirmed = await this.ShowConfirmDialogAsync(msg);
            if (confirmed)
            {
                var draftStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("StatusSPPOO", "Draft");

                if (doc.IdStatus == draftStatus.IdKeyValue)
                {
                    var result = await this.docService.DelteDocById(doc.IdDOC);
                    if (result > 0)
                    {
                        this.toast.sfSuccessToast.Content = "Записът е изтрит успешно!";
                        await toast.sfSuccessToast.ShowAsync();
                    }

                    await FilterGrid(false);
                }
                else
                {
                    toast.sfErrorToast.Content = "Не може да се изтрие ДОС който не е на статус Работен!";
                    await toast.sfErrorToast.ShowAsync();
                }
            }

        }

        private async Task ImportDOS()
        {
            await this.importDOCmodal.OpenModal();
        }

        private async Task<Task> UpdateAfterSave(DocVM _model)
        {
            await FilterGrid(false);
            return Task.CompletedTask;
        }


        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = dataSource.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "Name", HeaderText = "ДОС", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StartDateOnly", HeaderText = "В сила от", Width = "80", Format = "d", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Profession.ComboBoxName", HeaderText = "Професия", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "SpecialitiesJoin", HeaderText = "Специалности", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "StatusName", HeaderText = "Статус", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequirementsCandidates", HeaderText = "Изисквания към кандидатите", Width = "60", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "DescriptionProfession", HeaderText = "Описание на професията", Width = "60", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequirementsMaterialBase", HeaderText = "Изисквания към материалната база", Width = "60", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "RequirementsТrainers", HeaderText = "Изисквания към обучаващите", Width = "60", TextAlign = TextAlign.Left });
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

                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs(fileName + ".xlsx", result.ToArray());
            }
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<DocVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdDOC).Result.ToString();
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
                sheet.Range["A1"].Text = "ДОС";
                sheet.Range["B1"].ColumnWidth = 10;
                sheet.Range["B1"].Text = "В сила от";
                sheet.Range["C1"].ColumnWidth = 60;
                sheet.Range["C1"].Text = "Професия";
                sheet.Range["D1"].ColumnWidth = 60;
                sheet.Range["D1"].Text = "Специалности";
                sheet.Range["E1"].ColumnWidth = 10;
                sheet.Range["E1"].Text = "Статус";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Изисквания към кандидатите";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "Описание на професията";
                sheet.Range["H1"].ColumnWidth = 120;
                sheet.Range["H1"].Text = "Изисквания към материалната база";
                sheet.Range["I1"].ColumnWidth = 120;
                sheet.Range["I1"].Text = "Изисквания към обучаващите";

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
                dataSource = dataSource.OrderBy(d => d.Name).ToList();
                foreach (var item in dataSource)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.Name;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 20;
                    sheet.Range[$"B{rowCounter}"].Text = item.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT);
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.Profession.ComboBoxName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.SpecialitiesJoin;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.StatusName;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.RequirementsCandidates;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.DescriptionProfession;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.RequirementsMaterialBase;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"I{rowCounter}"].Text = item.RequirementsТrainers;
                    sheet.Range[$"I{rowCounter}"].WrapText = true;
                    sheet.Range[$"I{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream;
                }
            }
        }
        private async Task ClearFilter()
        {
            filterModel = new FilterDOC();
            dataSource = await this.docService.GetAllDocAsync();
            this.sfGrid.Refresh();
            this.StateHasChanged();
        }
        private async Task FilterGrid(bool IsModal = true)
        {
            bool hasPermission = await CheckUserActionPermission("ViewNomenclaturesData", false);
            if (!hasPermission) { return; }
            if (IsModal)
            {
                this.IsFilterVisible = !this.IsFilterVisible;
            }
            if (IsSearchConfirmed || !IsModal)
            {
                var allDocs = await this.docService.GetAllDocAsync();
                if (filterModel.Regulation != null || filterModel.NKPDIds.Count != 0 || filterModel.NKPDIds != null || filterModel.EruName != null || filterModel.IsDOI)
                {
                    allDocs = allDocs.Where(d => (!string.IsNullOrEmpty(filterModel.Regulation) ? (d.Regulation.ToLower() == filterModel.Regulation.ToLower()) || (d.Regulation.ToLower().Contains(filterModel.Regulation.ToLower())) : true)
                   && (filterModel.NKPDIds != null && filterModel.NKPDIds.Count != 0 ? filterModel.NKPDIds.All(y => d.DOCNKPDs.Any(n => n.IdNKPD == y)) : true)
                   && (!string.IsNullOrEmpty(filterModel.EruName) ? d.ERUs.Any(e => (e.Name.ToLower() == filterModel.EruName.ToLower()) || (e.Name.ToLower().Contains(filterModel.EruName.ToLower()))) : true && (filterModel.IsDOI ? d.IsDOI : true))
                   ).ToList();
                }
                dataSource = allDocs;


            }
            IsSearchConfirmed = false;
            this.StateHasChanged();
        }
        public class FilterDOC
        {
            public FilterDOC()
            {
                NKPDIds = new List<int>();
            }

            public string Regulation { get; set; }

            public string EruName { get; set; }

            public List<int> NKPDIds { get; set; }

            public bool IsDOI { get; set; }
        }
    }
}
