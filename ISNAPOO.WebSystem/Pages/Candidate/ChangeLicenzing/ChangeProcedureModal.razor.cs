using DocuWorkService;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate.ChangeLicenzing
{
    public partial class ChangeProcedureModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();
        private ChangeCompletenessCheckModal completenessCheck = new ChangeCompletenessCheckModal();
        private ChangeProcedureDocuments procedureDocuments = new ChangeProcedureDocuments();
        private ChangeOpportunityAssessment opportunityAssessment = new ChangeOpportunityAssessment();
        private ProcedureNotificationsSent procedureNotificationsSent = new ProcedureNotificationsSent();
        private ProcedureDeadlines procedureDeadlines = new ProcedureDeadlines();
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        List<string> validationMessages = new List<string>();
        int selectedTab = 0;

        //public override bool IsContextModified { get => this.trainingInstitution.IsEditContextModified() || this.specialities.IsEditContextModified(); } 

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        public async Task OpenModal(CandidateProviderVM candidateProviderVM)
        {
            this.selectedTab = 0;
            this.validationMessages.Clear();
            this.candidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(candidateProviderVM);
            this.isVisible = true;
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
            if (this.selectedTab == 4)
            {
                this.SpinnerShow();

                await this.procedureNotificationsSent.LoadDataAsync();

                this.SpinnerHide();
            }
        }
    }
}
