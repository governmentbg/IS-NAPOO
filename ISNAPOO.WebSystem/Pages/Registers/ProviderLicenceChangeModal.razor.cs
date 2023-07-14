using Data.Models.Data.Candidate;
using Data.Models.Data.ProviderData;
using Data.Models.Migrations;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Registers
{
    public partial class ProviderLicenceChangeModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        ToastMsg toast = new ToastMsg();
        ApplicationUser users = new ApplicationUser();
        private CandidateProviderLicenceChangeVM candidateProviderLicesingVM = new CandidateProviderLicenceChangeVM();
        private List<CandidateProviderLicenceChangeVM> candidateProviderLicesingList = new List<CandidateProviderLicenceChangeVM>();
        private IEnumerable<KeyValueVM> kvLicenseChangeStatus = new List<KeyValueVM>();
        private KeyValueVM kvStatusActiveUser = new KeyValueVM();
        private KeyValueVM kvStatusInActiveUser = new KeyValueVM();
        private KeyValueVM kvDefTakeAway = new KeyValueVM();
        private List<ApplicationUser> activeApplicationUsers = new List<ApplicationUser>();
        private List<ApplicationUser> activeApplicationUsersOrigList = new List<ApplicationUser>();

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        [Parameter]
        public EventCallback<bool> CallbackAfterSubmit { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IPersonService personService { get; set; }

        [Inject]

        public IApplicationUserService appApplicantUserService { get; set; }

        List<string> validationMessages = new List<string>();

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateProviderLicesingVM);
        }

        private async Task SubmitChangeLicenseHandler()
        {
            this.SpinnerShow();
            this.editContext = new EditContext(this.candidateProviderLicesingVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.kvDefTakeAway = this.kvLicenseChangeStatus.FirstOrDefault(x => x.KeyValueIntCode == "DefinitivelyTakenAway");
            this.kvStatusActiveUser = await this.dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "Active");
            this.kvStatusInActiveUser = await this.dataSourceService.GetKeyValueByIntCodeAsync("UserStatus", "InActive");
            ResultContext<CandidateProviderLicenceChangeVM> resultContext = new ResultContext<CandidateProviderLicenceChangeVM>();
            
            if (this.editContext.Validate())
            {
                if (this.candidateProviderLicesingVM.IdCandidateProviderLicenceChange != 0)
                {
                    resultContext = await this.CandidateProviderService.UpdateCandidateProviderLicenceChangeAsync(this.candidateProviderLicesingVM);
                }
                else
                {
                    resultContext = await this.CandidateProviderService.CreateCandidateProviderLicenceChangeAsync(this.candidateProviderLicesingVM);
                }

                if (this.kvDefTakeAway is not null && resultContext.ResultContextObject.IdStatus == this.kvDefTakeAway.IdKeyValue)
                {
                    this.activeApplicationUsers = await this.personService.GetPersonByIdCandidateProvider(this.candidateProviderLicesingVM.IdCandidate_Provider, this.kvStatusActiveUser.IdKeyValue);
                    if (this.activeApplicationUsers.Any())
                    {
                        resultContext = await this.appApplicantUserService.UpdateApplicationStatusUserAsync(this.activeApplicationUsers, "MakeUnActiveUsers", this.kvStatusInActiveUser.IdKeyValue);
                    }
                }
                else 
                {
                    this.activeApplicationUsers = await this.personService.GetPersonByIdCandidateProvider(this.candidateProviderLicesingVM.IdCandidate_Provider, this.kvStatusActiveUser.IdKeyValue);
                    if (!this.activeApplicationUsers.Any())
                    {
                        resultContext = await this.appApplicantUserService.UpdateApplicationStatusUserAsync(this.activeApplicationUsersOrigList, "MakeActiveUsers", this.kvStatusActiveUser.IdKeyValue);
                    }
                }


                if (resultContext.HasErrorMessages)
                {
                    this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                }
                else
                {
                    this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    await this.CallbackAfterSubmit.InvokeAsync();
                }
            }

            this.validationMessages.Clear();
            this.validationMessages.AddRange(this.editContext.GetValidationMessages());

            this.SpinnerHide();
            this.StateHasChanged();
        }

        public async Task OpenModal(CandidateProviderLicenceChangeVM candidateProviderLicenceChangeVM,IEnumerable<CandidateProviderLicenceChangeVM> candidateProviderLicenceChangeOrig, List<ApplicationUser> appUsers)
        {
            this.validationMessages.Clear();
            var candidateProviderLicense = await this.CandidateProviderService.GetCandidateProviderChangeLicenseByIdAsync(candidateProviderLicenceChangeVM.IdCandidate_Provider, candidateProviderLicenceChangeVM.IdCandidateProviderLicenceChange);
            this.candidateProviderLicesingVM = candidateProviderLicense;
                    
            this.activeApplicationUsersOrigList = appUsers;
            
            this.kvLicenseChangeStatus = await this.dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicenseStatus");
            this.kvLicenseChangeStatus = this.kvLicenseChangeStatus.OrderBy(x => x.Order);

            if (candidateProviderLicenceChangeOrig.Any())
            {
                this.candidateProviderLicesingList = candidateProviderLicenceChangeOrig.ToList();
                var latestLiceningChange = this.candidateProviderLicesingList.OrderByDescending(x => x.CreationDate).First();
                if (latestLiceningChange.LicenceStatusNameEn != null && (latestLiceningChange.LicenceStatusNameEn == "SuspendedFor4Months" 
                    || latestLiceningChange.LicenceStatusNameEn == "SuspendedFor6Months" 
                    || latestLiceningChange.LicenceStatusNameEn == "SuspendedFor3Months"
                    || latestLiceningChange.LicenceStatusNameEn == "active"))
                {
                    this.kvLicenseChangeStatus = this.kvLicenseChangeStatus.Where(s => s.KeyValueIntCode != "DocumentsPresented" && s.KeyValueIntCode != "Deleted");
                    
                }
                else
                {
                    this.kvLicenseChangeStatus = this.kvLicenseChangeStatus.Where(s => s.KeyValueIntCode != "active" && s.KeyValueIntCode != "DocumentsPresented" && s.KeyValueIntCode != "Deleted");
                }
            }
            else
            {
                this.kvLicenseChangeStatus = this.kvLicenseChangeStatus.Where(s => s.KeyValueIntCode != "active" && s.KeyValueIntCode != "DocumentsPresented" && s.KeyValueIntCode != "Deleted");
            }


            this.isVisible = true;
            this.StateHasChanged();
        }

    }
}
