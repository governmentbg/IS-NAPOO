using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Framework;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    
    public partial class CandidateProviderPremisesCheckingModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        ToastMsg toast = new ToastMsg();

        private CandidateProviderPremisesCheckingVM candidateProviderPremisesCheckingVM = new CandidateProviderPremisesCheckingVM();
        private List<CandidateProviderPremisesVM> candidateProviderPremisess = new List<CandidateProviderPremisesVM>();

        [Parameter]
        public EventCallback<CandidateProviderPremisesCheckingVM> CallbackAfterModalSubmit { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public override bool IsContextModified { get => this.IsEditContextModified(); }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateProviderPremisesCheckingVM);
        }

        public async Task OpenModal(List<CandidateProviderPremisesVM> _candidateProviderPremisess)
        {
              this.editContext = new EditContext(this.candidateProviderPremisesCheckingVM);
            this.candidateProviderPremisesCheckingVM = new CandidateProviderPremisesCheckingVM();
            this.candidateProviderPremisess = _candidateProviderPremisess;

            this.candidateProviderPremisesCheckingVM.CheckingDate = DateTime.Today;


            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SubmitCheckingHandler()
        {

            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.editContext = new EditContext(this.candidateProviderPremisesCheckingVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.SpinnerShow();

            if (this.editContext.Validate())
            {

                foreach (var premises in this.candidateProviderPremisess.ToList())
                {

                    ResultContext<CandidateProviderPremisesCheckingVM> resultContext = new ResultContext<CandidateProviderPremisesCheckingVM>();

                    this.candidateProviderPremisesCheckingVM.IdCandidateProviderPremises = premises.IdCandidateProviderPremises;

                    resultContext.ResultContextObject = this.candidateProviderPremisesCheckingVM;

                    resultContext = await this.CandidateProviderService.CreateCandidateProviderPremisesCheckingAsync(resultContext);

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        this.editContext = new EditContext(this.candidateProviderPremisesCheckingVM);
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    }
                }

                this.isVisible = false;
                await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderPremisesCheckingVM);
            }

            this.SpinnerHide();
        }
    }
}
