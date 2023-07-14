using Data.Models.Data.Candidate;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;
using Syncfusion.XlsIO.FormatParser.FormatTokens;

namespace ISNAPOO.WebSystem.Pages.Training.Consulting
{
    public partial class ConsultingListFilterModal : BlazorBaseComponent
    {

        private IEnumerable<ConsultingVM> consultingsSource = new List<ConsultingVM>();

        private bool isEmployedPersonFalse = false;
        private bool isEmployedPersonTrue = false;
        private bool isStudentFalse = false;
        private bool isStudentTrue = false;

        private IEnumerable<KeyValueVM> kvAimAtCIPOServicesType = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvAssingSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvConsultingReceiveTypeSource = new List<KeyValueVM>();
        private List<KeyValueVM> kvConsultingTypeSource = new();
        private IEnumerable<KeyValueVM> kvIndentTypeSource = new List<KeyValueVM>();
        private IEnumerable<KeyValueVM> kvRegistrationAtLabourOfficeType = new List<KeyValueVM>();
        public IEnumerable<ConsultingClientVM> consultingClientsSource { get; set; }

 

        [Parameter]
        public EventCallback<ConsultingClientVM> CallbackAfterSubmit { get; set; }
        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }
        [Parameter]
        public ConsultingClientVM ConsultingClientVM { get; set; }


        [Inject]
        public IDataSourceService DataSourceService { get; set; }
        [Inject]
        public ITrainingService TrainingService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            ConsultingClientVM = new ConsultingClientVM();
            this.editContext = new EditContext(ConsultingClientVM);
            FormTitle = "Данни за консултирано лице";

            this.kvConsultingReceiveTypeSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingReceiveType");
            this.kvRegistrationAtLabourOfficeType = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("RegistrationAtLabourOfficeType");
            this.kvAimAtCIPOServicesType = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AimAtCIPOServicesType");
            this.kvIndentTypeSource = await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("IndentType");
            this.kvAssingSource = (await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("AssignType")).Where(x => x.DefaultValue1 is "CIPO").ToList();
            this.consultingClientsSource = await TrainingService.GetAllConsultingClientsByIdCandidateProviderAsync(UserProps.IdCandidateProvider);
            this.kvConsultingTypeSource = (await DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("ConsultingType")).Where(x => this.consultingClientsSource.Any(y => y.Consultings.Any(c => c.IdConsultingType == x.IdKeyValue))).ToList();
            await LoadConsultingsDataAsync();

            SetIsStudentValue();
            SetIsEmployedPersonValue();
        }

        public async Task ClearFilter()
        {
            ConsultingClientVM = new ConsultingClientVM();

            await CallbackAfterSubmit.InvokeAsync(ConsultingClientVM);
        }

        public async Task Filter()
        {
            if (this.isStudentTrue)
            {
                ConsultingClientVM.IsStudent = true;
            }
            else if (this.isStudentFalse)
            {
                ConsultingClientVM.IsStudent = false;
            }
            else
            {
                ConsultingClientVM.IsStudent = null;
            }

            if (this.isEmployedPersonTrue)
            {
                ConsultingClientVM.IsEmployedPerson = true;
                ConsultingClientVM.IdRegistrationAtLabourOfficeType = null;
            }
            else if (this.isEmployedPersonFalse)
            {
                ConsultingClientVM.IsEmployedPerson = false;
            }
            else
            {
                ConsultingClientVM.IsEmployedPerson = null;
            }

            this.editContext = new EditContext(ConsultingClientVM);
 

            this.isVisible = false;
            await CallbackAfterSubmit.InvokeAsync(ConsultingClientVM);

            StateHasChanged();
        }


        public void OpenModal()
        {
            this.isVisible = true;
            StateHasChanged();
        }


        private async Task LoadConsultingsDataAsync()
        {
            this.consultingsSource = await CandidateProviderService.GetAllConsultingsByIdConsultingClientAsync(ConsultingClientVM.IdConsultingClient);
        }

        private void OnIsEmployedPersonFalseValueChange(ChangeEventArgs args)
        {
            if (this.isEmployedPersonTrue)
            {
                this.isEmployedPersonTrue = false;
            }
        }

        private void OnIsEmployedPersonTrueValueChange(ChangeEventArgs args)
        {
            if (this.isEmployedPersonFalse)
            {
                this.isEmployedPersonFalse = false;
                ConsultingClientVM.IdRegistrationAtLabourOfficeType = null;
            }
        }

        private void OnStudentFalseValueChange(ChangeEventArgs args)
        {
            if (this.isStudentTrue)
            {
                this.isStudentTrue = false;
            }
        }

        private void OnStudentTrueValueChange(ChangeEventArgs args)
        {
            if (this.isStudentFalse)
            {
                this.isStudentFalse = false;
            }
        }

        private void SetIsEmployedPersonValue()
        {
            if (ConsultingClientVM.IdConsultingClient == 0)
            {
                ConsultingClientVM.IsEmployedPerson = null;
            }

            if (ConsultingClientVM.IsEmployedPerson is not null)
            {
                if (ConsultingClientVM.IsEmployedPerson.Value)
                {
                    this.isEmployedPersonTrue = true;
                }
                else
                {
                    this.isEmployedPersonFalse = true;
                }
            }
        }

        private void SetIsStudentValue()
        {
            if (ConsultingClientVM.IdConsultingClient == 0)
            {
                ConsultingClientVM.IsStudent = null;
            }

            if (ConsultingClientVM.IsStudent is not null)
            {
                if (ConsultingClientVM.IsStudent.Value)
                {
                    this.isStudentTrue = true;
                }
                else
                {
                    this.isStudentFalse = true;
                }
            }
        }

    }
}
