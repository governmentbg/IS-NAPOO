using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOProcedureModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();
        private CIPOCompletenessCheckModal completenessCheck = new CIPOCompletenessCheckModal();
        private CIPOProcedureDocuments procedureDocuments = new CIPOProcedureDocuments();
        private CIPOOpportunityAssessment opportunityAssessment = new CIPOOpportunityAssessment();
        private CIPOLicenseIssuing licenseIssuing = new CIPOLicenseIssuing();
        private ProcedureNotificationsSent procedureNotificationsSent = new ProcedureNotificationsSent();
        private ProcedureDeadlines procedureDeadlines = new ProcedureDeadlines();
        private ExternalExpertsReports externalExpertsReports = new ExternalExpertsReports();
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        List<string> validationMessages = new List<string>();
        int selectedTab = 0;
        private bool hideTabs = false;

        //public override bool IsContextModified { get => this.trainingInstitution.IsEditContextModified() || this.specialities.IsEditContextModified(); } 

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public async Task OpenModal(CandidateProviderVM candidateProviderVM)
        {
            this.SpinnerShow();

            this.selectedTab = 0;
            this.validationMessages.Clear();
            this.candidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(candidateProviderVM);

            this.HideTabsAccordingToRole();

            this.isVisible = true;
            this.SpinnerHide();
            this.StateHasChanged();
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
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }
            this.SpinnerShow();
            this.validationMessages.Clear();
            this.completenessCheck.SubmitHandler();
            this.validationMessages.AddRange(this.completenessCheck.GetValidationMessages());
            this.licenseIssuing.SubmitHandler();
            this.validationMessages.AddRange(this.licenseIssuing.GetValidationMessages());

            if (!this.validationMessages.Any())
            {
                await this.completenessCheck.Save();

                await this.HandleTabSelection();
            }

            this.SpinnerHide();
        }

        private async Task CheckInRegix()
        {
            await this.completenessCheck.CheckInRegix();
        }

        private async Task RefreshDocumentsGrid()
        {
            await this.procedureDocuments.RefreshGridDocuments();
        }

        private async Task OnTabSelected(SelectEventArgs args)
        {
            await this.HandleTabSelection();
        }

        private async Task HandleTabSelection()
        {
            if (this.selectedTab == 5)
            {
                this.SpinnerShow();

                await this.procedureNotificationsSent.LoadDataAsync();

                this.SpinnerHide();
            }
        }

        private void HideTabsAccordingToRole()
        {
            var userRoles = this.GetUserRoles();
            if (userRoles.Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES"))
            {
                this.hideTabs = true;
            }
            else
            {
                this.hideTabs = false;
            }
        }
    }
}
