using Data.Models.Data.Candidate;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;
using Syncfusion.XlsIO;
using Microsoft.JSInterop;
using Syncfusion.PdfExport;
using DocuWorkService;
using DocuServiceReference;
using DocuWorkService;
using Data.Models.Data.ProviderData;
using ISNAPOO.Core.ViewModels.Control;
using DocuWorkService.ViewModel;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;

namespace ISNAPOO.WebSystem.Pages.Registers
{
    public partial class ProviderLicenceListModal : BlazorBaseComponent
    {
        List<CandidateProviderVM> candidateProviderVMs;
        CandidateProviderVM candidateProvider = new CandidateProviderVM();
        SfGrid<CandidateProviderLicenceChangeVM> sfGridModal = new SfGrid<CandidateProviderLicenceChangeVM>();
        List<string> validationMessages = new List<string>();
        ProviderLicenceChangeModal providerLicenceChangeModal = new ProviderLicenceChangeModal();
        private IEnumerable<CandidateProviderLicenceChangeVM> candidateProviderLicenceChangeVM, candidateProviderLicenceChangeOrig;
        private IEnumerable<KeyValueVM> kvLicenseChangeStatus = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvProcedureDoducments = new List<KeyValueVM>();
        List<ApplicationUser> appUsers = new List<ApplicationUser>();
        private SfDialog sfDialog;
        private string cpoNames = string.Empty;
        //  ToastMsg toast = new ToastMsg();
        private string title = string.Empty;
        private int idCandidatProvider = 0;
        private bool isVisibleLicenceChangeButton = true;
        private string fileName = $"Promeni_v_licenziyta_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDocuService docuService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IPersonService personService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }


        protected override async Task OnInitializedAsync()
        {
            //this.editContext = new EditContext(this.candidateProviderLicenceChangeVM);
        }

        private async Task OpenNewCandidateProviderLicenceModal()
        {         
            CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM = new CandidateProviderLicenceChangeVM();
            candidateProviderLicenceChangeVM.IdCandidate_Provider = idCandidatProvider;
            await this.providerLicenceChangeModal.OpenModal(candidateProviderLicenceChangeVM, candidateProviderLicenceChangeOrig, appUsers);
        }
        private async Task OpenProviderLicenceChangeModal(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM)
        {
           
            await this.providerLicenceChangeModal.OpenModal(candidateProviderLicenceChangeVM, candidateProviderLicenceChangeOrig, appUsers);
        }

        private  async Task<List<ApplicationUser>> GetActiveUsersByIdCandidateProvider()
        {
            var kvStatusActiveUser = await this.dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");
            var activeApplicationUsers = await this.personService.GetPersonByIdCandidateProvider(this.idCandidatProvider, kvStatusActiveUser.IdKeyValue);
            return activeApplicationUsers;
        }
        public async Task OpenModal(CandidateProviderVM candidateProviderVM, string cpoOrCipo)
        {
            this.validationMessages.Clear();
            this.isVisibleLicenceChangeButton = true;
            this.idCandidatProvider = candidateProviderVM.IdCandidate_Provider;
            this.appUsers = new List<ApplicationUser>();
            //this.candidateProvider = candidateProviderVM;
            if (cpoOrCipo == "ЦПО")
            {
                this.title = $"<span style=\"color: #ffffff;\">{candidateProviderVM.CPONameOwnerGrid}</span>";
            }
            else if (cpoOrCipo == "ЦИПО")
            {
                this.title = $"<span style=\"color: #ffffff;\">{candidateProviderVM.CIPONameOwnerGrid}</span>";
            }

            this.kvLicenseChangeStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus");
            this.kvProcedureDoducments = await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ProcedureDocumentType");

            var kvDefinitivelyTakenAway = this.kvLicenseChangeStatus.FirstOrDefault(x => x.KeyValueIntCode == "DefinitivelyTakenAway");           
            if (kvDefinitivelyTakenAway is not null && candidateProviderVM.IdLicenceStatus == kvDefinitivelyTakenAway.IdKeyValue)
            {
                this.isVisibleLicenceChangeButton = false;
            }
            appUsers = await GetActiveUsersByIdCandidateProvider();

            await LoadCandidateProviderLicensesList();
        }

