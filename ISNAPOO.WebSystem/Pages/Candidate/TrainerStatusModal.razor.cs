using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class TrainerStatusModal : BlazorBaseComponent
    {
        [Inject] public IApplicationUserService ApplicationUserService { get; set; }

        CandidateProviderTrainerVM candidateProviderTrainerVM;

        string TypeOfTrainer { get; set; }
        public async Task OpenModal(CandidateProviderTrainerVM candidateProviderTrainerVM, string typeOfTrainer)
        {
            this.candidateProviderTrainerVM = candidateProviderTrainerVM;
            this.candidateProviderTrainerVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateProviderTrainerVM.IdModifyUser);
            this.candidateProviderTrainerVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.candidateProviderTrainerVM.IdCreateUser);
            this.TypeOfTrainer = typeOfTrainer;
            this.isVisible = true;
            this.StateHasChanged();
            //IdStatus
        }
    }
}
