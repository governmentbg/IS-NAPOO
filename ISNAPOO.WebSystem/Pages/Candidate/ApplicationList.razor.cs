using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Candidate.ChangeLicenzing;
using ISNAPOO.WebSystem.Pages.EGovPayment;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.ExcelExport;
using Syncfusion.PdfExport;
using Syncfusion.XlsIO;
using Border = Syncfusion.Blazor.Grids.Border;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ApplicationList : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderVM> proceduresGrid = new SfGrid<CandidateProviderVM>();
        private PaymentFeeListModal paymentFeeListModal = new PaymentFeeListModal();
        private ApplicationModal applicationModal = new ApplicationModal();
        private AdditionalDocumentModal additionalDocumentModal = new AdditionalDocumentModal();
        private ProcedureModal procedureModal = new ProcedureModal();
        private ApplicationListFilterModal applicationListFilterModal = new ApplicationListFilterModal();
        private ChangeProcedureModal changeProcedureModal = new ChangeProcedureModal();
        private ApplicationNumberChangeModal applicationNumberChangeModal = new ApplicationNumberChangeModal();

        private IEnumerable<CandidateProviderVM> candidateProviders = new List<CandidateProviderVM>();
        private IEnumerable<CandidateProviderVM> candidateProvidersOriginalDataSource = new List<CandidateProviderVM>();
        private LocationVM locationVM = new LocationVM();
        private IEnumerable<LocationVM> locationSource = new List<LocationVM>();
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationTypeSource = new List<KeyValueVM>();
        private string LicenseType = string.Empty;
        private bool isVisibleProcedureBtn = true;
        private bool isApplicationChange = false;
        private bool showFilterBtn = false;
        private bool disableChangeLicenseAndChangeNameBtn = false;
        private List<CandidateProviderVM> filteredResults = new List<CandidateProviderVM>();
        private List<CandidateProviderVM> selectedCandidateProviders = new List<CandidateProviderVM>();
        private List<CandidateProviderVM> sourceForAppNumb = new List<CandidateProviderVM>();
        private List<int> applicationStatusIds = new List<int>();
        private KeyValueVM kvProcedureCompleted;
        private KeyValueVM kvRejectedApplicationLicensingNewCenter;
        private KeyValueVM kvDocumentPreparation;
        private KeyValueVM kvApplicationFilingType;
        private string title = string.Empty;
        private bool showAllGridButtons = true;
        private bool isLicenceNumberVisible = true;
        private bool isBtnVisibleForPayments = true;
        private bool isCPO = true;

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.isBtnVisibleForPayments = false;

                this.kvApplicationStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
                this.applicationStatusIds = this.kvApplicationStatusSource.Where(x => x.KeyValueIntCode == "PreparationDocumentationLicensing"
                    || x.KeyValueIntCode == "RequestedByCPOOrCIPO"
                    || x.KeyValueIntCode == "ProcedureWasRegisteredInKeepingSystem"
                    || x.KeyValueIntCode == "AdministrativeCheck"
                    || x.KeyValueIntCode == "LeadingExpertGavePositiveAssessment"
                    || x.KeyValueIntCode == "LeadingExpertGaveNegativeAssessment"
                    || x.KeyValueIntCode == "CorrectionApplication"
                    || x.KeyValueIntCode == "LicensingExpertiseStarted"
                    || x.KeyValueIntCode == "ExpertCommissionAssessment")
                        .Select(x => x.IdKeyValue).ToList();
                this.locationSource = await this.LocationService.GetAllLocationsAsync();
                this.kvApplicationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
                this.kvProcedureCompleted = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "ProcedureCompleted");
                this.kvDocumentPreparation = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "PreparationDocumentationLicensing");
                this.kvRejectedApplicationLicensingNewCenter =
                    kvApplicationStatusSource.FirstOrDefault(k =>
                        k.KeyValueIntCode == "RejectedApplicationLicensingNewCenter");
                this.kvApplicationFilingType = await this.DataSourceService.GetKeyValueByIntCodeAsync("ApplicationFilingType", "ThroughESignature");
            
                await this.HandleTokenData();              
                this.SetCandidateProvidersApplicationStatusName();
                this.SetCandidateProviderLocationName();

                await this.proceduresGrid.Refresh();
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

        private async Task HandleTokenData(bool invokedAfterApplicatioModalSubmit = false)
        {
            if (!string.IsNullOrEmpty(this.Token))
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
                        this.title = "Процедури по лицензиране на ЦПО";
                        this.isCPO = true;
                        this.isLicenceNumberVisible = false;
                        this.isVisibleProcedureBtn = false;
                        this.candidateProviders = await this.CandidateProviderService.GetFirstLicencingCandidateProviderByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
                        this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();
                    }
                    else if (this.LicenseType == GlobalConstants.TOKEN_LICENSING_CIPO_APPLICATION_LICENSE_VALUE)
                    {
                        this.title = "Процедури по лицензиране на ЦИПО";
                        this.isCPO = false;
                        this.isLicenceNumberVisible = false;
                        this.isVisibleProcedureBtn = false;
                        this.candidateProviders = await this.CandidateProviderService.GetFirstLicencingCandidateProviderByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider, false);
                        this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();
                    }
                    else if (this.LicenseType == GlobalConstants.TOKEN_LICENSING_MODIFICATION_LICENSE_VALUE)
                    {
                        this.isCPO = true;
                        this.title = "Процедури за изменение на лицензията на ЦПО";
                        this.isApplicationChange = true;
                        this.isLicenceNumberVisible = true;
                        this.disableChangeLicenseAndChangeNameBtn = await this.CandidateProviderService.DoesApplicationChangeOnStatusDifferentFromProcedureCompletedExistAsync(this.UserProps.IdCandidateProvider);
                        this.isVisibleProcedureBtn = false;
                        this.candidateProviders = await this.CandidateProviderService.GetLicencingChangeCandidateProvidersByIdCandidateProviderAsync(this.UserProps.IdCandidateProvider);
                        this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();

                        if (!invokedAfterApplicatioModalSubmit)
                        {
                            // позволява редакция по заявлението, ако статусът е на Подготовка на документи за лицензиране
                            this.showAllGridButtons = true;
                            foreach (var cp in this.candidateProviders)
                            {
                                if (cp.IdApplicationStatus.HasValue)
                                {
                                    cp.ShowApplicationGridButtonsIfFirstLicensing = this.applicationStatusIds.Contains(cp.IdApplicationStatus.Value);
                                }
                            }
                        }
                    }
                    else if (this.LicenseType == GlobalConstants.TOKEN_LICENSING_CPO_APPLICATION_LICENSE_NAPOO_VALUE)
                    {
                        this.title = "Процедури по лицензиране на ЦПО";

                        this.isCPO = true;

                        this.candidateProviders = await this.CandidateProviderService.GetAllActiveProceduresAsync();

                        // сортира списъка с CandidateProviders от достъп НАПОО по най-нова промяна на статуса на заявлението в StartedProcedureProgress
                        this.FilterCandidateProvidersByLatestProcedureStatus();

                        this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();
                        this.showFilterBtn = true;

                        return;
                    }
                    else if (this.LicenseType == GlobalConstants.TOKEN_LICENSING_CIPO_APPLICATION_LICENSE_NAPOO_VALUE)
                    {
                        this.title = "Процедури по лицензиране на ЦИПО";

                        this.isCPO = false;

                        this.candidateProviders = await this.CandidateProviderService.GetAllActiveProceduresAsync(false);

                        // сортира списъка с CandidateProviders от достъп НАПОО по най-нова промяна на статуса на заявлението в StartedProcedureProgress
                        this.FilterCandidateProvidersByLatestProcedureStatus();

                        this.candidateProvidersOriginalDataSource = this.candidateProviders.ToList();
                        this.showFilterBtn = true;

                        return;
                    }

                    if (!invokedAfterApplicatioModalSubmit && this.candidateProviders.Count() == 1)
                    {
                        bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
                        if (!hasPermission) { return; }

                        var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(this.candidateProviders.FirstOrDefault()!.IdCandidate_Provider, "CandidateProvider");
                        if (concurrencyInfoValue == null)
                        {
                            await this.AddEntityIdAsCurrentlyOpened(this.candidateProviders.FirstOrDefault()!.IdCandidate_Provider, "CandidateProvider");
                        }

                        // позволява редакция по заявлението, ако статусът е на Подготовка на документи за лицензиране
                        var candidateProvider = this.candidateProviders.FirstOrDefault();
                        this.showAllGridButtons = candidateProvider!.IdApplicationStatus.HasValue && this.applicationStatusIds.Contains(candidateProvider!.IdApplicationStatus!.Value);
                        if (this.showAllGridButtons)
                        {
                            var kvFirstLicenzing = await DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicenzing");
                            bool isLicenceChange = candidateProvider!.IdTypeApplication != kvFirstLicenzing.IdKeyValue;
                            await this.applicationModal.OpenModal(candidateProvider!, false, isLicenceChange, concurrencyInfoValue);
                        }
                    }
                }
                else
                {
                    //TODO да те препраща някъде ако токена е невалиден
                    await this.ShowErrorAsync(string.Join(Environment.NewLine, currentContext.ListErrorMessages));
                }
            }
        }

        // сортира списъка с CandidateProviders по най-нова промяна на статуса на заявлението в StartedProcedureProgress
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

        private async Task OpenProcedureDocumentsModalBtnAsync(CandidateProviderVM candidateProviderVM)
        {
            bool hasPermission = await CheckUserActionPermission("ViewApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var kvFirstLicenzing = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "FirstLicenzing");
                bool isLicenceChange = this.isCPO
                    ? candidateProviderVM.IdTypeApplication != kvFirstLicenzing.IdKeyValue
                    : false;

                if (this.GetUserRoles().Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES" || x.StartsWith("NAPOO")))
                {
                    await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider }, false, isLicenceChange);
                }
                else
                {
                    var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(candidateProviderVM.IdCandidate_Provider, "CandidateProvider");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(candidateProviderVM.IdCandidate_Provider, "CandidateProvider");
                    }

                    // позволява редакция по заявлението, ако статусът е на Подготовка на документи за лицензиране
                    await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = candidateProviderVM.IdCandidate_Provider }, false, isLicenceChange, concurrencyInfoValue);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OpenAdditionalDocumentModalBtnAsync(CandidateProviderVM candidateProviderVM)
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

                await this.additionalDocumentModal.OpenModal(candidateProviderVM, this.isCPO);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OpenProcedureModalBtnAsync(CandidateProviderVM candidateProviderVM)
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

                var kvChangeLicenzing = await DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "ChangeLicenzing");

                await this.procedureModal.OpenModal(candidateProviderVM, candidateProviderVM.IdTypeApplication == kvChangeLicenzing.IdKeyValue);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OnApplicationSubmit()
        {
            // запазва резултатите от филтрацията и след запис на заявлението
            if (this.filteredResults.Any())
            {
                this.candidateProviders = this.filteredResults.Where(x => this.candidateProviders.Any(y => y.IdCandidate_Provider == x.IdCandidate_Provider)).ToList();
            }
            else
            {
                await this.HandleTokenData(true);
            }

            this.SetCandidateProviderLocationName();
            this.SetCandidateProvidersApplicationStatusName();
        }

        private async Task OpenModalForApplicationChangeBtnAsync()
        {
            string msg = "Сигурни ли сте, че искате да стартирате процедура за изменение на лицензията? Няма да можете да извършвате промени в профила на ЦПО до завършване на процедурата.";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var kvApplicationChange = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "ChangeLicenzing");
                    var idInactiveCandidateProvider = await this.CandidateProviderService.CreateApplicationChangeCandidateProviderAsync(this.UserProps.IdCandidateProvider, kvApplicationChange.IdKeyValue);

                    var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(idInactiveCandidateProvider, "CandidateProvider");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(idInactiveCandidateProvider, "CandidateProvider");
                    }

                    await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = idInactiveCandidateProvider }, false, true, concurrencyInfoValue);

                    await this.HandleTokenData(true);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
        }

        private async Task OpenModalForNameChangeBtn()
        {
            string msg = "Сигурни ли сте, че искате да стартирате процедура за изменение на името на центъра? Няма да можете да извършвате промени в профила на ЦПО до завършване на процедурата.";
            bool isConfirmed = await this.ShowConfirmDialogAsync(msg);
            if (isConfirmed)
            {
                this.SpinnerShow();

                if (this.loading)
                {
                    return;
                }
                try
                {
                    this.loading = true;

                    var kvApplicationChange = await this.DataSourceService.GetKeyValueByIntCodeAsync("TypeApplication", "ChangeLicenzing");
                    var idInactiveCandidateProvider = await this.CandidateProviderService.CreateApplicationChangeCandidateProviderAsync(this.UserProps.IdCandidateProvider, kvApplicationChange.IdKeyValue);

                    var concurrencyInfoValue = this.GetAllCurrentlyOpenedModalsConcurrencyInfoValue(idInactiveCandidateProvider, "CandidateProvider");
                    if (concurrencyInfoValue == null)
                    {
                        await this.AddEntityIdAsCurrentlyOpened(idInactiveCandidateProvider, "CandidateProvider");
                    }

                    await this.applicationModal.OpenModal(new CandidateProviderVM() { IdCandidate_Provider = idInactiveCandidateProvider }, false, true, concurrencyInfoValue);

                    await this.HandleTokenData(true);
                }
                finally
                {
                    this.loading = false;
                }

                this.SpinnerHide();
            }
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
                candidateProviderVM.fromPage = "ApplicationList";

                await this.paymentFeeListModal.openPaymentFeeList(candidateProviderVM);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task OpenFilterModalBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var title = this.isCPO 
                    ? "ЦПО"
                    : "ЦИПО";

                //зададено е да подава списъка на кандидат-провайдърите от първоначалното филтриране по най-нова промяна на статуса на заявлението в StartedProcedureProgress при зареждане на страницата Процедури
                await this.applicationListFilterModal.OpenModal(title, this.candidateProviders.ToList(), this.candidateProvidersOriginalDataSource.ToList(), this.kvApplicationTypeSource, this.kvApplicationStatusSource);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private void UpdateAppNumberDateBtn(CandidateProviderVM candidateProvider)
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

        private void OnFilterModalSubmit(List<CandidateProviderVM> candidateProviders)
        {
            this.candidateProviders = candidateProviders.ToList();
            this.filteredResults = candidateProviders.ToList();
            this.SetCandidateProvidersApplicationStatusName();
            this.SetCandidateProviderLocationName();
            this.proceduresGrid.DataSource = this.candidateProviders;

            this.StateHasChanged();
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
                sheet.Range["C1"].Text = "Населено място";
                sheet.Range["D1"].ColumnWidth = 120;
                sheet.Range["D1"].Text = "Адрес за кореспонденция";
                sheet.Range["E1"].ColumnWidth = 120;
                sheet.Range["E1"].Text = "Лице за контакт";
                sheet.Range["F1"].ColumnWidth = 120;
                sheet.Range["F1"].Text = "Телефон";
                sheet.Range["G1"].ColumnWidth = 120;
                sheet.Range["G1"].Text = "E-mail";
                sheet.Range["H1"].ColumnWidth = 120;
                sheet.Range["H1"].Text = "Процедура";
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
                    sheet.Range[$"A{rowCounter}"].Text = item.LicenceNumber;
                    sheet.Range[$"A{rowCounter}"].WrapText = true;
                    sheet.Range[$"A{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;

                    sheet.Range[$"B{rowCounter}"].Text = item.ProviderOwner;
                    sheet.Range[$"B{rowCounter}"].WrapText = true;
                    sheet.Range[$"B{rowCounter}"].CellStyle.VerticalAlignment = ExcelVAlign.VAlignTop;
                    sheet.Range[$"B{rowCounter}"].RowHeight = 10;

                    sheet.Range[$"C{rowCounter}"].Text = item.LocationCorrespondence != null ? item.LocationCorrespondence.LocationName : "";
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

                    sheet.Range[$"H{rowCounter}"].Text = item.TypeApplication;
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

        private async Task SendNotificationAsync()
        {
            var selectedRows = await this.proceduresGrid.GetSelectedRecordsAsync();
            if (!selectedRows.Any())
            {
                var msg = this.isCPO
                    ? "ЦПО"
                    : "ЦИПО";

                await this.ShowErrorAsync($"Моля, изберете {msg}, към което/които да изпратите известие!");
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

                var listIds = selectedRows.Select(x => x.IdCandidate_Provider).ToList();
                if (listIds.Any())
                {
                    await this.LoadDataForPersonsToSendNotificationToAsync(null, null, listIds);
                    await this.OpenSendNotificationModal(true, this.personIds);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        protected async Task ToolbarClick(ClickEventArgs args)
        {
            if (args.Item.Id.Contains("pdf"))
            {
                int temp = proceduresGrid.PageSettings.PageSize;
                proceduresGrid.PageSettings.PageSize = candidateProviders.Count();
                await proceduresGrid.Refresh();
                PdfExportProperties ExportProperties = new PdfExportProperties();
                List<GridColumn> ExportColumns = new List<GridColumn>();
#pragma warning disable BL0005
                ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", Width = "60", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", Width = "80", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "PoviderBulstat", HeaderText = "ЕИК", Width = "60", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", Width = "80", HeaderText = "Населено място", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "ExportDataAsStr", HeaderText = "Контакти", Width = "180", TextAlign = TextAlign.Left });
                ExportColumns.Add(new GridColumn() { Field = "TypeApplication", HeaderText = "Процедура", Width = "80", TextAlign = TextAlign.Left });               
                ExportColumns.Add(new GridColumn() { Field = "ApplicationDate", HeaderText = "Подадена на", Width = "80", Format = "d", TextAlign = TextAlign.Left });
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
                ExportProperties.FileName = $"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.pdf";

                await this.proceduresGrid.ExportToPdfAsync(ExportProperties);
                proceduresGrid.PageSettings.PageSize = temp;
                await proceduresGrid.Refresh();
            }
            else if (args.Item.Id.Contains("excel"))
            {
                ExcelExportProperties ExportProperties = new ExcelExportProperties();
                ExportProperties.FileName = $"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.xlsx";
                ExportProperties.IncludeTemplateColumn = true;
                ExportProperties.Columns = this.SetGridColumnsForExport();
                ExcelTheme Theme = new ExcelTheme();
                Border border = new Border();
                border.LineStyle = LineStyle.Thin;
                border.Color = "#000000";
                ExcelStyle HeaderThemeStyle = new ExcelStyle()
                {
                    BackColor = "#00BCD4",
                    FontSize = 12,
                    Bold = true,
                    FontColor = "#FFFFFF",
                    Borders = border,
                    WrapText = true,
                    VAlign = ExcelVerticalAlign.Top
                };

                ExcelStyle RecordThemeStyle = new ExcelStyle()
                {
                    Borders = border,
                    WrapText = true,
                    VAlign = ExcelVerticalAlign.Top
                };
                Theme.Header = HeaderThemeStyle;
                Theme.Record = RecordThemeStyle;

                ExportProperties.Theme = Theme;
                await this.proceduresGrid.ExportToExcelAsync(ExportProperties);
            }
            else
            {
                var result = CreateExcelCurriculumValidationErrors();
                await this.JsRuntime.SaveAs($"ApplicationList_{DateTime.Now.ToString(GlobalConstants.DATE_FORMAT_DELETED_FILE)}.csv", result.ToArray());
            }
        }

        private List<GridColumn> SetGridColumnsForExport()
        {
            List<GridColumn> ExportColumns = new List<GridColumn>();
            ExportColumns.Add(new GridColumn() { Field = "LicenceNumber", HeaderText = "Лицензия", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ProviderOwner", HeaderText = "Юридическо лице", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "PoviderBulstat", HeaderText = "ЕИК", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "LocationCorrespondence.LocationName", HeaderText = "Населено място", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ExportDataAsStr", HeaderText = "Контакти", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "TypeApplication", HeaderText = "Процедура", TextAlign = TextAlign.Left });           
            ExportColumns.Add(new GridColumn() { Field = "ApplicationDate", HeaderText = "Подадена на", Width = "80", Format = "d", TextAlign = TextAlign.Left });
            ExportColumns.Add(new GridColumn() { Field = "ApplicationStatus", HeaderText = "Статус", TextAlign = TextAlign.Left });

            return ExportColumns;
        }
    }
}
