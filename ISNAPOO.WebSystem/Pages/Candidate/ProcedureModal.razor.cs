using DocuWorkService;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ProcedureModal : BlazorBaseComponent
    {
        private CompletenessCheckModal completenessCheck = new CompletenessCheckModal();
        private ProcedureDocuments procedureDocuments = new ProcedureDocuments();
        private OpportunityAssessment opportunityAssessment = new OpportunityAssessment();
        private LicenseIssuing licenseIssuing = new LicenseIssuing();
        private ProcedureNotificationsSent procedureNotificationsSent = new ProcedureNotificationsSent();
        private ProcedureDeadlines procedureDeadlines = new ProcedureDeadlines();
        private ExternalExpertsReports externalExpertsReports = new ExternalExpertsReports();
        private CandidateProviderVM candidateProviderVM = new CandidateProviderVM();
        private ExpertVM expert = new ExpertVM();
        List<string> validationMessages = new List<string>();
        int selectedTab = 0;
        private bool hideTabs = false;
        private bool isLicenceChange = false;
        private string title = string.Empty;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDocuService DocuService { get; set; }

        [Inject]
        public IExpertService ExpertService { get; set; }

        public async Task OpenModal(CandidateProviderVM candidateProviderVM, bool isLicenceChange)
        {
            this.selectedTab = 0;
            this.validationMessages.Clear();
            this.candidateProviderVM = await this.CandidateProviderService.GetCandidateProviderByIdAsync(candidateProviderVM);

            this.HideTabsAccordingToRole();

            this.SetTitle();

            this.isLicenceChange = isLicenceChange;

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void SetTitle()
        {
            var providerInfo = !string.IsNullOrEmpty(this.candidateProviderVM.ProviderName)
                ? $"ЦПО {this.candidateProviderVM.ProviderName} към {this.candidateProviderVM.ProviderOwner}"
                : $"ЦПО към {this.candidateProviderVM.ProviderOwner}";

            this.title = !this.isLicenceChange
                ? $"Процедура за лицензиране Заявление <span style=\"font-size: 18px;color: white !important;\">№ {this.candidateProviderVM.ApplicationNumber}/{this.candidateProviderVM.ApplicationDateFormated}</span>, <span style=\"font-size: 18px;color: white !important;\">{providerInfo}</span>, Статус: <span style=\"font-size: 18px;color: white !important;\">{this.candidateProviderVM.ApplicationStatus}</span>"
                : $"Процедура за изменение на лицензия Заявление <span style=\"font-size: 18px;color: white !important;\">№ {this.candidateProviderVM.ApplicationNumber}/{this.candidateProviderVM.ApplicationDateFormated}</span>, <span style=\"font-size: 18px;color: white !important;\">{providerInfo}</span>, Статус: <span style=\"font-size: 18px;color: white !important;\">{this.candidateProviderVM.ApplicationStatus}</span>";
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
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
                if (!hasPermission) { return; }

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
            }
            finally
            {
                this.loading = false;
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
            if (!userRoles.Any(x => x.Contains("NAPOO")))
            {
                if (userRoles.Any(x => x == "EXTERNAL_EXPERTS" || x == "EXPERT_COMMITTEES"))
                {
                    this.hideTabs = true;
                }
            }
            else
            {
                this.hideTabs = false;
            }
        }
    }
}
