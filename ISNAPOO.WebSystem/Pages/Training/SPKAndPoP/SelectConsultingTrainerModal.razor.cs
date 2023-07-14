using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SelectConsultingTrainerModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderTrainerVM> trainersGrid = new SfGrid<CandidateProviderTrainerVM>();

        private List<CandidateProviderTrainerVM> trainersSource = new List<CandidateProviderTrainerVM>();
        private ConsultingClientVM consultingClientVM = new ConsultingClientVM();
        private List<ConsultingTrainerVM> addedTrainersSource = new List<ConsultingTrainerVM>();

        [Parameter]
        public EventCallback<List<CandidateProviderTrainerVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        public async Task OpenModal(ConsultingClientVM consultingClientVM, List<ConsultingTrainerVM> trainersSource)
        {
            this.consultingClientVM = consultingClientVM;
            this.addedTrainersSource = trainersSource.ToList();

            this.trainersSource.Clear();

            await this.LoadConsultantTrainersDataAsync();

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

                var selectedTrainers = await this.trainersGrid.GetSelectedRecordsAsync();
                if (!selectedTrainers.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един консултант от списъка!");
                    return;
                }

                this.isVisible = false;

                await this.CallbackAfterSubmit.InvokeAsync(selectedTrainers);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task LoadConsultantTrainersDataAsync()
        {
            this.SpinnerShow();

            this.trainersSource = (await this.CandidateProviderService.GetAllActiveTrainersByIdCandidateProviderAsync(this.consultingClientVM.IdCandidateProvider)).ToList();
            if (this.addedTrainersSource.Any())
            {
                foreach (var addedTrainer in this.addedTrainersSource)
                {
                    this.trainersSource.RemoveAll(x => x.IdCandidateProviderTrainer == addedTrainer.IdTrainer);
                }
            }

            this.StateHasChanged();

            this.SpinnerHide();
        }
    }
}
