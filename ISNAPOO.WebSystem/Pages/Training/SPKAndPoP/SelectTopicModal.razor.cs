using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.SPKAndPoP
{
    public partial class SelectTopicModal : BlazorBaseComponent
    {
        private SfGrid<TrainingCurriculumVM> curriculumsGrid = new SfGrid<TrainingCurriculumVM>();

        private string trainingType = string.Empty;
        private List<TrainingCurriculumVM> curriculumSource = new List<TrainingCurriculumVM>();
        private CourseScheduleVM model = new CourseScheduleVM();

        [Parameter]
        public EventCallback<TrainingCurriculumVM> CallbackAfterSubmit { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        public async Task OpenModal(List<TrainingCurriculumVM> curriculums, string scheduleType, CourseScheduleVM courseScheduleVM)
        {
            this.curriculumSource = curriculums.ToList();
            this.trainingType = scheduleType;
            this.model = courseScheduleVM;

            this.isVisible = true;
            this.StateHasChanged();

            await this.HandleRowSelectionAsync();
        }

        private async Task RowSelectingHandler()
        {
            var selectedRows = await this.curriculumsGrid.GetSelectedRecordsAsync();
            if (selectedRows.Any())
            {
                await this.curriculumsGrid.ClearRowSelectionAsync();
            }
        }

        private async Task SubmitBtn()
        {
            this.SpinnerShow();

            var selectedCurr = await this.curriculumsGrid.GetSelectedRecordsAsync();
            if (!selectedCurr.Any())
            {
                await this.ShowErrorAsync("Моля, изберете тема!");
                this.SpinnerHide();
                return;
            }

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.isVisible = false;

                await this.CallbackAfterSubmit.InvokeAsync(selectedCurr.FirstOrDefault());
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }   

        private async Task HandleRowSelectionAsync()
        {
            if (this.model.IdTrainingCurriculum != 0)
            {
                var idx = await this.curriculumsGrid.GetRowIndexByPrimaryKeyAsync(this.model.IdTrainingCurriculum);
                if (idx != -1)
                {
                    await this.curriculumsGrid.SelectRowAsync(idx);
                }
            }
        }
    }
}
