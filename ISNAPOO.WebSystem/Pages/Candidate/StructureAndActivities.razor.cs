using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class StructureAndActivities : BlazorBaseComponent
    {
        private CandidateProviderCPOStructureActivityVM candidateProviderCPOStructureActivityVM = new CandidateProviderCPOStructureActivityVM();
        private CandidateProviderCIPOStructureActivityVM candidateProviderCIPOStructureActivityVM = new CandidateProviderCIPOStructureActivityVM();

        [Parameter]
        public CandidateProviderVM CandidateProviderVM { get; set; }

        [Parameter]
        public bool IsCPO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenOpenFromProfile { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Parameter]
        public bool DisableFieldsWhenApplicationStatusIsNotDocPreparation { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsNAPOO { get; set; }

        [Parameter]
        public bool DisableFieldsWhenActiveLicenceChange { get; set; }

        [Parameter]
        public bool IsUserProfileAdministrator { get; set; }

        [Parameter]
        public bool IsLicenceChange { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = this.IsCPO
                ? new EditContext(this.candidateProviderCPOStructureActivityVM)
                : this.editContext = new EditContext(this.candidateProviderCIPOStructureActivityVM);

            this.FormTitle = "Устройство и дейност";

            this.editContext.MarkAsUnmodified();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                this.SpinnerShow();

                if (this.IsCPO)
                {
                    this.candidateProviderCPOStructureActivityVM = await this.CandidateProviderService.GetCandidateProviderCPOStructureActivityByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
                    this.candidateProviderCPOStructureActivityVM = this.candidateProviderCPOStructureActivityVM == null ? new CandidateProviderCPOStructureActivityVM() { IdCandidate_Provider = this.CandidateProviderVM.IdCandidate_Provider } : this.candidateProviderCPOStructureActivityVM;
                }
                else
                {
                    this.candidateProviderCIPOStructureActivityVM = await this.CandidateProviderService.GetCandidateProviderCIPOStructureActivityByIdCandidateProviderAsync(this.CandidateProviderVM.IdCandidate_Provider);
                    this.candidateProviderCIPOStructureActivityVM = this.candidateProviderCIPOStructureActivityVM == null ? new CandidateProviderCIPOStructureActivityVM() { IdCandidate_Provider = this.CandidateProviderVM.IdCandidate_Provider } : this.candidateProviderCIPOStructureActivityVM;
                }

                this.editContext.MarkAsUnmodified();

                this.StateHasChanged();

                this.SpinnerHide();
            }
        }

        public new async Task SubmitHandler()
        {
            this.editContext = new EditContext(this.candidateProviderCPOStructureActivityVM);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                if (this.IsCPO)
                {
                    var inputContext = new ResultContext<CandidateProviderCPOStructureActivityVM>();
                    inputContext.ResultContextObject = this.candidateProviderCPOStructureActivityVM;

                    var resultContext = new ResultContext<CandidateProviderCPOStructureActivityVM>();

                    if (this.candidateProviderCPOStructureActivityVM.IdCandidateProviderCPOStructureActivity == 0)
                    {
                        if (!this.candidateProviderCPOStructureActivityVM.IsEmpty())
                        {
                            resultContext = await this.CandidateProviderService.CreateCandidateProviderCPOStructureActivityAsync(inputContext);
                        }
                    }
                    else
                    {
                        resultContext = await this.CandidateProviderService.UpdateCandidateProviderCPOStructureActivityAsync(inputContext);
                    }
                }
                else
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
}
