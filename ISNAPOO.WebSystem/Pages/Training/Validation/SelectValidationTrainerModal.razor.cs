using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Training.Validation
{
    public partial class SelectValidationTrainerModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderTrainerVM> trainersGrid = new SfGrid<CandidateProviderTrainerVM>();

        private IEnumerable<KeyValueVM> trainingTypeSource = new List<KeyValueVM>();
        private List<CandidateProviderTrainerVM> trainersSource = new List<CandidateProviderTrainerVM>();
        private int idTrainingType = 0;
        private int kvTheoryAndPracticeId = 0;
        private ValidationClientVM clientVM = new ValidationClientVM();
        private List<ValidationTrainerVM> addedTrainersSource = new List<ValidationTrainerVM>();

        [Parameter]
        public EventCallback<Dictionary<int, List<CandidateProviderTrainerVM>>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        public async Task OpenModal(ValidationClientVM clientVM, List<ValidationTrainerVM> trainersSource)
        {
            this.clientVM = clientVM;
            this.addedTrainersSource = trainersSource.ToList();

            this.idTrainingType = 0;

            this.trainingTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            this.kvTheoryAndPracticeId = this.trainingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice").IdKeyValue;

            this.trainersSource.Clear();

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

                if (this.idTrainingType == 0)
                {
                    await this.ShowErrorAsync("Моля, изберете вид на провежданото обучение!");
                    return;
                }

                var selectedTrainers = await this.trainersGrid.GetSelectedRecordsAsync();
                if (!selectedTrainers.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне един преподавател от списъка!");
                    return;
                }

                this.isVisible = false;

                var dict = new Dictionary<int, List<CandidateProviderTrainerVM>>()
                {
                    {
                        this.idTrainingType, selectedTrainers
                    }
                };

                await this.CallbackAfterSubmit.InvokeAsync(dict);
            }
            finally
            {
                this.loading = false;
            }
        }

        private async Task TrainingTypeSelectedHandler(ChangeEventArgs<int, KeyValueVM> args)
        {
            this.SpinnerShow();

            if (args.Value != 0)
            {
                this.trainersSource = (await this.TrainingService.GetAllActiveCandidateProviderTrainersByIdTrainingTypeByIdSpecialityAndByIdCandidateProviderAsync(this.clientVM.IdCandidateProvider, this.clientVM.IdSpeciality.Value, this.idTrainingType, this.kvTheoryAndPracticeId)).ToList();
                if (this.addedTrainersSource.Any())
                {
                    foreach (var addedTrainer in this.addedTrainersSource)
                    {
                        this.trainersSource.RemoveAll(x => x.IdCandidateProviderTrainer == addedTrainer.IdTrainer);
                    }
                }
            }
            else
            {
                this.trainersSource = new List<CandidateProviderTrainerVM>();
                this.idTrainingType = 0;
            }

            await this.trainersGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerHide();
        }
    }
}
