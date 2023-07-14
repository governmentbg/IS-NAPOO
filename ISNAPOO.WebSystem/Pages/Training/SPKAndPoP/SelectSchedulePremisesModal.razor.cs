using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SelectSchedulePremisesModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderPremisesVM> premisesGrid = new SfGrid<CandidateProviderPremisesVM>();

        private List<int> scheduleIdList = new List<int>();
        private List<CandidateProviderPremisesVM> premisesSource = new List<CandidateProviderPremisesVM>();

        [Parameter]
        public EventCallback<ResultContext<NoResult>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        public void OpenModal(List<int> scheduleIdList, List<CandidateProviderPremisesVM> premisesSource)
        {
            this.scheduleIdList = scheduleIdList.ToList();
            this.premisesSource = premisesSource.ToList();

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task RowSelectingHandler()
        {
            var selectedRows = await this.premisesGrid.GetSelectedRecordsAsync();
            if (selectedRows.Any())
            {
                await this.premisesGrid.ClearRowSelectionAsync();
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

                var selectedRows = await this.premisesGrid.GetSelectedRecordsAsync();
                if (!selectedRows.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете МТБ!");
                    this.SpinnerHide();
                    return;
                }

                var selectedPremisesId = selectedRows.FirstOrDefault()!.IdCandidateProviderPremises;
                var result = await this.TrainingService.AddPremisesToListCourseSchedulesAsync(selectedPremisesId, this.scheduleIdList);

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
