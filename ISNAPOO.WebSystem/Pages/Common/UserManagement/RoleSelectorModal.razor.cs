using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Popups;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class RoleSelectorModal   : BlazorBaseComponent
    {
        [Parameter]
        public EventCallback<ResultContext<List<ApplicationRoleVM>>> CallbackAfterSelect { get; set; }

        private SfDialog sfDialog = new SfDialog();
        IEnumerable<ApplicationRoleVM> roles;
        SfGrid<ApplicationRoleVM> refRoleGrid;
        List<ApplicationRoleVM> selectedRoles = new List<ApplicationRoleVM>();

        [Inject]
        IApplicationUserService ApplicationUserService { get; set; }

        private async Task RoleDeselectedHandler(RowDeselectEventArgs<ApplicationRoleVM> args)
        {
            this.selectedRoles.Clear();
            this.selectedRoles = await this.refRoleGrid.GetSelectedRecordsAsync();
        }

        private async Task RoleSelectedHandler(RowSelectEventArgs<ApplicationRoleVM> args)
        {
            this.selectedRoles.Clear();
            this.selectedRoles = await this.refRoleGrid.GetSelectedRecordsAsync();
        }

        private async Task AddNewRole()
        {
            this.isVisible = false;
            ResultContext<List<ApplicationRoleVM>> resultContext = new ResultContext<List<ApplicationRoleVM>>();

            resultContext.ResultContextObject = this.selectedRoles;

            await this.CallbackAfterSelect.InvokeAsync(resultContext);
        }

        public async Task OpenModal(ApplicationUserVM applicationUserVM)
        {   
            roles = await ApplicationUserService.GetAllRolesExceptAsync(applicationUserVM);



            this.isVisible = true;
            this.StateHasChanged();
        }
    }
}
