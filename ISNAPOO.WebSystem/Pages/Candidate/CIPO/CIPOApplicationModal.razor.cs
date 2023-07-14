using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common.Concurrency;
using ISNAPOO.Common.HelperClasses;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOApplicationModal : BlazorBaseComponent, IConcurrencyCheck<CandidateProviderVM>
    {
        private SfDialog sfDialog = new SfDialog();
        private CIPOTrainingInstitution trainingInstitution = new CIPOTrainingInstitution();
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        private CIPOTrainers trainers = new CIPOTrainers();
        private CIPOMaterialTechnicalBase materialTechnicalBase = new CIPOMaterialTechnicalBase();
        private CIPOCandidateProviderDocuments candidateProviderDocuments = new CIPOCandidateProviderDocuments();
        private CIPOApplicationStatus applicationStatus = new CIPOApplicationStatus();
        private CIPOFormApplicationModal formApplicationModal = new CIPOFormApplicationModal();
        private CIPOStructureAndActivities cipoStructureAndActivities = new CIPOStructureAndActivities();
        private LicenceChange licenceChange = new LicenceChange();
        private string kvApplicationStatus = string.Empty;
        private IEnumerable<KeyValueVM> kvApplicationStatusSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvApplicationTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvProviderDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvMTBDocumentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvTrainerDocumentTypeSource = new List<KeyValueVM>();
        private KeyValueVM kvApplicationChange = new KeyValueVM();
        private KeyValueVM kvDocumentPreparation = new KeyValueVM();
        private bool isProfileSubmitted = false;
        private bool isLicenceChange = false;
        private bool disabled = false;
        private int percentage = 0;
        private bool openFromCIPO = false;
        private bool disableFields = false;
        private bool FormApplicationStatus = false;
        private bool hideApplicationStatusTab = false;
        private string applicationNumberAndDate = string.Empty;
        private bool isUserExternalExpertOrExpertCommittee = false;
        private bool hideBtnsConcurrentModal = false;
        private bool hideActionsWhenStatusIsCompleted = false;
        private bool isInRoleNAPOO = false;
        private string licenceNumberAndDate = string.Empty;

        ExpertVM model = new ExpertVM();

        List<string> validationMessages = new List<string>();
        int selectedTab = 0;

        public override bool IsContextModified
        {
            get => this.trainingInstitution.IsEditContextModified()
                    || this.materialTechnicalBase.IsEditContextModified()
                    || this.trainers.editContextGeneralData.IsModified();
        }

        [Parameter]
        public EventCallback<ResultContext<CandidateProviderVM>> CallbackAfterSubmit { get; set; }

        [Parameter]
        public EventCallback<ResultContext<CandidateProviderVM>> CallbackAfterProfileSubmit { get; set; }
        
        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IProviderService ProviderService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public ISettingService SettingService { get; set; }

        public async Task OpenModal(CandidateProviderVM candidateProviderVM, bool isProfileSubmitted = false, bool isLicenceChange = false, bool openFromRegisterCIPO = false, bool hideApplicationStatusTab = false, ConcurrencyInfo concurrencyInfo = null)
        {
            this.selectedTab = 0;
            this.disableFields = openFromRegisterCIPO;
            this.openFromCIPO = openFromRegisterCIPO;
            this.validationMessages.Clear();
            this.percentage = 0;
            this.IdCandidateProvider = candidateProviderVM.IdCandidate_Provider;

            this.kvApplicationStatusSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ApplicationStatus");
            this.kvApplicationTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TypeApplication");
            this.kvApplicationChange = this.kvApplicationTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "ChangeLicenzing");
            this.kvDocumentPreparation = this.kvApplicationStatusSource.FirstOrDefault(x => x.KeyValueIntCode == "PreparationDocumentationLicensing");

            if (this.candidateProviderVM.IdApplicationStatus != null)
            {
                var status = this.kvApplicationStatusSource.FirstOrDefault(x => x.IdKeyValue == this.candidateProviderVM.IdApplicationStatus);

                if (status is not null)
                {
                    this.candidateProviderVM.ApplicationStatus = status.Name;
                }
            }

            if (isProfileSubmitted)
            {
                this.isProfileSubmitted = true;
                this.hideActionsWhenStatusIsCompleted = true;
            }

            if (hideApplicationStatusTab)
            {
                this.hideApplicationStatusTab = hideApplicationStatusTab;
            }

            this.isLicenceChange = isLicenceChange;

            this.candidateProviderVM = candidateProviderVM;
            var applicationStatus = this.kvApplicationStatusSource.FirstOrDefault(x => x.IdKeyValue == this.candidateProviderVM.IdApplicationStatus);
            if (applicationStatus is not null)
            {
                this.candidateProviderVM.ApplicationStatus = applicationStatus.Name;
            }

            if (this.kvDocumentPreparation != null)
            {
                if (this.candidateProviderVM.IdApplicationStatus == this.kvDocumentPreparation.IdKeyValue)
                {
                    await this.CalculcatePercentage();
                }
            }

            this.applicationNumberAndDate = !string.IsNullOrEmpty(this.candidateProviderVM.ApplicationNumber) ? $"№ {this.candidateProviderVM.ApplicationNumber}/" + this.candidateProviderVM.ApplicationDate == null ? string.Empty : this.candidateProviderVM.ApplicationDate.Value.ToString("dd.MM.yyyy") + "г." : string.Empty;

            this.CheckForRolesExternalExpertOrExpertCommittees();
            this.CheckForRoleNapoo();

            if (!this.isUserExternalExpertOrExpertCommittee)
            {
                if (concurrencyInfo is not null && concurrencyInfo.IdPerson != this.UserProps.IdPerson)
                {
                    this.hideBtnsConcurrentModal = true;
                    this.SetPersonFullNameAndVisibilityOfDialog(concurrencyInfo);
                }
                else
                {
                    this.hideBtnsConcurrentModal = false;
                }
            }
            this.licenceNumberAndDate = string.IsNullOrEmpty(candidateProviderVM.LicenceNumber) && !candidateProviderVM.LicenceDate.HasValue ? "" : candidateProviderVM.LicenceNumberWithDate;

            if (this.candidateProviderVM.IdLicenceStatus.HasValue)
            {
                this.candidateProviderVM.LicenceStatusName = (await this.DataSourceService.GetKeyValueByIdAsync(this.candidateProviderVM.IdLicenceStatus.Value))?.Name;
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task HandleRowSelectionInTabs()
        {
            if (this.selectedTab == 1)
            {
                await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
            }
            else if (this.selectedTab == 2)
            {
                await this.trainers.trainersGrid.SelectRowAsync(this.trainers.selectedRowIdx);
            }
            else if (this.selectedTab == 3)
            {
                await this.materialTechnicalBase.mtbsGrid.SelectRowAsync(this.materialTechnicalBase.selectedRowIdx);
            }
        }

        // спира смяната на табове при Swipe на мишката
        private void Select(SelectingEventArgs args)
        {
            if (args.IsSwiped)
            {
                args.Cancel = true;
            }
        }

        private async Task Submit()
        {
            if (this.disabled)
            {
                return;
            }
            try
            {
                this.disabled = true;

                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

                this.SpinnerShow();

                this.validationMessages.Clear();
                this.trainingInstitution.SubmitHandler();
                this.validationMessages.AddRange(this.trainingInstitution.GetValidationMessages());
                await this.cipoStructureAndActivities.SubmitHandler();
                this.validationMessages.AddRange(this.cipoStructureAndActivities.GetValidationMessages());

                if (!this.trainers.candidateProviderTrainerVM.IsEmpty())
                {
                    this.trainers.SubmitHandler();
                    this.validationMessages.AddRange(this.trainers.editContextGeneralData.GetValidationMessages());
                }

                if (!this.materialTechnicalBase.candidateProviderPremisesVM.IsEmpty())
                {
                    this.materialTechnicalBase.SubmitHandler();
                    this.validationMessages.AddRange(this.materialTechnicalBase.GetValidationMessages());
                }

                if (!this.validationMessages.Any())
                {
                    var inputContext = new ResultContext<CandidateProviderVM>();

                    inputContext.ResultContextObject = this.candidateProviderVM;
                    var resultContext = new ResultContext<CandidateProviderVM>();
                    if (!this.isLicenceChange)
                    {
                        resultContext = await this.CandidateProviderService.CreateApplicationAsync(inputContext);
                        await this.trainers.RefreshTrainersData();
                        await this.materialTechnicalBase.RefreshPremisesData();
                    }
                    else
                    {
                        inputContext.ResultContextObject.IdTypeApplication = this.kvApplicationChange.IdKeyValue;
                        resultContext = await this.CandidateProviderService.CreateApplicationChangeAsync(inputContext);
                    }

                    this.candidateProviderVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateProviderVM.IdModifyUser);
                    this.candidateProviderVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateProviderVM.IdCreateUser);

                    this.candidateProviderVM.CandidateProviderSpecialities = (await this.CandidateProviderService.GetAllCandidateProviderSpecialitiesByIdCandidateProvider(this.candidateProviderVM.IdCandidate_Provider)).ToList();
                    await this.RefreshApplciationList(resultContext);
                    await this.HandleRowSelectionInTabs();
                }
                else
                {
                    for (int i = 0; i < this.validationMessages.Count; i++)
                    {
                        var newError = this.validationMessages[i].Replace("ЦПО/ЦИПО", "ЦИПО");
                        var idx = this.validationMessages.IndexOf(this.validationMessages[i]);
                        this.validationMessages.RemoveAt(idx);
                        this.validationMessages.Insert(idx, newError);
                    }

                    await this.JsRuntime.InvokeVoidAsync("scrollToErrors");
                }

                this.SpinnerHide();
            }
            finally
            {
                this.disabled = false;
            }
        }

        private void CloseModal()
        {
            this.isVisible = false;
        }

        protected override async Task OnInitializedAsync()
        {
            this.selectedTab = 0;
            this.editContext = new EditContext(this.model);
        }

        private async Task OnTabSelected(SelectEventArgs args)
        {
            this.SpinnerShow();
            if (!this.isUserExternalExpertOrExpertCommittee)
            {
                if (this.selectedTab == 6)
                {
                    this.kvProviderDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType");
                    this.kvMTBDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType");
                    this.kvTrainerDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
                    this.candidateProviderDocuments.documentsSource = (await this.CandidateProviderService.SetDataForDocumentsGrid(this.candidateProviderVM.IdCandidate_Provider, this.kvProviderDocumentTypeSource, this.kvMTBDocumentTypeSource, this.kvTrainerDocumentTypeSource)).ToList();
                }
            }
            else 
            {
                if (this.selectedTab == 5)
                {
                    this.kvProviderDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("CandidateProviderDocumentType");
                    this.kvMTBDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("MTBDocumentType");
                    this.kvTrainerDocumentTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainerDocumentType");
                    this.candidateProviderDocuments.documentsSource = (await this.CandidateProviderService.SetDataForDocumentsGrid(this.candidateProviderVM.IdCandidate_Provider, this.kvProviderDocumentTypeSource, this.kvMTBDocumentTypeSource, this.kvTrainerDocumentTypeSource)).ToList();
                }
            }

            this.SpinnerHide();
        }

        private async Task RefreshApplciationList(ResultContext<CandidateProviderVM> resultContext)
        {
            if (this.isProfileSubmitted)
            {
                await this.CallbackAfterProfileSubmit.InvokeAsync(resultContext);
            }
            else
            {
                await this.CallbackAfterSubmit.InvokeAsync(resultContext);
            }

            if (this.kvDocumentPreparation != null)
            {
                if (this.candidateProviderVM.IdApplicationStatus == this.kvDocumentPreparation.IdKeyValue)
                {
                    await this.CalculcatePercentage();
                }
            }
        }

        private async Task CalculcatePercentage()
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

            if (this.candidateProviderVM.CandidateProviderTrainers.Any())
            {
                weight += 0.2;
            }

            if (this.candidateProviderVM.CandidateProviderPremises.Any())
            {
                weight += 0.2;
            }

            if (this.candidateProviderVM.CandidateProviderDocuments.Any())
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

            if (this.candidateProviderVM.IdReceiveLicense != null && this.candidateProviderVM.IdApplicationFiling != null)
            {
                weight += 0.1;
            }

            var structureAndActivity = await this.CandidateProviderService.GetCandidateProviderCIPOStructureActivityByIdCandidateProviderAsync(this.candidateProviderVM.IdCandidate_Provider);
            if (structureAndActivity is not null)
            {
                weight += 0.1;
            }

            double sum = (weight / 1) * 100;
            this.percentage = (int)Math.Floor(sum);
        }

        private async Task OpenFormApplicationModal()
        {
            if (!FormApplicationStatus)
            {
                FormApplicationStatus = true;
                await formApplicationModal.OpenModal();
            }
        }

        private void CheckForRolesExternalExpertOrExpertCommittees()
        {
            this.isUserExternalExpertOrExpertCommittee = this.GetUserRoles().Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES");
        }

        private void CheckForRoleNapoo()
        {
            this.isInRoleNAPOO = this.GetUserRoles().Any(x => x.StartsWith("NAPOO"));
        }
    }
}
