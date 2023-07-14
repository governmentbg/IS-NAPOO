using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.Core.ViewModels.Register;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.DropDowns;
using ISNAPOO.Core.Contracts.Candidate;
using Syncfusion.Blazor.Data;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;

namespace ISNAPOO.WebSystem.Pages.Registers.State_Examination
{
    public partial class StateExaminationInfoFilterList : BlazorBaseComponent
    {
    
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();
        private StateExaminationInfoFilterListVM model = new StateExaminationInfoFilterListVM();
        private List<CandidateProviderVM> candidateProvidersSource = new List<CandidateProviderVM>();
        private List<KeyValueVM> kvTrainingScheduleType = new List<KeyValueVM>();
        private KeyValueVM kvTheory = new KeyValueVM();

        [Parameter]
        public EventCallback<List<ClientCourseVM>> CallbackAfterSubmit { get; set; }

        [Parameter]
        public EventCallback<StateExaminationInfoFilterListVM> CallbackAfterSubmitStateExamInfoList { get; set; }

        [Inject]
        public ICandidateProviderService candidateProviderService { get; set; }

        [Inject]
        public IDataSourceService dataSourceService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal()
        {

            this.editContext = new EditContext(this.model);

            if (!this.candidateProvidersSource.Any())
            {
                this.candidateProvidersSource = (await this.candidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO")).ToList();
            }

            this.kvTrainingScheduleType = (await dataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("TrainingScheduleType")).ToList();           
            this.kvTheory = this.kvTrainingScheduleType.FirstOrDefault(x => x.KeyValueIntCode == "Theory");
            this.isVisible = true;
            this.StateHasChanged();
        }
        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ProviderName", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.candidateProvidersSource, query);
        }
        public void ClearFilter()
        {
            this.model = new StateExaminationInfoFilterListVM();
        }
        public async Task Save()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                if (!this.model.IdCandidateProvider.HasValue && string.IsNullOrEmpty(this.model.LicenceNumber)
                   && string.IsNullOrEmpty(this.model.CourseName) && string.IsNullOrEmpty(this.model.TrainingTypeIntCode)
                   && !this.model.ExamDateFrom.HasValue && !this.model.ExamDateTo.HasValue)
                {
                    await this.ShowErrorAsync("Моля, изберете поне един критерий, по който да филтрирате данни за обучени лица!");
                    this.loading = false;
                    this.SpinnerHide();
                    return;
                }
                    await this.CallbackAfterSubmitStateExamInfoList.InvokeAsync(this.model);

                this.isVisible = false;
                this.StateHasChanged();
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }
    }
}
