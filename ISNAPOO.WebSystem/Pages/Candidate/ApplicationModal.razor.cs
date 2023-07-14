using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Candidate.CIPO;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ApplicationModal : BlazorBaseComponent, IConcurrencyCheck<CandidateProviderVM>
    {
        private SfTab applicationTab = new SfTab();
        private TrainingInstitution trainingInstitution = new TrainingInstitution();
        private Specialities specialities = new Specialities();
        private Consultings consultings = new Consultings();
        private StructureAndActivities structureAndActivities = new StructureAndActivities();
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        private Trainers trainers = new Trainers();
        private MaterialTechnicalBase materialTechnicalBase = new MaterialTechnicalBase();
        private CandidateProviderDocuments candidateProviderDocuments = new CandidateProviderDocuments();
        private FormApplicationModal formApplicationModal = new FormApplicationModal();
        private ApplicationStatus applicationStatus = new ApplicationStatus();
        private LicensingProcedure licensingProcedure = new LicensingProcedure();
        private LicenceChange licenceChange = new LicenceChange();
        private string kvApplicationStatus = string.Empty;
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvApplicationChange = new KeyValueVM();
        private KeyValueVM kvDocumentPreparation = new KeyValueVM();
        private bool isLicenceChange = false;
        private int percentage = 0;
        private string title = string.Empty;
        private bool FormApplicationStatus = false;
        private bool hideBtnsConcurrentModal = false;
        private bool isUserExternalExpertOrExpertCommittee = false;
        private bool disableFieldsWhenUserIsNAPOO = false;
        private bool isUserProfileAdministrator = false;
        private bool disableFieldsWhenApplicationStatusIsNotDocPreparation = false;
        private bool disableFieldsWhenOpenFromProfile = false;
        private bool disableFieldsWhenActiveLicenceChange = false;
        private bool isCPO = true;
        private List<string> validationMessages = new List<string>();
        private int selectedTab = 0;

        public override bool IsContextModified
        {
            get
            {
                if (this.disableFieldsWhenUserIsNAPOO || this.disableFieldsWhenActiveLicenceChange || this.isUserExternalExpertOrExpertCommittee)
                {
                    return false;
                }
                else
                {
                    if (this.isCPO)
                    {
                        return this.trainingInstitution.IsEditContextModified()
                    || (this.trainers.editContextGeneralData is not null && this.trainers.editContextGeneralData.IsModified())
                    || (this.trainers.editContextTrainerProfile is not null && this.trainers.editContextTrainerProfile.IsModified())
                    || this.materialTechnicalBase.IsEditContextModified()
                    || this.structureAndActivities.IsEditContextModified();
                    }

                    return this.trainingInstitution.IsEditContextModified()
                    || (this.trainers.editContextGeneralData is not null && this.trainers.editContextGeneralData.IsModified())
                    || this.materialTechnicalBase.IsEditContextModified()
                    || this.structureAndActivities.IsEditContextModified();
                }
            }
        }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        [Parameter]
        public EventCallback CallbackAfterProfileSubmit { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public ISettingService SettingService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        public async Task OpenModal(CandidateProviderVM candidateProviderVM, bool isOpenFromProfile, bool isLicenceChange, ConcurrencyInfo concurrencyInfo = null)
        {
            this.selectedTab = 0;

            this.percentage = 0;

            this.validationMessages.Clear();

            this.disableFieldsWhenOpenFromProfile = isOpenFromProfile;
            this.isLicenceChange = isLicenceChange;

            this.CheckForRolesExternalExpertOrExpertCommittees();
            this.CheckForRoleNapoo();
            await this.CheckForUserProfileAdministratorAsync();
            this.CheckForApplicationStatusDocumentPreparation();

            await this.LoadDataAsync(candidateProviderVM);

            await this.SetIsCPOOrCIPOAsync();

            await this.SetTitleAsync();

            // проверява дали candidate provider е ЦПО и дали модалът е отворен от Профил ЦПО и прави проверка за активен запис за промяна на издадена лицензия на центъра
            if (this.isCPO && this.disableFieldsWhenOpenFromProfile)
            {
                this.disableFieldsWhenActiveLicenceChange = await this.CandidateProviderService.DoesApplicationChangeOnStatusDifferentFromProcedureCompletedExistAsync(this.IdCandidateProvider);
            }

            // проверява дали candidate provider е ЦПО и дали за промени в ДОС-а и неактулизарани учебни планове, ако модалът е отворен от Профил ЦПО и няма активна процедура по промяна на издадена лицензия
            if (this.isCPO && this.disableFieldsWhenOpenFromProfile && !this.disableFieldsWhenActiveLicenceChange)
            {
                await this.HandleDOSNotSubmittedChangesAsync();
            }

            // пресмята прогрес на заявлението, ако статусът му е Подготовка на документи и модалът не е отворен от Профил ЦПО/ЦИПО
            if (this.candidateProviderVM.IdApplicationStatus == this.kvDocumentPreparation.IdKeyValue && !this.disableFieldsWhenOpenFromProfile)
            {
                await this.CalculcateApplicationProgressAsync();
            }

            // проверява за вече отворен модал за редакция от друг потребител на центъра
            if (concurrencyInfo is not null && concurrencyInfo.IdPerson != this.UserProps.IdPerson)
            {
                this.hideBtnsConcurrentModal = true;
                this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfo);
            }
            else
            {
                this.hideBtnsConcurrentModal = false;
            }

            this.isVisible = true;
            this.StateHasChanged();

            if (this.disableFieldsWhenActiveLicenceChange)
            {
                this.ShowAlertDialog("Има стартирана процедура по изменение на лицензията на центъра. Профилът е заключен за редакция!");
            }
        }

        private async Task LoadCandidateProviderDataAsync(CandidateProviderVM model)
        {
            // зарежда candidate provider от БД, ако е подаден празен VM
            this.candidateProviderVM = model.CreationDate == default(DateTime)
                ? await this.CandidateProviderService.GetActiveCandidateProviderWithLocationIncludedByIdAsync(model.IdCandidate_Provider)
                : model;

            if (this.candidateProviderVM.IdApplicationStatus.HasValue)
            {
                var status = this.kvApplicationStatusSource.FirstOrDefault(x => x.IdKeyValue == this.candidateProviderVM.IdApplicationStatus!.Value);
                if (status is not null)
                {
                    this.candidateProviderVM.ApplicationStatus = status.Name;
                }
            }
        }

        private async Task LoadDataAsync(CandidateProviderVM model)
        {
            this.kvApplicationStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            this.kvApplicationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
            this.kvApplicationChange = this.kvApplicationTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ChangeLicenzing")!;
            this.kvDocumentPreparation = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "PreparationDocumentationLicensing")!;

            await this.LoadCandidateProviderDataAsync(model);

            this.IdCandidateProvider = this.candidateProviderVM.IdCandidate_Provider;
        }

        private async Task SetTitleAsync()
        {
            if (this.candidateProviderVM.IdLicenceStatus.HasValue)
            {
                this.candidateProviderVM.LicenceStatusName = (await this.DataSourceService.GetKeyValueByIdAsync(this.candidateProviderVM.IdLicenceStatus.Value))?.Name;

                string licenseNumber = string.IsNullOrEmpty(candidateProviderVM.LicenceNumber) && !candidateProviderVM.LicenceDate.HasValue ? "" : candidateProviderVM.LicenceNumberWithDate;
                if (!string.IsNullOrEmpty(licenseNumber))
                {
                    this.title = this.isCPO
                        ? $"Данни за <span style=\"color: #ffffff;\">{this.candidateProviderVM.CPONameAndOwner}</span>, Лицензия <span style=\"color: #ffffff;\">№&nbsp;{licenseNumber}</span>, Статус на лицензията: <span style=\"color: #ffffff;\">{this.candidateProviderVM.LicenceStatusName}</span>"
                        : $"Данни за <span style=\"color: #ffffff;\">{this.candidateProviderVM.CIPONameAndOwner}</span>, Лицензия <span style=\"color: #ffffff;\">№&nbsp;{licenseNumber}</span>, Статус на лицензията: <span style=\"color: #ffffff;\">{this.candidateProviderVM.LicenceStatusName}</span>";
                }
                else
                {
                    this.title = this.isCPO
                        ? $"Данни за <span style=\"color: #ffffff;\">{this.candidateProviderVM.CPONameAndOwner}<span>, Статус на лицензията: <span style=\"color: #ffffff;\">{this.candidateProviderVM.LicenceStatusName}</span>"
                        : $"Данни за <span style=\"color: #ffffff;\">{this.candidateProviderVM.CIPONameAndOwner}<span>, Статус на лицензията: <span style=\"color: #ffffff;\">{this.candidateProviderVM.LicenceStatusName}</span>";
                }
            }
            else
            {
                var applicationNumber = !string.IsNullOrEmpty(this.candidateProviderVM.ApplicationNumber) ? $"{this.candidateProviderVM.ApplicationNumber}/" : string.Empty;
                var applicationDate = candidateProviderVM.ApplicationDate == null ? string.Empty : $"{this.candidateProviderVM.ApplicationDate.Value.ToString("dd.MM.yyyy")} г.";
                var applicationNumberAndDate = applicationNumber + applicationDate;
                if (this.isLicenceChange)
                {
                    this.title = $"Заявление за изменение на лицензия {(string.IsNullOrEmpty(applicationNumberAndDate) || string.IsNullOrWhiteSpace(applicationNumberAndDate) ? "" : "№ ")}<span style=\"color: #ffffff;\">{applicationNumberAndDate}</span> <span style=\"color: #ffffff;\">{this.candidateProviderVM.CPONameAndOwner}</span>, Статус: <span style=\"color: #ffffff;\">{this.candidateProviderVM.ApplicationStatus}</span>";
                }
                else
                {
                    this.title = this.isCPO
                        ? $"Заявление за лицензиране {(string.IsNullOrEmpty(applicationNumberAndDate) || string.IsNullOrWhiteSpace(applicationNumberAndDate) ? "" : "№ ")}<span style=\"color: #ffffff;\">{applicationNumberAndDate}</span> <span style=\"color: #ffffff;\">{this.candidateProviderVM.CPONameAndOwner}</span>, Статус: <span style=\"color: #ffffff;\">{this.candidateProviderVM.ApplicationStatus}</span>"
                        : $"Заявление за лицензиране {(string.IsNullOrEmpty(applicationNumberAndDate) || string.IsNullOrWhiteSpace(applicationNumberAndDate) ? "" : "№ ")}<span style=\"color: #ffffff;\">{applicationNumberAndDate}</span> <span style=\"color: #ffffff;\">{this.candidateProviderVM.CPONameAndOwner}</span>, Статус: <span style=\"color: #ffffff;\">{this.candidateProviderVM.ApplicationStatus}</span>";
                }
            }
        }

        private void CheckForRolesExternalExpertOrExpertCommittees()
        {
            this.isUserExternalExpertOrExpertCommittee = this.GetUserRoles().Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES");
        }

        private void CheckForRoleNapoo()
        {
            this.disableFieldsWhenUserIsNAPOO = this.GetUserRoles().Any(x => x.StartsWith("NAPOO"));
        }

        private async Task CheckForUserProfileAdministratorAsync()
        {
            this.isUserProfileAdministrator = await this.IsPersonProfileAdministratorAsync();
        }

        private void CheckForApplicationStatusDocumentPreparation()
        {
            if (this.candidateProviderVM.IdApplicationStatus.HasValue && !this.disableFieldsWhenOpenFromProfile)
            {
                this.disableFieldsWhenApplicationStatusIsNotDocPreparation = this.candidateProviderVM.IdApplicationStatus != this.kvDocumentPreparation.IdKeyValue;
            }
            else
            {
                this.disableFieldsWhenApplicationStatusIsNotDocPreparation = false;
            }
        }

        // проверява дали candidate provider е ЦПО
        private async Task SetIsCPOOrCIPOAsync()
        {
            var kvLicenceCPOValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
            this.isCPO = this.candidateProviderVM.IdTypeLicense == kvLicenceCPOValue.IdKeyValue;
        }

        // Проверява за промени в ДОС-а и неактулизарани учебни планове
        private async Task HandleDOSNotSubmittedChangesAsync()
        {
            var isDOSChangeWithoutActualizationMsgList = await this.CheckForDOSChangesWithoutActualizationOfCurriculumsAsync();
            if (isDOSChangeWithoutActualizationMsgList.Any())
            {
                List<string> dosErrorMsgList = new List<string>();
                if (isDOSChangeWithoutActualizationMsgList.Count > 2)
                {
                    for (int i = 0; i < isDOSChangeWithoutActualizationMsgList.Count - 1; i++)
                    {
                        var specialityInfo = isDOSChangeWithoutActualizationMsgList[i];
                        var dosStartDateInfo = isDOSChangeWithoutActualizationMsgList[i + 1];

                        dosErrorMsgList.Add($"Моля, актуализирайте в профила на ЦПО учебния план и учебните програми за специалност '{specialityInfo}' в съответствие с промените в държавния образователен стандарт в сила от {dosStartDateInfo}!");
                    }
                }
                else
                {
                    var specialityInfo = isDOSChangeWithoutActualizationMsgList[0];
                    var dosStartDateInfo = isDOSChangeWithoutActualizationMsgList[1];
                    dosErrorMsgList.Add($"Моля, актуализирайте в профила на ЦПО учебния план и учебните програми за специалност '{specialityInfo}' в съответствие с промените в държавния образователен стандарт в сила от {dosStartDateInfo}!");
                }

                this.ShowAlertDialog(string.Join(Environment.NewLine, dosErrorMsgList));
            }
        }

        // Проверява за промени в ДОС-а и неактулизарани учебни планове
        private async Task<List<string>> CheckForDOSChangesWithoutActualizationOfCurriculumsAsync()
        {
            return await this.CandidateProviderService.AreDOSChangesWithoutActualizationOfCurriculumsAsync(this.candidateProviderVM);
        }

        // спира смяната на табове при Swipe на мишката
        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private async Task OpenFormApplicationModal()
        {
            if (!FormApplicationStatus)
            {
                FormApplicationStatus = true;
                await formApplicationModal.OpenModal();
            }
        }

        private async Task Submit()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.validationMessages.Clear();

                this.trainingInstitution.SubmitHandler();
                this.validationMessages.AddRange(this.trainingInstitution.GetValidationMessages());

                if (!this.validationMessages.Any() && this.structureAndActivities.IsEditContextModified())
                {
                    await this.structureAndActivities.SubmitHandler();
                    this.validationMessages.AddRange(this.structureAndActivities.GetValidationMessages());
                }

                if (!this.validationMessages.Any() && this.trainers.editContextGeneralData is not null)
                {
                    await this.trainers.SubmitHandler();
                    var messagesFromTrainers = new List<string>();
                    messagesFromTrainers.AddRange(this.trainers.editContextGeneralData.GetValidationMessages());
                    if (this.isCPO)
                    {
                        messagesFromTrainers.AddRange(this.trainers.editContextTrainerProfile.GetValidationMessages());
                    }

                    messagesFromTrainers = messagesFromTrainers.Select(x => $"{x} (Преподаватели)").ToList();
                    this.validationMessages.AddRange(messagesFromTrainers);
                }

                if (!this.validationMessages.Any() && this.materialTechnicalBase.IsEditContextModified())
                {
                    await this.materialTechnicalBase.SubmitHandler();
                    this.validationMessages.AddRange(this.materialTechnicalBase.GetValidationMessages());
                }

                if (!this.validationMessages.Any())
                {
                    var inputContext = new ResultContext<CandidateProviderVM>();
                    inputContext.ResultContextObject = this.candidateProviderVM;
                    var result = await this.CandidateProviderService.UpdateCandidateProviderAsync(inputContext);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));

                        await this.LoadCandidateProviderDataAsync(inputContext.ResultContextObject);

                        // презарежда данните за потребители, които да получават известия след ъпдейт
                        await this.ReloadPersonsForNotificationsDataAsync();

                        await this.SetTitleAsync();

                        await this.RefreshAppliationList();
                    }
                }
                else
                {
                    for (int i = 0; i < this.validationMessages.Count; i++)
                    {
                        var newError = this.validationMessages[i].Replace("ЦПО/ЦИПО", "ЦПО");
                        var idx = this.validationMessages.IndexOf(this.validationMessages[i]);
                        this.validationMessages.RemoveAt(idx);
                        this.validationMessages.Insert(idx, newError);
                    }

                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }
            }
            finally
            {
                this.loading = false;
            }

            this.StateHasChanged();

            this.SpinnerHide();
        }

        // презарежда данните за потребители, които да получават известия след ъпдейт
        private async Task ReloadPersonsForNotificationsDataAsync()
        {
            if (this.applicationTab.Items[this.selectedTab].Header.Text == "Обучаваща институция")
            {
                if (this.candidateProviderVM.IdCandidateProviderActive is not null)
                {
                    this.candidateProviderVM.PersonsForNotifications =
                    (await this.ProviderService
                    .GetAllPersonsForNotificationByCandidateProviderIdAsync(this.candidateProviderVM.IdCandidateProviderActive.Value)).ToList();
                }
                else
                {
                    this.candidateProviderVM.PersonsForNotifications =
                    (await this.ProviderService
                    .GetAllPersonsForNotificationByCandidateProviderIdAsync(this.candidateProviderVM.IdCandidate_Provider)).ToList();
                }
            }
        }

        // презарежда данните на CandidateProvider след стартиране на процедура
        private async Task ReloadCandidateProviderDataAfterStartedProcedureAsync()
        {
            await this.LoadCandidateProviderDataAsync(this.candidateProviderVM);

            this.CheckForApplicationStatusDocumentPreparation();
        }

        private async Task RefreshAppliationList()
        {
            if (this.disableFieldsWhenOpenFromProfile)
            {
                await this.CallbackAfterProfileSubmit.InvokeAsync();
            }
            else
            {
                await this.CallbackAfterSubmit.InvokeAsync();
            }

            if (this.candidateProviderVM.IdApplicationStatus.HasValue && this.candidateProviderVM.IdApplicationStatus == this.kvDocumentPreparation.IdKeyValue)
            {
                await this.CalculcateApplicationProgressAsync();
            }
        }

        private async Task CalculcateApplicationProgressAsync()
        {
            if (this.candidateProviderVM.IdStartedProcedure.HasValue)
            {
                this.percentage = 100;
                return;
            }

            double weight = 0;
            if (this.candidateProviderVM.IdProviderOwnership != 0)
            {
                weight = 0.1;
            }

            if (this.isCPO && await this.CandidateProviderService.AreAnyCandidateProviderSpecialitiesByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider))
            {
                weight += 0.2;
            }

            if (await this.CandidateProviderService.AreAnyCandidateProviderTrainersByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider))
            {
                weight += this.isCPO
                    ? 0.1
                    : 0.2;
            }

            if (await this.CandidateProviderService.AreAnyCandidateProviderPremisesByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider))
            {
                weight += this.isCPO
                    ? 0.1
                    : 0.2;
            }

            if (await this.CandidateProviderService.AreAnyCandidateProviderDocumentsByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider))
            {
                weight += 0.1;
            }

            var settingResource = (await this.SettingService.GetSettingByIntCodeAsync("ResourcesFolderName")).SettingValue;
            var pathName = settingResource + @$"\UploadedFiles\CandidateProvider\{this.candidateProviderVM.IdCandidate_Provider}\FormApplication.docx";
            var hasFile = File.Exists(pathName);
            if (hasFile)
            {
                weight += 0.1;
            }

            if (this.candidateProviderVM.IdReceiveLicense.HasValue && this.candidateProviderVM.IdApplicationFiling.HasValue)
            {
                weight += 0.1;
            }

            var structureAndActivity = await this.CandidateProviderService.GetCandidateProviderCPOStructureActivityByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider);
            if (structureAndActivity is not null)
            {
                weight += 0.1;
            }

            double sum = (weight / 1) * 100;
            this.percentage = (int)Math.Floor(sum);
        }

        private async Task CancelLicenceChangeBtn()
        {
            bool confirmed = await this.ShowConfirmDialogAsync("Сигурни ли сте, че искате да откажете процедурата по изменение на лицензията?");
            if (confirmed)
            {
                if (this.isLicenceChange && this.candidateProviderVM.IdApplicationStatus.HasValue && this.candidateProviderVM.IdApplicationStatus.Value == this.kvDocumentPreparation.IdKeyValue)
                {
                    this.SpinnerShow();

                    if (this.loading)
                    {
                        return;
                    }
                    try
                    {
                        this.loading = true;
                        var kvProcedureCanceledByProviderValue = await this.DataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureTerminatedByCenter");
                        await this.CandidateProviderService.ChangeCandidateProviderApplicationStatusAsync(this.candidateProviderVM.IdCandidate_Provider, kvProcedureCanceledByProviderValue.IdKeyValue);

                        await this.CallbackAfterSubmit.InvokeAsync();

                        await this.ShowSuccessAsync("Процедурата по изменение на лицензията е отказана успешно!");

                        this.isVisible = false;
                        this.StateHasChanged();
                    }
                    finally
                    {
                        this.loading = false;
                    }

                    this.SpinnerHide();
                }
            }
        }
    }
}
