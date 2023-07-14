using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Data.Models.Common;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using RegiX;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class RegistrationFormList : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();

        
        private int MAX_TEXT_COUNT = 512;
        private ToastMsg toast;
        private IEnumerable<CandidateProviderVM> candidateProviders = Enumerable.Empty<CandidateProviderVM>();
        private LocationVM locationVM = new LocationVM();
        private string LicenseType = string.Empty;
        private string fileName = $"Registration_Form_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
        private ShowRegixDataModal regixData = new ShowRegixDataModal();
        private RejectReason model = new RejectReason();
        private KeyValueVM kvApprovedRegistration = new KeyValueVM();
        private KeyValueVM kvAwaitingRegistration = new KeyValueVM();
        private KeyValueVM kvRejectRegistration = new KeyValueVM();

        [Inject]
        public ILocService LocService { get; set; }

        [Inject]
        public IUploadFileService uploadService { get; set; }

        [Inject]
        public IMailService mailService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        public IRegiXService RegiXService { get; set; }


        protected override async Task OnInitializedAsync()
        {
            //this.candidateProviders = await this.CandidateProviderService.GetAllCandidateProvidersAsync();
            this.editContext = new EditContext(this.model);
            this.kvApprovedRegistration = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "APPROVED_APPLICATION_FROM_NAPOO");
            this.kvAwaitingRegistration = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "AWAITING_CONFIRMATION_FROM_NAPOO");
            this.kvRejectRegistration = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "REJECTED_APPLICATION_FORM_NAPOO");
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.LicenseType = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.TOKEN_LICENSING_TYPE_KEY).Value.ToString();

                    var model = await SetLicenseTypeIdToModelForFilterAsync(this.LicenseType);
                    this.candidateProviders = await this.CandidateProviderService.GetAllExpertsAsync(model);
                    this.StateHasChanged();
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, currentContext.ListErrorMessages);
                    toast.sfErrorToast.ShowAsync();
                }
            }
        }

        public async Task<CandidateProviderVM> SetLicenseTypeIdToModelForFilterAsync(string licenseType)
        {
            var kvLicenseType = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync(GlobalConstants.TOKEN_LICENSING_TYPE_KEY);

            var vm = new CandidateProviderVM();
            if (licenseType == GlobalConstants.TOKEN_LICENSING_CPO_VALUE)
            {
                vm.IdTypeLicense = kvLicenseType.First(x => x.KeyValueIntCode == GlobalConstants.TOKEN_LICENSING_CPO_VALUE).IdKeyValue;
            }
            else if (licenseType == GlobalConstants.TOKEN_LICENSING_CIPO_VALUE)
            {
                vm.IdTypeLicense = kvLicenseType.First(x => x.KeyValueIntCode == GlobalConstants.TOKEN_LICENSING_CIPO_VALUE).IdKeyValue;
            }

            return vm;
        }

        private async Task OnDownloadClick(CandidateProviderVM model)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var hasFile = await this.uploadService.CheckIfExistUploadedFileAsync<CandidateProvider>(model.IdCandidate_Provider);
                if (hasFile)
                {
                    var documentStream = await this.uploadService.GetUploadedFileAsync<CandidateProvider>(model.IdCandidate_Provider);

                    if (!string.IsNullOrEmpty(documentStream.FileNameFromOldIS))
                    {
                        await FileUtils.SaveAs(this.JsRuntime, documentStream.FileNameFromOldIS, documentStream.MS!.ToArray());
                    }
                    else
                    {
                        await FileUtils.SaveAs(this.JsRuntime, model.FileName, documentStream.MS!.ToArray());
                    }
                }
                else
                {
                    var msg = this.LocService.GetLocalizedHtmlString("NotExistingFileForDownload").Value;

                    await this.ShowSuccessAsync(msg);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task ApproveRegistration()
        {
            bool hasPermission = await CheckUserActionPermission("ManageRegistrationFormData", false);
            if (!hasPermission) { return; }

            if (sfGrid.SelectedRecords.Count > GlobalConstants.INVALID_ID_ZERO)
            {
                ResultContext<List<CandidateProviderVM>> currentContext = new ResultContext<List<CandidateProviderVM>>();
                currentContext.ResultContextObject = new List<CandidateProviderVM>();
                currentContext.ResultContextObject.AddRange(sfGrid.SelectedRecords);

                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                foreach (var item in currentContext.ResultContextObject)
                {
                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("PoviderBulstat", item.PoviderBulstat) };

                    item.Token = this.CommonService.GetTokenWithParams(tokenContext, GlobalConstants.MINUTES_SIXTY);
                }

                currentContext = await this.CandidateProviderService.ApproveRegistrationAsync(currentContext);

                if (currentContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = string.Join("<br />", currentContext.ListMessages);
                    await toast.sfSuccessToast.ShowAsync();
                    currentContext.ListMessages.Clear();
                }
                else if (currentContext.HasErrorMessages)
                {
                    toast.sfErrorToast.Content = string.Join("<br />", currentContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    currentContext.ListErrorMessages.Clear();
                }

                var model = await SetLicenseTypeIdToModelForFilterAsync(this.LicenseType);
                this.candidateProviders = await this.CandidateProviderService.GetAllExpertsAsync(model);
                this.StateHasChanged();
            }
            else
            {
                toast.sfErrorToast.Content = "Моля първо изберете записи от таблицата!";
                await toast.sfErrorToast.ShowAsync();
            }
        }

        private async Task RejectRegistration()
        {
            bool hasPermission = await CheckUserActionPermission("ManageRegistrationFormData", false);
            if (!hasPermission) { return; }
            this.editContext = new EditContext(this.model);
            this.editContext.EnableDataAnnotationsValidation();
            bool isValid = this.editContext.Validate();
            if (isValid)
            {
                this.isVisible = false;
                this.SpinnerShow();
                ResultContext<List<CandidateProviderVM>> currentContext = new ResultContext<List<CandidateProviderVM>>();
                currentContext.ResultContextObject = new List<CandidateProviderVM>();
                currentContext.ResultContextObject.AddRange(sfGrid.SelectedRecords);
                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                foreach (var item in currentContext.ResultContextObject)
                {
                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("PoviderBulstat", item.PoviderBulstat) };

                    item.Token = this.CommonService.GetTokenWithParams(tokenContext, 1);
                }

                currentContext = await this.CandidateProviderService.RejectRegistrationAsync(currentContext, this.model.RejectionReason);

                if (currentContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = string.Join(Environment.NewLine, currentContext.ListMessages);
                    await toast.sfSuccessToast.ShowAsync();
                    currentContext.ListMessages.Clear();
                }
                else if (currentContext.HasErrorMessages)
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, currentContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    currentContext.ListErrorMessages.Clear();
                }

                var model = await SetLicenseTypeIdToModelForFilterAsync(this.LicenseType);
                this.candidateProviders = await this.CandidateProviderService.GetAllExpertsAsync(model);
                this.SpinnerHide();
                this.StateHasChanged();
            }
        }

        private async Task OpenRejectReasonModal()
        {
            var awaitingConform = await DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "AWAITING_CONFIRMATION_FROM_NAPOO");
            ResultContext<List<CandidateProviderVM>> currentContext = new ResultContext<List<CandidateProviderVM>>();
            currentContext.ResultContextObject.AddRange(sfGrid.SelectedRecords);
            List<string> errorMessages = new List<string>();
            if (sfGrid.SelectedRecords.Count > GlobalConstants.INVALID_ID_ZERO)
            {
                foreach (var item in currentContext.ResultContextObject)
                {
                    if (item.IdRegistrationApplicationStatus != awaitingConform.IdKeyValue)
                    {
                        var statusKeyValue = await DataSourceService.GetKeyValueByIdAsync(item.IdRegistrationApplicationStatus);
                        errorMessages.Add($"Можете да откажете електронна регистрация само на заявки на статус 'Очаква се одобрение на регистрацията от НАПОО'! Избраният ред с ЕИК {item.PoviderBulstat} е на статус '{statusKeyValue.Name}'!");
                    }
                }

                if (errorMessages.Count != 0)
                {
                    toast.sfErrorToast.Content = string.Join("<br />", errorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    return;
                }
                this.model.RejectionReason = "";
                this.editContext = new EditContext(this.model);
                this.isVisible = true;
            }
            else
            {
                toast.sfErrorToast.Content = "Моля първо изберете записи от таблицата!";
                await toast.sfErrorToast.ShowAsync();
            }
        }

        private async Task SendApproveMail()
        {
            bool hasPermission = await CheckUserActionPermission("ManageRegistrationFormData", false);
            if (!hasPermission) { return; }

            if (sfGrid.SelectedRecords.Count > GlobalConstants.INVALID_ID_ZERO)
            {
                ResultContext<CandidateProviderVM> currentContext = new ResultContext<CandidateProviderVM>();
                currentContext.ResultContextObject = new CandidateProviderVM();

                foreach (var item in sfGrid.SelectedRecords)
                {
                    var emailValidation = await DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "EMAIL_VALIDATION_EXPECTED");
                    currentContext.ResultContextObject = item;
                    if (currentContext.ResultContextObject.IdRegistrationApplicationStatus == emailValidation.IdKeyValue)
                    {
                        /*
                        ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                        tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("PoviderBulstat", item.PoviderBulstat) };

                        item.Token = this.CommonService.GetTokenWithParams(tokenContext, GlobalConstants.MINUTES_SIXTY);

                        currentContext.ResultContextObject = item;

                        var candidateProvider = currentContext.ResultContextObject.To<CandidateProvider>();
                       
                        await this.mailService.SendEmailNewRegistration(currentContext);
                        currentContext.ListMessages.Add("Успешно изпратихте e-mail за потвърждение!");
                    
                         */
                        ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                        tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("PoviderBulstat", item.PoviderBulstat) };

                        var minutesValidToken = await this.DataSourceService.GetSettingByIntCodeAsync("MinutesForEmailValidation");

                        currentContext.ResultContextObject.Token = this.CommonService.GetTokenWithParams(tokenContext, Int32.Parse(minutesValidToken.SettingValue));
                        await this.mailService.SendEmailNewRegistration(currentContext);
                        
                        await this.CandidateProviderService.UpdateCandidateProvider(currentContext.ResultContextObject.To<CandidateProvider>());
                        
                        currentContext.ListMessages.Add("Успешно изпратихте e-mail за потвърждение!");
                    }
                    else
                    {
                        var statusKeyValue = await DataSourceService.GetKeyValueByIdAsync(currentContext.ResultContextObject.IdRegistrationApplicationStatus);
                        currentContext.ListErrorMessages.Add($"Можете да изпратите e-mail за потвърждение само на заявки на статус 'Очаква се потвърждение за валиден e-mail адрес'! Избраният ред с ЕИК '{currentContext.ResultContextObject.PoviderBulstat}' е на статус '{statusKeyValue.Name}'!");
                    }
                }



                if (currentContext.HasMessages)
                {
                    toast.sfSuccessToast.Content = string.Join(Environment.NewLine, currentContext.ListMessages);
                    await toast.sfSuccessToast.ShowAsync();
                    currentContext.ListMessages.Clear();
                }
                else if (currentContext.HasErrorMessages)
                {
                    toast.sfErrorToast.Content = string.Join(Environment.NewLine, currentContext.ListErrorMessages);
                    await toast.sfErrorToast.ShowAsync();
                    currentContext.ListErrorMessages.Clear();
                }
                currentContext.ListMessages.Clear();
                var model = await SetLicenseTypeIdToModelForFilterAsync(this.LicenseType);
                this.candidateProviders = await this.CandidateProviderService.GetAllExpertsAsync(model);
                this.StateHasChanged();

            }
            else
            {
                toast.sfErrorToast.Content = "Моля първо изберете записи от таблицата!";
                await toast.sfErrorToast.ShowAsync();
            }
        }

        private async Task CellInfoHandler(QueryCellInfoEventArgs<CandidateProviderVM> args)
        {
            var form = this.candidateProviders.FirstOrDefault(x => x.IdCandidate_Provider == args.Data.IdCandidate_Provider);
            var keyValues = await this.DataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "AWAITING_CONFIRMATION_FROM_NAPOO");
            var isValid = keyValues.IdKeyValue == form.IdRegistrationApplicationStatus;
            if (isValid)
            {
                args.Cell.AddClass(new string[] { "row-red" });
            }
        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            if (args.Item.Id == "sfGrid_pdfexport")
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = candidateProviders.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PoviderBulstat", HeaderText = "ЕИК", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ManagerName", HeaderText = "Представлявано от", Width = "80", Format = "C2", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "AttorneyName", HeaderText = "Пълномощник", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderEmail", HeaderText = "E-mail", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "CreationDate", HeaderText = "Регистрирано на", Width = "80", Format = "d", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = fileName + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs(fileName + ".csv", result.ToArray());
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
                sheet.Range["B1"].Text = "ЕИК";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Представлявано от";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Пълномощник";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "E-mail";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Регистрирано на";
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
                foreach (var item in candidateProviders)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.ProviderOwner;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.PoviderBulstat;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.ManagerName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.AttorneyName;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.ProviderEmail;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.CreationDate.ToString(GlobalConstants.DATE_FORMAT);
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.ApplicationStatus;
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
        public void PdfQueryCellInfoHandler(PdfQueryCellInfoEventArgs<CandidateProviderVM> args)
        {
            if (args.Column.HeaderText == " ")
            {
                args.Cell.Value = GetRowNumber(sfGrid, args.Data.IdCandidate_Provider).Result.ToString();
            }
        }

        private async Task CheckInRegix(CandidateProviderVM candidateProviderVM)
        {
            var callContext = await this.GetCallContextAsync(this.BulstatCheckKV);
            var actualStateResponseType = this.RegiXService.GetActualState(candidateProviderVM.PoviderBulstat, callContext);
            await this.LogRegiXRequestAsync(callContext, actualStateResponseType != null);

            this.regixData.OpenModal(actualStateResponseType);
        }
    }

    public class RejectReason
    {
        [Required(ErrorMessage = "Полето 'Причина за отказ' е задължително!")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Причина за отказ' не може да съдържа повече от 512 символа.")]
        public string? RejectionReason { get; set; } = "";
    }
}
