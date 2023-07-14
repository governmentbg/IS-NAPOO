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
    public partial class SelectValidationPremisesModal : BlazorBaseComponent
    {
        private SfGrid<CandidateProviderPremisesVM> premisesGrid = new SfGrid<CandidateProviderPremisesVM>();

        private IEnumerable<KeyValueVM> trainingTypeSource = new List<KeyValueVM>();
        private int kvTheoryAndPracticeId = 0;
        private List<CandidateProviderPremisesVM> premisesSource = new List<CandidateProviderPremisesVM>();
        private int idTrainingType = 0;
        private ValidationClientVM clientVM = new ValidationClientVM();
        private List<ValidationPremisesVM> addedPremisesSource = new List<ValidationPremisesVM>();

        [Parameter]
        public EventCallback<Dictionary<int, List<CandidateProviderPremisesVM>>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ITrainingService TrainingService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        public async Task OpenModal(ValidationClientVM clientVM, List<ValidationPremisesVM> addedPremisesSource)
        {
            this.clientVM = clientVM;
            this.addedPremisesSource = addedPremisesSource.ToList();

            this.idTrainingType = 0;

            this.trainingTypeSource = await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingType");
            this.kvTheoryAndPracticeId = this.trainingTypeSource.FirstOrDefault(x => x.KeyValueIntCode == "TrainingInTheoryAndPractice").IdKeyValue;

            this.premisesSource.Clear();

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

                var selectedPremises = await this.premisesGrid.GetSelectedRecordsAsync();
                if (!selectedPremises.Any())
                {
                    await this.ShowErrorAsync("Моля, изберете поне една материално-техническа база от списъка!");
                    return;
                }

                this.isVisible = false;

                var dict = new Dictionary<int, List<CandidateProviderPremisesVM>>()
                {
                    {
                        this.idTrainingType, selectedPremises
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
                this.premisesSource = (await this.TrainingService.GetAllActiveCandidateProviderPremisesByIdTrainingTypeByIdSpecialityAndByIdCandidateProviderAsync(this.clientVM.IdCandidateProvider, this.clientVM.IdSpeciality.Value, this.idTrainingType, this.kvTheoryAndPracticeId)).ToList();
                if (this.addedPremisesSource.Any())
                {
                    foreach (var addedPremises in this.addedPremisesSource)
                    {
                        this.premisesSource.RemoveAll(x => x.IdCandidateProviderPremises == addedPremises.IdPremises);
                    }
                }
            }
            else
            {
                this.premisesSource = new List<CandidateProviderPremisesVM>();
                this.idTrainingType = 0;
            }

            await this.premisesGrid.Refresh();
            this.StateHasChanged();

            this.SpinnerHide();
        }
    }
}
