using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SelectScheduleTrainerModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderTrainerVM> trainersGrid = new SfGrid<CandidateProviderTrainerVM>();

        private List<int> scheduleIdList = new List<int>();
        private List<CandidateProviderTrainerVM> trainersSource = new List<CandidateProviderTrainerVM>();

        [Parameter]
        public EventCallback<ResultContext<NoResult>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public void OpenModal(List<int> scheduleIdList, List<CandidateProviderTrainerVM> trainersSource)
        {
            this.scheduleIdList = scheduleIdList.ToList();
            this.trainersSource = trainersSource.ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task RowSelectingHandler()
        {
            var selectedRows = await this.trainersGrid.GetSelectedRecordsAsync();
            if (selectedRows.Any())
            {
                await this.trainersGrid.ClearRowSelectionAsync();
            }
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                var selectedRows = await this.trainersGrid.GetSelectedRecordsAsync();
                if (!selectedRows.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете преподавател!");
                    this.SpinnerHide();
                    return;
                }

                var selectedTrainerId = selectedRows.FirstOrDefault()!.IdCandidateProviderTrainer;
                var result = await this.TrainingService.AddTrainerToListCourseSchedulesAsync(selectedTrainerId, this.scheduleIdList);

                this.isVisible = false;
                await this.CallbackAfterSubmit.InvokeAsync(result);
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
