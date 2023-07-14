using System.Security.Claims;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Navigations;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Pages.Common.ManagingTimelines.TimelinesManagementList;

namespace ISNAPOO.WebSystem.Pages.Common.ManagingTimelines
{

    public partial class TimelinesManagementModal : BlazorBaseComponent
    {
        private ProcedureTimeline applicationRoleVM = new ProcedureTimeline();

        [Parameter]
        public EventCallback<ResultContext<ProcedureTimeline>> CallbackAfterSubmit { get; set; }

        [Inject]
        IPolicyService PolicyService { get; set; }

        private SfDialog sfDialog = new SfDialog();
        List<string> validationMessages = new List<string>();
        int selectedTab = 0;
        ToastMsg toast;
        SfGrid<RoleClaim> refGrid;
        IEnumerable<RoleClaim> roleClaims;
        IEnumerable<PolicyVM> policies;

       // private PolicySelectorModal policySelectorModal = new PolicySelectorModal();

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.applicationRoleVM);

        }
        public async Task OpenModal(ProcedureTimeline _applicationRoleVM)
        {
            //policies = await PolicyService.GetAllPolicyAsync(new PolicyVM());

            //this.applicationRoleVM = _applicationRoleVM;
    


            //if (applicationRoleVM.ID != null)
            //{
               

            //    roleClaims = this.applicationRoleVM.RoleClaims;
            //    foreach (var claim in roleClaims)
            //    {
            //        claim.Value = policies.Where(p => p.PolicyCode == claim.Type).FirstOrDefault().PolicyDescription;
            //    }
            //}

            //this.applicationRoleVM.ModifyPersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.applicationRoleVM.IdModifyUser);
            //this.applicationRoleVM.CreatePersonName = await this.ApplicationUserService.GetApplicationUsersPersonNameAsync(this.applicationRoleVM.IdCreateUser);
            //roleClaims = roleClaims.OrderBy(b => b.Value).ToList();

            //this.isVisible = true;
            //this.StateHasChanged();
        }

        public async Task RemovePolicy()
        {
           // bool hasPermission = await CheckUserActionPermission("ManageRolesData", false);
           // if (!hasPermission) { return; }

           // this.applicationRoleVM.RoleClaimsForRemove = await this.refGrid.GetSelectedRecordsAsync();

           // ResultContext<ProcedureTimeline> resultContext = new ResultContext<ProcedureTimeline>();

           // //resultContext = await ApplicationUserService.RemoveRoleClaims(resultContext);

           // //if (applicationRoleVM.Id != null)
           // //{
           // //    this.applicationRoleVM = await ApplicationUserService.GetApplicationRoleByIdAsync(this.applicationRoleVM);

           // //    roleClaims = this.applicationRoleVM.RoleClaims;
           // //    foreach (var claim in roleClaims)
           // //    {
           // //        claim.Value = policies.Where(p => p.PolicyCode == claim.Type).FirstOrDefault().PolicyDescription;
           // //    }
           // //    refGrid.Refresh();
           // //}

           // this.applicationRoleVM.RoleClaims = roleClaims.Where(x => !applicationRoleVM.RoleClaimsForRemove.Contains(x)).ToList();

           // roleClaims = this.applicationRoleVM.RoleClaims;

           // resultContext.ResultContextObject = this.applicationRoleVM;

           //await refGrid.Refresh();

           // this.StateHasChanged();
        }



        private void Select(SelectingEventArgs args)
        {
            //if (args.IsSwiped)
            //{
            //    args.Cancel = true;
            //}
        }

        private async Task SubmitRoleHandler()
        {
            //bool hasPermission = await CheckUserActionPermission("ManageRolesData", false);
            //if (!hasPermission) { return; }
            //this.SpinnerShow();

            //ResultContext<ProcedureTimeline> resultContext = new ResultContext<ProcedureTimeline>();


            //if (string.IsNullOrEmpty(this.applicationRoleVM.Id))
            //{
            //    resultContext = await this.ApplicationUserService.CreateApplicationRoleAsync(this.applicationRoleVM);
            //}
            //else
            //{
            //    resultContext = await this.ApplicationUserService.UpdateApplicationRoleAsync(this.applicationRoleVM);

            //}

            //if (resultContext.HasErrorMessages)
            //{
            //    await this.ShowErrorAsync(string.Join(Environment.NewLine, resultContext.ListErrorMessages));
            //}
            //else
            //{
            //    this.editContext = new EditContext(this.applicationRoleVM);
            //    await this.ShowSuccessAsync(string.Join(Environment.NewLine, resultContext.ListMessages));

            //}

            //await this.CallbackAfterSubmit.InvokeAsync(resultContext);

        }

        private async Task OpenPolicy()
        {
            //bool hasPermission = await CheckUserActionPermission("ManageRolesData", false);
            //if (!hasPermission) { return; }

            //this.policySelectorModal.OpenModal(this.applicationRoleVM);
        }

        private async Task GetSelectedPolicyVM(ResultContext<List<PolicyVM>> resultContext)
        {

            //foreach (var policy in resultContext.ResultContextObject)
            //{
            //    this.applicationRoleVM.RoleClaims.Add(new RoleClaim() { Type = policy.PolicyCode, Value = policy.PolicyDescription });
            //}


            //roleClaims = this.applicationRoleVM.RoleClaims;
            //refGrid.Refresh();

        }

    }
}
