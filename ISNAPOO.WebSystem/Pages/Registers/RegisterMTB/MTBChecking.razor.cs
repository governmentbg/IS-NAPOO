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

namespace ISNAPOO.WebSystem.Pages.Registers.RegisterMTB
{
    public partial class MTBChecking : BlazorBaseComponent
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
        public IControlService ControlService { get; set; }

        [Inject] 
        public IApplicationUserService ApplicationUserService { get; set; }

        SfGrid<CandidateProviderPremisesCheckingVM> sfGrid = new SfGrid<CandidateProviderPremisesCheckingVM>();
        List<CandidateProviderPremisesCheckingVM> candidateProviderPremisesCheckingVMsource = new List<CandidateProviderPremisesCheckingVM>();
        FollowUpControlReadonlyModal followUpControlModal = new FollowUpControlReadonlyModal();
        private string PremisesName = "";
        private string LicensingType = "LicensingCPO";
        
        public async Task OpenModal(int IdCandidateProviderPremises, string? Name, int IdCandidateProvider)
        {
            this.PremisesName = Name;
            var temp = await this.CandidateProviderService.GetAllActiveCandidateProviderPremisesCheckingAsync(IdCandidateProviderPremises);
            this.candidateProviderPremisesCheckingVMsource = temp.Count > 0 ? temp : new List<CandidateProviderPremisesCheckingVM>();
            var idTypeLicense = (await this.CandidateProviderService.GetActiveCandidateProviderByIdAsync(IdCandidateProvider)).IdTypeLicense;
            this.LicensingType = (await this.DataSourceService.GetKeyValuesByKeyTypeIntCodeAsync("LicensingType")).FirstOrDefault(x => x.IdKeyValue == idTypeLicense).Name;
            this.isVisible = true;
            this.StateHasChanged();
        }
        private async Task<string> GetPersonName(int PersonId)
        {
            return await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(PersonId);
        }

        private async Task OpenFollowUpControl(CandidateProviderPremisesCheckingVM candidateProviderPremisesChecking)
        {
            var model = await this.ControlService.GetControlByIdFollowUpControlAsync(candidateProviderPremisesChecking.IdFollowUpControl.Value);
            this.followUpControlModal.OpenModal(model);
        }
    }
}
