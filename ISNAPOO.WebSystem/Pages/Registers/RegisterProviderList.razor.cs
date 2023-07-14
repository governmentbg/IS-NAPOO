using System.Net.Mail;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Candidate;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO;
using ISNAPOO.WebSystem.Pages.Candidate.NAPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Pages.Mail;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Registers
{
    partial class RegisterProviderList : BlazorBaseComponent
    {
        List<CandidateProviderVM> candidateProviderVMs;
        SfGrid<CandidateProviderVM> sfGrid = new SfGrid<CandidateProviderVM>();
        GridFilterSettings gridFilterSettings = new GridFilterSettings();
        ToastMsg toast = new ToastMsg();
        ApplicationModal applicationModal = new ApplicationModal();
        CIPOApplicationModal cipoApplicationModal = new CIPOApplicationModal();
        ProviderLicenceChangeModal providerLicenceChangeModal = new ProviderLicenceChangeModal();
        ProviderLicenceListModal providerLicenceListModal = new ProviderLicenceListModal();
        SendMailModal sendMailModal = new SendMailModal();
        NAPOOCandidateProviderSearchModal napooCandidateProviderSearchModal = new NAPOOCandidateProviderSearchModal();
        RegisterProviderFollowUpControlModal registerProviderFollowUpControlModal = new RegisterProviderFollowUpControlModal();
        RegisterProviderReportFilter reportFilter;

        private KeyValueVM kvProcedureCompleted = new KeyValueVM();
        public string Header { get; set; }
        private bool isVisibleLicenceChangeBtn = true;
        private string fileName = "";
        private string LicensingType = string.Empty;
        private string type = string.Empty;
        private string cpoOrCipo = string.Empty;
        private bool showInactiveProviders = false;
        private bool showFollowUpControlBtn = false;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IMailService mailService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }
        [Inject]
        public ITrainingService trainingService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                tokenContext.ResultContextObject.Token = Token;

                this.kvProcedureCompleted = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");
                try
                {
                    tokenContext = BaseHelper.GetDecodeToken(tokenContext);
                }
                catch (Exception)
                {
                    await this.ShowErrorAsync("Грешен линк");
                    return;
                }

                this.showInactiveProviders = false;
                var tokenType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(x => x.Key == "EntryFrom");
                if (tokenType.Value != null)
                {
                    if (tokenType.Value.ToString() == "LicensedCPO")
                    {
                        this.Header = "Лицензирани ЦПО";
                        this.LicensingType = "LicensingCPO";
                        this.fileName = $"Register_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                        this.cpoOrCipo = "ЦПО";
                    }
                    else if (tokenType.Value.ToString() == "LicensedCIPO")
                    {
                        this.Header = "Лицензирани ЦИПО";
                        this.LicensingType = "LicensingCIPO";
                        this.fileName = $"Register_CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                        this.cpoOrCipo = "ЦИПО";
                    }
                    else if (tokenType.Value.ToString() == "DeactivatedLicenseCPO")
                    {
                        this.showInactiveProviders = true;
                        this.Header = "ЦПО с отнета лицензия";
                        this.LicensingType = "LicensingCPO";
                        this.fileName = $"Register_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                        this.cpoOrCipo = "ЦПО";
                    }
                    else if (tokenType.Value.ToString() == "DeactivatedLicenseCIPO")
                    {
                        this.showInactiveProviders = true;
                        this.Header = "ЦИПО с отнета лицензия";
                        this.LicensingType = "LicensingCIPO";
                        this.fileName = $"Register_CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                        this.cpoOrCipo = "ЦИПО";
                    }
                }
                else
                {
                    tokenType = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(x => x.Key == "LicensingType");
                    if (tokenType.Value.ToString() == "LicensingCPO")
                    {
                        this.Header = "Регистър на ЦПО";
                        this.LicensingType = "LicensingCPO";
                        this.fileName = $"Register_CPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                        this.cpoOrCipo = "ЦПО";
                    }
                    else
                    {
                        this.Header = "Регистър на ЦИПО";
                        this.LicensingType = "LicensingCIPO";
                        this.fileName = $"Register_CIPO_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}";
                        this.cpoOrCipo = "ЦИПО";
                    }
                }

                this.type = this.Header.Contains("ЦПО") ? "ЦПО" : "ЦИПО";

                await LoadCandidateProvidersLicenseList();

                //if (!showInactiveProviders)
                //{
                //    this.candidateProviderVMs = (await candidateProviderService.GetAllActiveCandidateProvidersWithoutIncludesAsync(LicensingType)).ToList();
                //}
                //else
                //{
                //    this.candidateProviderVMs = (await candidateProviderService.GetAllCandidateProvidersWithLicenseDeactivatedAsync(LicensingType)).ToList();
                //}

                //this.StateHasChanged();

                //this.SpinnerHide();
            }
        }

        private async Task LoadCandidateProvidersLicenseList()
        {
            if (!showInactiveProviders)
            {
                this.candidateProviderVMs = (await candidateProviderService.GetAllActiveCandidateProvidersWithoutIncludesAsync(LicensingType)).OrderBy(x => x.LicenceNumber).ToList();
            }
            else
            {
                this.candidateProviderVMs = (await candidateProviderService.GetAllCandidateProvidersWithLicenseDeactivatedAsync(LicensingType)).OrderBy(x => x.LicenceNumber).ToList();
            }        

            this.StateHasChanged();

            this.SpinnerHide();

        }

        protected async Task ToolbarClick(Syncfusion.Blazor.Navigations.ClickEventArgs args)
        {
            this.SpinnerShow();

            if (args.Item.Id.Contains("pdf"))
            {
                int temp = sfGrid.PageSettings.PageSize;
                sfGrid.PageSettings.PageSize = candidateProviderVMs.Count();
                await sfGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                PdfTheme Theme = new PdfTheme();
                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { HeaderText = " ", Width = "30" });
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "80", TextAlign = TextAlign.Left });
                if (LicensingType == "LicensingCIPO")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "150", TextAlign = TextAlign.Left });
                }
                else
                {
                    ExportColumns.Add(new GridColumn() { Field = "CPONameOwnerGrid", HeaderText = "ЦПО", Width = "150", TextAlign = TextAlign.Left });
                }
                ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", HeaderText = "Населено място", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderAddressCorrespondence", HeaderText = "Адрес за кореспонденция", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonNameCorrespondence", HeaderText = "Лице за контакт", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderPhoneCorrespondence", HeaderText = "Телефон", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderEmailCorrespondence", HeaderText = "E-mail", Width = "80", TextAlign = TextAlign.Left });

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

                await this.sfGrid.ExportToPdfAsync(ExportProperties);
                sfGrid.PageSettings.PageSize = temp;
                await sfGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "90", TextAlign = TextAlign.Left });
                if (LicensingType == "LicensingCIPO")
                {
                    ExportColumns.Add(new GridColumn() { Field = "CIPONameOwnerGrid", HeaderText = "ЦИПО", Width = "150", TextAlign = TextAlign.Left });
                }
                else
                {
                    ExportColumns.Add(new GridColumn() { Field = "CPONameOwnerGrid", HeaderText = "ЦПО", Width = "150", TextAlign = TextAlign.Left });
                }
                ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", HeaderText = "Населено място", Width = "120", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderAddressCorrespondence", HeaderText = "Адрес за кореспонденция", Width = "150", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PersonNameCorrespondence", HeaderText = "Лице за контакт", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderPhoneCorrespondence", HeaderText = "Телефон", Width = "200", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderEmailCorrespondence", HeaderText = "E-mail", Width = "300", TextAlign = TextAlign.Left });

                ExportProperties.Columns = ExportColumns;
                ExportProperties.FileName = fileName + ".xlsx";
                await this.sfGrid.ExportToExcelAsync(ExportProperties);
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
                sheet.Range["A1"].Text = "Лицензия";
                sheet.Range["B1"].ColumnWidth = 120;
                sheet.Range["B1"].Text = $"{cpoOrCipo}";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Населено място";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Адрес за кореспонденция";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Лице за контакт";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Телефон";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "E-mail";

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
                foreach (var item in candidateProviderVMs)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.LicenceNumber;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    if (LicensingType == "LicensingCIPO")
                    {
                        sheet.Range[$"B{rowCounter}"].Text = item.CPONameOwnerGrid.Replace("ЦПО", "ЦИПО");
                    }
                    else
                    {
                        sheet.Range[$"B{rowCounter}"].Text = item.CPONameOwnerGrid;
                    }
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.LocationCorrespondence?.LocationName;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.ProviderAddressCorrespondence;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.PersonNameCorrespondence;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.ProviderPhoneCorrespondence;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.ProviderEmailCorrespondence;
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

        private async Task OpenProfileModalBtn(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider }, false, false);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenProviderLicenceListModal(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;
                //
                await this.providerLicenceListModal.OpenModal(candidateProviderVM, cpoOrCipo);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task sendEmail()
        {
            await sendMailModal.openModal();
        }

        private async Task sendEmails(MailMessage mail)
        {
            var rows = await sfGrid.GetSelectedRecordsAsync();

            foreach (var provider in rows)
            {
                if (provider.ProviderEmailCorrespondence != null)
                {
                    mail.To.Add(provider.ProviderEmailCorrespondence);
                }
            }
            await mailService.SendCustomEmail(mail);

            await sfGrid.Refresh();

            this.SpinnerHide();

            await this.ShowSuccessAsync("E-mail-ите са изпратени успешно!");
        }

        private async Task FilterBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var searchType = this.Header.Contains("ЦПО") ? "ЦПО" : "ЦИПО";
                await this.napooCandidateProviderSearchModal.OpenModal(searchType, this.showInactiveProviders);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async void UpdateAfterFilterBtn(List<CandidateProviderVM> candidateProviders)
        {
            this.SpinnerShow();

            var licenceStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus");
            var locations = await this.LocationService.GetAllLocationsAsync();
            this.candidateProviderVMs = candidateProviders.ToList();
            foreach (var entry in this.candidateProviderVMs)
            {
                if (entry.IdLicenceStatus.HasValue)
                {
                    var licenceStatusValue = licenceStatusSource.FirstOrDefault(x => x.IdKeyValue == entry.IdLicenceStatus.Value);
                    if (licenceStatusValue is not null)
                    {
                        entry.LicenceStatusName = licenceStatusValue.Name;
                    }
                }

                if (entry.IdLocationCorrespondence.HasValue)
                {
                    entry.LocationCorrespondence = locations.FirstOrDefault(x => x.idLocation == entry.IdLocationCorrespondence.Value);
                }
            }
            
            await this.sfGrid.Refresh();
            this.StateHasChanged();
            this.SpinnerHide();
        }
        public async Task Filter(CoursesFilterVM filter)
        {
            MemoryStream stream = await trainingService.getCoursesReportStream(filter);

            await JsRuntime.SaveAs($"Spravka_obucheniq_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx", stream.ToArray());
        }
        private async Task SendNotificationAsync()
        {
            var selectedProviders = await this.sfGrid.GetSelectedRecordsAsync();
            var listIds = selectedProviders.Select(x => x.IdCandidate_Provider).ToList();
            if (listIds.Any())
            {
                await this.LoadDataForPersonsToSendNotificationToAsync(null, null, listIds);
                await this.OpenSendNotificationModal(true, this.personIds);
            }
            else
            {
                await this.ShowErrorAsync($"Моля, изберете {this.type}, към което/които да изпратите известие!");
            }
        }

        private async void FilterReport()
        {
            await reportFilter.openModal();
        }

        private async void OpenChecking(CandidateProviderVM candidateProviderVM)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                await this.registerProviderFollowUpControlModal.OpenModal(candidateProviderVM);
            }
            finally
            {
                this.loading = false;
            }
        }
    }
}
