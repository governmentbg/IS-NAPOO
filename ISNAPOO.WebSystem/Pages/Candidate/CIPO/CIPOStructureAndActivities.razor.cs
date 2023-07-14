using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOStructureAndActivities : BlazorBaseComponent
    {
        private CandidateProviderCIPOStructureActivityVM candidateProviderCIPOStructureActivityVM = new CandidateProviderCIPOStructureActivityVM();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool DisableAllFields { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.candidateProviderCIPOStructureActivityVM);
            this.FormTitle = "Устройство и дейност";
        }

        protected override async Task OnInitializedAsync()
        {
            this.SpinnerShow();

            this.candidateProviderCIPOStructureActivityVM = await this.CandidateProviderService.GetCandidateProviderCIPOStructureActivityByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
            this.candidateProviderCIPOStructureActivityVM = this.candidateProviderCIPOStructureActivityVM == null ? new CandidateProviderCIPOStructureActivityVM() { IdCandidate_Provider = this.CandidateProviderVM.IdCandidate_Provider } : this.candidateProviderCIPOStructureActivityVM;

            this.SpinnerHide();
        }

        public new async Task SubmitHandler()
        {
            this.editContext = new EditContext(this.candidateProviderCIPOStructureActivityVM);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                var inputContext = new ResultContext<CandidateProviderCIPOStructureActivityVM>();
                inputContext.ResultContextObject = this.candidateProviderCIPOStructureActivityVM;

                var resultContext = new ResultContext<CandidateProviderCIPOStructureActivityVM>();

                if (this.candidateProviderCIPOStructureActivityVM.IdCandidateProviderCIPOStructureActivity == 0)
                {
                    if (!this.candidateProviderCIPOStructureActivityVM.IsEmpty())
                    {
                        resultContext = await this.CandidateProviderService.CreateCandidateProviderCIPOStructureActivityAsync(inputContext);
                    }
                }
                else
                {
                    resultContext = await this.CandidateProviderService.UpdateCandidateProviderCIPOStructureActivityAsync(inputContext);
                }
            }
        }
    }
}
