using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class MTBStatusModal : BlazorBaseComponent
    {
        [Inject] public IApplicationUserService ApplicationUserService { get; set; }

        CandidateProviderPremisesVM candidateProviderPremisesVM;
        public async void OpenModal(CandidateProviderPremisesVM candidateProviderPremisesVM)
        {
            this.candidateProviderPremisesVM = candidateProviderPremisesVM;
            this.candidateProviderPremisesVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateProviderPremisesVM.IdModifyUser);
            this.candidateProviderPremisesVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateProviderPremisesVM.IdCreateUser);
            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
