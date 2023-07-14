using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.WebSystem.Pages.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;
using ISNAPOO.Common.Framework;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class CandidateProviderTrainerCheckingModal : BlazorBaseComponent
    {
        SfDialog sfDialog = new SfDialog();
        ToastMsg toast = new ToastMsg();

        private CandidateProviderTrainerCheckingVM candidateProviderTrainerCheckingVM = new CandidateProviderTrainerCheckingVM();
        private List<CandidateProviderTrainerVM> candidateProviderTrainers = new List<CandidateProviderTrainerVM>();

        [Parameter]
        public EventCallback<CandidateProviderTrainerCheckingVM> CallbackAfterModalSubmit { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IProfessionService ProfessionService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public override bool IsContextModified { get => this.IsEditContextModified(); }

        protected override async Task OnInitializedAsync()
        {
            this.editContext = new EditContext(this.candidateProviderTrainerCheckingVM);
        }

        public async Task OpenModal(List<CandidateProviderTrainerVM> _candidateProviderTrainers, CandidateProviderTrainerCheckingVM _candidateProviderTrainerCheckingVM)
        {
            this.candidateProviderTrainerCheckingVM = _candidateProviderTrainerCheckingVM;
            this.editContext = new EditContext(this.candidateProviderTrainerCheckingVM);

            this.candidateProviderTrainers = _candidateProviderTrainers;



            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SubmitCheckingHandler()
        {

            bool hasPermission = await CheckUserActionPermission("ManageApplicationData", false);
            if (!hasPermission) { return; }

            this.editContext = new EditContext(this.candidateProviderTrainerCheckingVM);
            this.editContext.EnableDataAnnotationsValidation();

            this.SpinnerShow();

            if (this.editContext.Validate())
            {

                foreach (var trainer in this.candidateProviderTrainers.ToList())
                {

                    ResultContext<CandidateProviderTrainerCheckingVM> resultContext = new ResultContext<CandidateProviderTrainerCheckingVM>();

                    this.candidateProviderTrainerCheckingVM.IdCandidateProviderTrainer = trainer.IdCandidateProviderTrainer;

                    if (this.candidateProviderTrainerCheckingVM.CheckingDate == null)
                    {
                        this.candidateProviderTrainerCheckingVM.CheckingDate = DateTime.Today;
                    }

                    resultContext.ResultContextObject = this.candidateProviderTrainerCheckingVM;


                    resultContext = await this.CandidateProviderService.CreateCandidateProviderTrainerCheckingAsync(resultContext);

                    if (resultContext.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
                    }
                    else
                    {
                        this.editContext = new EditContext(this.candidateProviderTrainerCheckingVM);
                        await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));
                    }
                }

                this.isVisible = false;
                await this.CallbackAfterModalSubmit.InvokeAsync(this.candidateProviderTrainerCheckingVM);
            }

            this.SpinnerHide();
        }
    }



}
