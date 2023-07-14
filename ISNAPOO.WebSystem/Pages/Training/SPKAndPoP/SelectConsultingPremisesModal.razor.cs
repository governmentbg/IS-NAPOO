using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SelectConsultingPremisesModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderPremisesVM> premisesGrid = new SfGrid<CandidateProviderPremisesVM>();

        private List<CandidateProviderPremisesVM> premisesSource = new List<CandidateProviderPremisesVM>();
        private ConsultingClientVM consultingClientVM = new ConsultingClientVM();
        private List<ConsultingPremisesVM> addedPremisesSource = new List<ConsultingPremisesVM>();

        [Parameter]
        public EventCallback<List<CandidateProviderPremisesVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public async Task OpenModal(ConsultingClientVM consultingClientVM, List<ConsultingPremisesVM> addedPremisesSource)
        {
            this.consultingClientVM = consultingClientVM;
            this.addedPremisesSource = addedPremisesSource.ToList();

            this.premisesSource.Clear();

            await this.LoadPremisesDataAsync();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SubmitBtn()
        {
            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedPremises = await this.premisesGrid.GetSelectedRecordsAsync();
                if (!selectedPremises.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне една материално-техническа база от списъка!");
                    return;
                }

                this.isVisible = false;

                await this.CallbackAfterSubmit.InvokeAsync(selectedPremises.ToList());
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task LoadPremisesDataAsync()
        {
            this.SpinnerShow();

            this.premisesSource = (await this.CandidateProviderService.GetAllActivePremisesByIdCandidateProviderAsync(this.consultingClientVM.IdCandidateProvider)).ToList();
            if (this.addedPremisesSource.Any())
            {
                foreach (var addedPremises in this.addedPremisesSource)
                {
                    this.premisesSource.RemoveAll(x => x.IdCandidateProviderPremises == addedPremises.IdPremises);
                }
            }

            this.StateHasChanged();

            this.SpinnerHide();
        }
    }
}
