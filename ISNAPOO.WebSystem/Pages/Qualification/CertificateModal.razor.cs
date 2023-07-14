using System;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Training;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Qualification
{
    public partial class CertificateModal : BlazorBaseComponent
    {
        int selectedTab = 0;

        SfDialog sfDialog;

        CandidateProviderVM model = new CandidateProviderVM();

        ExpertsActivityModal expertsActivityModal;


        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
   


        private async Task SelectedEventHandler()
        {
            if (this.selectedTab == 0)
            {
               await this.expertsActivityModal.openModal(model);
            }
            
        }

        public async void openModal(CandidateProviderVM candidate)
        {
            this.isVisible = true;

            model = candidate;

            this.model.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdModifyUser);
            this.model.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.model.IdCreateUser);

            await this.expertsActivityModal.openModal(model);

            this.StateHasChanged();
        }
    }
}