        private async  Task LoadCandidateProviderLicensesList() 
        {
            this.candidateProviderLicenceChangeVM = await this.CandidateProviderService.GetCandidateProviderLicensesListByIdAsync(this.idCandidatProvider);
            this.candidateProviderLicenceChangeOrig = candidateProviderLicenceChangeVM;

            await this.CallbackAfterSubmit.InvokeAsync();
           
            this.isVisible = true;
            this.StateHasChanged();
            
        }
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CandidateProviderLicenceChangeVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGridModal, args.Data.IdCandidateProviderLicenceChange).Result.ToString();
            }
        }
        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            this.SpinnerShow();

            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGridModal.PageSettings.PageSize;
                sfGridModal.PageSettings.PageSize = candidateProviderLicenceChangeVM.Count();
                await sfGridModal.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfTheme Theme = new PdfTheme();
                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenceStatusName", HeaderText = "Статус на лицензията", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "NumberCommand", HeaderText = "Заповед", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ChangeDate", HeaderText = "Дата", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "LicenceStatusDetailName", HeaderText = "Вид на промяната", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Notes", HeaderText = "Бележки", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Archive", HeaderText = "Съхранение на архива", Width = "80", TextAlign = TextAlign.Left });
  
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
                ExportProperties.FileName = fileName + ".pdf";

                await this.sfGridModal.ExportToPdfAsync(ExportProperties);
                sfGridModal.PageSettings.PageSize = temp;
                await sfGridModal.Refresh();
            }
            else if (args.Item.Id == "sfGrid_excelexport")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
                //ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenceStatusName", HeaderText = "Статус на лицензията", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "NumberCommand", HeaderText = "Заповед", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ChangeDate", HeaderText = "Дата", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "LicenceStatusDetailName", HeaderText = "Вид на промяната", Format = "d", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Notes", HeaderText = "Бележки", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "Archive", HeaderText = "Съхранение на архива", Width = "80", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = fileName + ".xlsx";
                await this.sfGridModal.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs(fileName + ".csv", result.ToArray());
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
                sheet.Range["A1"].Text = "Статус на лицензията";
                sheet.Range["B1"].ColumnWidth = 120;
                sheet.Range["B1"].Text = "Заповед";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Дата";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Бележки";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Съхранение на архива";

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

                var rowCounter = 2;
                foreach (var item in candidateProviderLicenceChangeVM)
                {
                    var newDate = item.ChangeDate.Value.ToString("dd.MM.yyyy");
                    sheet.Range[$"A{rowCounter}"].Text = item.LicenceStatusName;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"B{rowCounter}"].Text = item.NumberCommand;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = newDate + " г.";
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.Notes;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.Archive;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"Е{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }

                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }
        public async Task GetDocument(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                string guid = string.Empty;
                FileData[] files;
                var VidCode1 = this.kvProcedureDoducments.Where(x => x.KeyValueIntCode.Equals("Application19")).First();
                var VidCode2 = this.kvProcedureDoducments.Where(x => x.KeyValueIntCode.Equals("Application19a")).First();
                var VidCode3 = this.kvProcedureDoducments.Where(x => x.KeyValueIntCode.Equals("CIPO_Application19")).First();
       
                var DocumentParameters = new GetDocumentVM()
                {
                    DocID = candidateProviderLicenceChangeVM.DS_ID,
                    GUID = candidateProviderLicenceChangeVM.DS_GUID,
                    OfficialDocID = candidateProviderLicenceChangeVM.DS_OFFICIAL_ID,
                    OfficialGUID = candidateProviderLicenceChangeVM.DS_OFFICIAL_GUID,
                    DocDate = candidateProviderLicenceChangeVM.DS_DATE,
                    DocNumber = candidateProviderLicenceChangeVM.DS_DocNumber,
                    OfficialDocDate = candidateProviderLicenceChangeVM.DS_OFFICIAL_DATE,
                    OfficialDocNumber = candidateProviderLicenceChangeVM.DS_OFFICIAL_DocNumber,
                    VidCodes = new int[] {Int32.Parse(VidCode1.DefaultValue6), Int32.Parse(VidCode2.DefaultValue6), Int32.Parse(VidCode3.DefaultValue6) }
                };

                var contextResponse = await this.docuService.CheckAndGetDocument(DocumentParameters);

                if (contextResponse.HasErrorMessages)
                {
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, contextResponse.ListErrorMessages));

                    return;
                }

                var doc = contextResponse.ResultContextObject;

                files = doc.File;
                    guid = doc.GUID;

                    if (files == null || files.Count() == 0)
                    {
                       // this.SpinnerHide();
                        await this.ShowErrorAsync("За въведеният номер на заповед няма прикачен файл в деловодната система!");
                    }
                    else
                    {
                        foreach (var file in files)
                        {
                            var fileResponse = await docuService.GetFileAsync(file.FileID, guid);

                            await FileUtils.SaveAs(JsRuntime, file.Filename, fileResponse.File.BinaryContent.ToArray());
                        }
                    }
                                                                                  
            }
            finally
            {
                this.loading = false;
                //this.SpinnerHide();
            }

            this.SpinnerHide();
        }
    }
}
