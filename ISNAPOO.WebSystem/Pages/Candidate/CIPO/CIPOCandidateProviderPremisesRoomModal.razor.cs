using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Common.Framework;

namespace ISNAPOO.WebSystem.Pages.Candidate.CIPO
{
    public partial class CIPOCandidateProviderPremisesRoomModal : BlazorBaseComponent
    {
        private SfDialog sfDialog = new SfDialog();

        private CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM = new CandidateProviderPremisesRoomVM();
        private IEnumerable<KeyValueVM> kvPremisesTypeSource = new List<KeyValueVM>();
        private string mtbName = string.Empty;

        [Parameter]
        public EventCallback<CandidateProviderPremisesRoomVM> CallbackAfterModalSubmit { get; set; }

        [Parameter]
        public bool DisableFieldsWhenUserIsExternalExpertOrCommittee { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateProviderPremisesRoomVM);
        }

        private async Task SubmitRoomHandler()
        {
            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.SpinnerShow();

                this.editContext = new EditContext(this.candidateProviderPremisesRoomVM);
                this.editContext.EnableDataAnnotationsValidation();

                this.candidateProviderPremisesRoomVM.IdUsage = 1;

                if (this.editContext.Validate())
                {
                    this.candidateProviderPremisesRoomVM.IdUsage = 0;
                    ResultContext<CandidateProviderPremisesRoomVM> resultContext = new ResultContext<CandidateProviderPremisesRoomVM>();

                    if (this.candidateProviderPremisesRoomVM.IdCandidateProviderPremisesRoom != 0)
                    {
                        resultContext = await this.CandidateProviderService.UpdateCandidateProviderPremisesRoomAsync(this.candidateProviderPremisesRoomVM);
                    }
                    else
                    {
                        resultContext = await this.CandidateProviderService.CreateCandidateProviderPremisesRoomAsync(this.candidateProviderPremisesRoomVM);
                    }

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    }

                    await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderPremisesRoomVM);
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        public async Task OpenModal(CandidateProviderPremisesRoomVM candidateProviderPremisesRoomVM, IEnumerable<KeyValueVM> kvTrainingTypeSource, IEnumerable<KeyValueVM> kvPremisesTypeSource, string mtbName)
        {
            this.editContext = new EditContext(this.candidateProviderPremisesRoomVM);

            this.mtbName = mtbName;

            if (candidateProviderPremisesRoomVM.IdCandidateProviderPremisesRoom != 0)
            {
                this.candidateProviderPremisesRoomVM = await this.CandidateProviderService.GetCandidateProviderPremisesRoomByIdAsync(candidateProviderPremisesRoomVM);
            }
            else
            {
                this.candidateProviderPremisesRoomVM = new CandidateProviderPremisesRoomVM() { IdCandidateProviderPremises = candidateProviderPremisesRoomVM.IdCandidateProviderPremises };
            }

            this.kvPremisesTypeSource = kvPremisesTypeSource;

            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
