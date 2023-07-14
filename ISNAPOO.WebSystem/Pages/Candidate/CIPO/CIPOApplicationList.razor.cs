using System.Globalization;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NuGet.Packaging;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOApplicationList : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderVM> applicationsGrid = new SfGrid<CandidateProviderVM>();

        private IEnumerable<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersOriginalDataSource = new List<CandidateProviderVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersAfterFilterSource = new List<CandidateProviderVM>();
        private CIPOApplicationModal applicationModal = new CIPOApplicationModal();
        private CIPOProcedureModal procedureModal = new CIPOProcedureModal();
        private PaymentFeeListModal paymentFeeListModal = new PaymentFeeListModal();
        private ApplicationNumberChangeModal applicationNumberChangeModal = new ApplicationNumberChangeModal();
        private ApplicationListFilterModal applicationListFilterModal = new ApplicationListFilterModal();
        private LocationVM locationVM = new LocationVM();
        private IEnumerable<LocationVM> locationSource = new List<LocationVM>();
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationTypeSource = new List<KeyValueVM>();
        private string LicenseType = string.Empty;
        private bool isVisibleProcedureBtn = true;
        private bool isApplicationChange = false;
        private bool showFilterBtn = false;
        private List<CandidateProviderVM> filteredResults = new List<CandidateProviderVM>();
        private List<CandidateProviderVM> selectedCandidateProviders = new List<CandidateProviderVM>();
        private AdditionalDocumentModal additionalDocumentModal = new AdditionalDocumentModal();
        private KeyValueVM kvDocumentPreparation;
        private KeyValueVM kvRejectedApplicationLicensingNewCenter;
        private KeyValueVM kvProcedureCompleted;
        private KeyValueVM kvApplicationFilingType;
        private bool showAllGridButtons = true;

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.locationSource = await this.LocationService.GetAllLocationsAsync();
                this.kvApplicationStatusSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
                this.kvApplicationTypeSource = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
                this.kvDocumentPreparation = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "PreparationDocumentationLicensing");
                this.kvProcedureCompleted = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "ProcedureCompleted");
                this.kvRejectedApplicationLicensingNewCenter =
                    kvApplicationStatusSource.FirstOrDefault(k =>
                        k.KeyValueIntCode == "RejectedApplicationLicensingNewCenter");
                this.kvApplicationFilingType = await this.dataSourceService.GetKeyValueByIntCodeAsync("ApplicationFilingType", "ThroughESignature");
                
                await this.HandleTokenData();
                
                this.SetCandidateProvidersApplicationStatusName();
                this.SetCandidateProviderLocationName();

                await this.applicationsGrid.Refresh();
                this.StateHasChanged();
            }
        }

        private void SetCandidateProvidersApplicationStatusName()
        {
            foreach (var provider in this.candidateProviders)
            {
                if (provider.IdApplicationStatus != null)
                {
                    provider.ApplicationStatus = this.kvApplicationStatusSource.FirstOrDefault(x => x.IdKeyValue == provider.IdApplicationStatus).Name;
                }
            }
        }

        private async Task HandleTokenData(bool invokedAfterApplicationModalSubmit = false)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

                currentContext.ResultContextObject = new TokenVM();
                currentContext.ResultContextObject.Token = Token;
                currentContext = this.CommonService.GetDecodeToken(currentContext);

                if (currentContext.ResultContextObject.IsValid)
                {
                    this.LicenseType = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(t => t.Key == GlobalConstants.TOKEN_LICENSING_TYPE_KEY).Value.ToString();

                    if (this.LicenseType == GlobalConstants.TOKEN_LICENSING_CPO_APPLICATION_LICENSE_VALUE)
                    {
                        var kvFirstLicenzing = await dataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicensingCIPO");

                        this.isVisibleProcedureBtn = false;
                        this.candidateProviders = await this.CandidateProviderService.GetFirstLicencingCandidateProviderByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, false);
                        this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();
                    }

                    if (!invokedAfterApplicationModalSubmit && this.candidateProviders.Count() == 1)
                    {
                        var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(this.candidateProviders.FirstOrDefault()!.IdCandidate_Provider, "CandidateProvider");
                        if (concurrencyInfoValue != null && concurrencyInfoValue.IdPerson != this.UserProps.IdPerson)
                        {
                            this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfoValue);
                            return;
                        }

                        if (concurrencyInfoValue == null)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.candidateProviders.FirstOrDefault()!.IdCandidate_Provider, "CandidateProvider");
                        }

                        // позволява редакция по заявлението, ако статусът е на Подготовка на документи за лицензиране
                        this.showAllGridButtons = this.candidateProviders.FirstOrDefault()!.IdApplicationStatus != this.kvProcedureCompleted.IdKeyValue && this.candidateProviders.FirstOrDefault()!.IdApplicationStatus != this.kvRejectedApplicationLicensingNewCenter.IdKeyValue && this.candidateProviders.FirstOrDefault()!.IdApplicationStatus.HasValue;
                        if (this.showAllGridButtons)
                        {
                            bool isEditDisabled = this.candidateProviders.FirstOrDefault()!.IdApplicationStatus != this.kvDocumentPreparation.IdKeyValue;
                            var candidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(this.candidateProviders.FirstOrDefault()!);
                            await this.applicationModal.OpenModal(candidateProviderVM, false, false, isEditDisabled, false, concurrencyInfoValue);
                        }
                    }
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
            else
            {
                this.candidateProviders = await this.CandidateProviderService.GetAllActiveProceduresAsync(false);
                // сортира списъка с CandidateProviders от достъп НАПОО по най-нова промяна на статуса на заявлението в StartedProcedureProgress
                this.FilterCandidateProvidersByLatestProcedureStatus();

                this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();

                this.showFilterBtn = true;
            }
        }

        // сортира списъка с CandidateProviders от достъп НАПОО по най-нова промяна на статуса на заявлението в StartedProcedureProgress
        private void FilterCandidateProvidersByLatestProcedureStatus()
        {
            var progressesList = new List<StartedProcedureProgressVM>();
            var sortedCandidateProviders = new List<CandidateProviderVM>();
            foreach (var candidateProvider in this.candidateProviders)
            {
                if (candidateProvider.StartedProcedure is not null)
                {
                    progressesList.AddRange(candidateProvider.StartedProcedure.StartedProcedureProgresses);
                }
            }

            progressesList = progressesList.OrderByDescending(x => x.IdStartedProcedureProgress).ToList();
            foreach (var progress in progressesList)
            {
                var candidateProvider = this.candidateProviders.FirstOrDefault(x => x.IdStartedProcedure == progress.IdStartedProcedure);
                if (candidateProvider is not null)
                {
                    if (!sortedCandidateProviders.Any(x => x.IdCandidate_Provider == candidateProvider.IdCandidate_Provider))
                    {
                        sortedCandidateProviders.Add(candidateProvider);
                    }
                }
            }

            this.candidateProviders = sortedCandidateProviders.ToList();
            this.candidateProvidersAfterFilterSource = this.candidateProviders.ToList();
        }

        // зарежда имената на населените места на CandidateProvider
        private void SetCandidateProviderLocationName()
        {
            foreach (var candidateProvider in this.candidateProviders)
            {
                if (candidateProvider.IdLocation != null)
                {
                    this.locationVM = this.locationSource.FirstOrDefault(x => x.idLocation == candidateProvider.IdLocationCorrespondence);

                    if (this.locationVM != null)
                    {
                        candidateProvider.LocationCorrespondence = this.locationVM;
                    }
                }
            }
        }
        private async Task UpdateAppNumberDate(CandidateProviderVM candidateProvider)
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.applicationNumberChangeModal.OpenModal(candidateProvider);

            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SelectedRow(CandidateProviderVM candidateProviderVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(candidateProviderVM.IdCandidate_Provider, "CandidateProvider");
            if (concurrencyInfoValue != null && concurrencyInfoValue.IdPerson != this.UserProps.IdPerson)
            {
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfoValue);
                return;
            }

            if (concurrencyInfoValue == null) 
            {
                await this.AddEntityIdAsCurrentlyOpened(candidateProviderVM.IdCandidate_Provider, "CandidateProvider");
            }


            // позволява редакция по заявлението, ако статусът е на Подготовка на документи за лицензиране
            bool isEditDisabled = candidateProviderVM.IdApplicationStatus != this.kvDocumentPreparation.IdKeyValue;
            candidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(candidateProviderVM);
            await this.applicationModal.OpenModal(candidateProviderVM, false, false, isEditDisabled, false, concurrencyInfoValue);
        }

        private async Task SelectedRowProcedure(CandidateProviderVM candidateProviderVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            await this.procedureModal.OpenModal(candidateProviderVM);
        }

        private async Task OnApplicationSubmit(ResultContext<CandidateProviderVM> resultContext)
        {
            if (!resultContext.HasErrorMessages)
            {
                await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                await this.HandleTokenData(true);

                // запазва резултатите от филтрацията и след запис на заявлението
                if (this.filteredResults.Any())
                {
                    this.candidateProviders = this.filteredResults.Where(x => this.candidateProviders.Any(y => y.IdCandidate_Provider == x.IdCandidate_Provider)).ToList();
                }

                this.SetCandidateProvidersApplicationStatusName();
                this.SetCandidateProviderLocationName();
            }
            else
            {
                await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
            }
        }
        private async Task OpenModalForApplicationChangeBtn()
        {
            var applicationData = await this.CandidateProviderService.GetCandidateProviderWithoutAnythingIncludedByIdAsync(this.UserProps.IdCandidateProvider);
            await this.applicationModal.OpenModal(applicationData, false, true);
        }

        private async Task OpenFilterBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                //this.candidateProviders = await this.CandidateProviderService.GetAllCIPOApplicationsByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
                //this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();
                var title = "ЦИПО";
                await this.applicationListFilterModal.OpenModal(title, this.candidateProviders.ToList(), this.candidateProvidersAfterFilterSource.ToList(), this.kvApplicationTypeSource, this.kvApplicationStatusSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterModalSubmit(List<CandidateProviderVM> candidateProviders)
        {
            this.candidateProviders = candidateProviders.ToList();
            this.filteredResults = candidateProviders.ToList();

            this.SetCandidateProviderLocationName();
            this.SetCandidateProvidersApplicationStatusName();

            this.applicationsGrid.DataSource = this.candidateProviders;
        }
        private async Task ShowPaymentRequests(CandidateProviderVM candidateProviderVM)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                if (!hasPermission) { return; }
                candidateProviderVM.fromPage = "CIPOApplicationList";
                await this.paymentFeeListModal.openPaymentFeeList(candidateProviderVM);
            }
            finally
            {
                this.loading = false;
            }
        }
        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Text == "Експорт PDF")
            {
                PdfExportProperties ExportProperties = new PdfExportProperties();
                ExportProperties.PageOrientation = PageOrientation.Landscape;
                ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.Columns = this.SetGridColumnsForExport();

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
                ExportProperties.FileName = $"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.applicationsGrid.ExportToPdfAsync(ExportProperties);
            }
            else if (args.Item.Text == "Експорт Excel")
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.Columns = this.SetGridColumnsForExport();
                await this.applicationsGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
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
                sheet.Range["B1"].Text = "Населено място";
                sheet.Range["C1"].ColumnWidth = 120;
                sheet.Range["C1"].Text = "Адрес за кореспонденция";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Лице за контакт";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Телефон";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "E-mail";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "Процедура";
                sheet.Range["H1"].ColumnWidth = 120;
                sheet.Range["H1"].Text = "№ на лицензия";
                sheet.Range["I1"].ColumnWidth = 120;
                sheet.Range["I1"].Text = "Подадена на";
                sheet.Range["J1"].ColumnWidth = 120;
                sheet.Range["J1"].Text = "Статус";

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
                IRichTextString boldTextJ = rangeJ.RichText;
                IFont boldFontJ = workbook.CreateFont();

                boldFontJ.Bold = true;
                boldTextJ.SetFont(0, sheet.Range["J1"].Text.Length, boldFontJ);

                var rowCounter = 2;
                foreach (var item in candidateProviders)
                {
                    sheet.Range[$"A{rowCounter}"].Text = item.ProviderOwner;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"A{rowCounter}"].RowHeight = 10;
                    sheet.Range[$"B{rowCounter}"].Text = item.Location != null ? item.Location.LocationName != null ? item.Location.LocationName : "" : "" ; 
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"C{rowCounter}"].Text = item.ProviderAddressCorrespondence;
                    sheet.Range[$"C{rowCounter}"].WrapText = true;
                    sheet.Range[$"C{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"D{rowCounter}"].Text = item.PersonNameCorrespondence;
                    sheet.Range[$"D{rowCounter}"].WrapText = true;
                    sheet.Range[$"D{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"E{rowCounter}"].Text = item.ProviderPhoneCorrespondence;
                    sheet.Range[$"E{rowCounter}"].WrapText = true;
                    sheet.Range[$"E{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"F{rowCounter}"].Text = item.ProviderEmailCorrespondence;
                    sheet.Range[$"F{rowCounter}"].WrapText = true;
                    sheet.Range[$"F{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"G{rowCounter}"].Text = item.TypeApplication;
                    sheet.Range[$"G{rowCounter}"].WrapText = true;
                    sheet.Range[$"G{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"H{rowCounter}"].Text = item.LicenceNumber;
                    sheet.Range[$"H{rowCounter}"].WrapText = true;
                    sheet.Range[$"H{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"I{rowCounter}"].Text = item.ApplicationDate.ToString();
                    sheet.Range[$"I{rowCounter}"].WrapText = true;
                    sheet.Range[$"I{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"J{rowCounter}"].Text = item.ApplicationStatus;
                    sheet.Range[$"J{rowCounter}"].WrapText = true;
                    sheet.Range[$"J{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    rowCounter++;
                }



                using (MemoryStream stream = new MemoryStream())
                {
                    workbook.SaveAs(stream, "      ", System.Text.Encoding.UTF8);
                    return stream;
                }
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();

            ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "PoviderBulstat", HeaderText = "ЕИК", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", HeaderText = "Населено място", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ExportDataAsStr", HeaderText = "Контакти", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TypeApplication", HeaderText = "Процедура", TextAlign = TextAlign.Left });           
            ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ApplicationDate", HeaderText = "Подадена на", Width = "80", Format = "d", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ApplicationStatus", HeaderText = "Статус", TextAlign = TextAlign.Left });
            return ExportColumns;
        }

        private async Task RowSelectedHandler(RowSelectEventArgs<CandidateProviderVM> args)
        {
            this.selectedCandidateProviders.Clear();
            this.selectedCandidateProviders = await this.applicationsGrid.GetSelectedRecordsAsync();
        }

        private async Task RowDeselectedHandler(RowDeselectEventArgs<CandidateProviderVM> args)
        {
            this.selectedCandidateProviders.Clear();
            this.selectedCandidateProviders = await this.applicationsGrid.GetSelectedRecordsAsync();
        }

        private async Task SendNotificationAsync()
        {
            var listIds = this.selectedCandidateProviders.Select(x => x.IdCandidate_Provider).ToList();
            if (listIds.Any())
            {
                await this.LoadDataForPersonsToSendNotificationToAsync(null, null, listIds);
                await this.OpenSendNotificationModal(true, this.personIds);
            }
            else
            {
                await this.ShowErrorAsync("Моля, изберете ЦИПО, към което/които да изпратите известие!");
            }
        }
        private async Task SelectedRowCIPOAdditionalDocument(CandidateProviderVM candidateProviderVM)
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                if (!hasPermission) { return; }

                await this.additionalDocumentModal.OpenModal(candidateProviderVM, false);
            }
            finally
            {
                this.loading = false;
            }
        }
    }
}
