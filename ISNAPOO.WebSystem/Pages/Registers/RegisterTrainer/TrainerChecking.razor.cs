using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Control;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Control;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Syncfusion.Blazor.Grids;

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterTrainer
{
    public partial class TrainerChecking : BlazorBaseComponent
    {
        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public IJSRuntime JsRuntime { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject] 
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject] 
        public IControlService ControlService { get; set; }

        [Parameter] 
        public string LicensingType { get; set; }

        SfGrid<CandidateProviderTrainerCheckingVM> sfGrid = new SfGrid<CandidateProviderTrainerCheckingVM>();
        List<CandidateProviderTrainerCheckingVM> candidateProviderTrainerCheckingVMsource = new List<CandidateProviderTrainerCheckingVM>();
        FollowUpControlReadonlyModal followUpControlModal = new FollowUpControlReadonlyModal();
        private string TrainerName = "";
        private string Header = "";
        public async Task OpenModal(int IdCandidateProviderPremises, string? name)
        {
            this.TrainerName = name;
            var temp = await this.CandidateProviderService.GetAllActiveCandidateProviderTrainerCheckingAsync(IdCandidateProviderPremises);
            this.candidateProviderTrainerCheckingVMsource = temp.Count > 0 ? temp : new List<CandidateProviderTrainerCheckingVM>();
            if(LicensingType == "LicensingCPO")
            {
                this.Header = "Преподавател";
            }
            else
            {
                this.Header = "Консултант";
            }
            this.isVisible = true;
            this.StateHasChanged();
        }
        private async Task<string> GetPersonName(int PersonId)
        {
            return await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(PersonId);
        }
        private async Task OpenFollowUpControl(CandidateProviderTrainerCheckingVM courseChecking)
        {
            var model = await this.ControlService.GetControlByIdFollowUpControlAsync(courseChecking.IdFollowUpControl.Value);
            this.LicensingType = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType")).FirstOrDefault(x => x.IdKeyValue == model.CandidateProvider.IdTypeLicense).KeyValueIntCode;
            this.followUpControlModal.OpenModal(model);
        }
    }
}
