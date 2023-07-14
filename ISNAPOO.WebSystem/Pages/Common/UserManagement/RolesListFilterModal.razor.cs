using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.EKATTE;
using ISNAPOO.Core.Services.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class RolesListFilterModal : BlazorBaseComponent
    {
        private List<ApplicationRoleVM> roles = new List<ApplicationRoleVM>();
        private SfAutoComplete<string, RoleClaim> sfAutoCompleteRoleClaimType = new SfAutoComplete<string, RoleClaim>();
        private SfAutoComplete<string, RoleClaim> sfAutoCompleteRoleClaimValue = new SfAutoComplete<string, RoleClaim>();

        private List<PolicyVM> policies;
        private List<RoleClaim> roleClaims = new List<RoleClaim>();

        private RoleClaim roleClaim = new RoleClaim();
        ValidationMessageStore? messageStore;

        [Parameter]
        public EventCallback<List<ApplicationRoleVM>> CallbackAfterSubmit { get; set; }

        [Inject]
        public ILocationService LocationService { get; set; }

        [Inject]
        IPolicyService PolicyService { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.roleClaim);
            this.messageStore = new ValidationMessageStore(this.editContext);
        }

        public async Task OpenModal(List<ApplicationRoleVM> roles)
        {
            this.editContext = new EditContext(this.roleClaim);
            roleClaims.Clear();
            this.roles.Clear();
            this.roles = roles;
            policies = (await PolicyService.GetAllPolicyAsync(new PolicyVM())).ToList();
            foreach (var policy in policies)
            {
                roleClaims.Add(new RoleClaim() { Type = policy.PolicyCode, Value = policy.PolicyDescription });
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task SearchBtn()
        {
            this.editContext = new EditContext(this.roleClaim);
            this.messageStore = new ValidationMessageStore(this.editContext);
            this.editContext.EnableDataAnnotationsValidation();

            if (this.editContext.Validate())
            {
                this.roles = this.FilterRoles(this.roles).ToList();

                this.isVisible = false;

                await this.CallbackAfterSubmit.InvokeAsync(this.roles);
            }
        }

        private void ClearBtn()
        {
            this.roleClaim = new RoleClaim();

        }



        private async Task OnFilterRoleClaimValue(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    //roleClaims.Clear();
                    policies = (await PolicyService.GetAllPolicyAsync(new PolicyVM())).ToList();
                    foreach (var policy in policies)
                    {
                        if (policy.PolicyDescription.Contains(args.Text))
                        {
                            roleClaim = new RoleClaim() { Type = policy.PolicyCode, Value = policy.PolicyDescription };
                        }
                    }
                }
                catch (Exception ex) { }

                var query1 = new Query().Where(new WhereFilter() { Field = "Value", Operator = "contains", value = args.Text, IgnoreCase = true });

                query1 = !string.IsNullOrEmpty(args.Text) ? query1 : new Query();

                await this.sfAutoCompleteRoleClaimValue.FilterAsync(roleClaims, query1);

            }
        }
        private async Task OnFilterRoleClaimType(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            if (args.Text.Length > 2)
            {
                try
                {
                    //roleClaims.Clear();
                    policies = (await PolicyService.GetAllPolicyAsync(new PolicyVM())).ToList();
                    foreach (var policy in policies)
                    {
                        if (policy.PolicyCode.Contains(args.Text))
                        {
                            roleClaim = new RoleClaim() { Type = policy.PolicyCode, Value = policy.PolicyDescription };
                        }
                    }
                }
                catch (Exception ex) { }

                var query1 = new Query().Where(new WhereFilter() { Field = "Type", Operator = "contains", value = args.Text, IgnoreCase = true });

                query1 = !string.IsNullOrEmpty(args.Text) ? query1 : new Query();

                await this.sfAutoCompleteRoleClaimType.FilterAsync(roleClaims, query1);

            }
        }

        private List<ApplicationRoleVM> FilterRoles(List<ApplicationRoleVM> roles)
        {
            if (!string.IsNullOrEmpty(this.roleClaim.Type))
            {
                roles = roles.Where(x => x.RoleClaims.Any(rc => rc.Type == this.roleClaim.Type)).ToList();
            }
 



            return roles;
        }

        private async Task OnValueChangeValue(ChangeEventArgs<string, RoleClaim> args)
        {
            this.roleClaim = this.roleClaims.Where(x => x.Value == args.Value).FirstOrDefault();
            if (roleClaim != null && roleClaim.Value != null && roleClaim.Type != null)
            {
                this.sfAutoCompleteRoleClaimType.Value = roleClaim.Type;
            }
            else
            {
                this.roleClaim = new RoleClaim();

            }
        }
        private async Task OnValueChangeType(ChangeEventArgs<string, RoleClaim> args)
        {
            this.roleClaim = this.roleClaims.Where(x => x.Type == args.Value).FirstOrDefault();
            if (roleClaim != null && roleClaim.Value != null && roleClaim.Type != null)
            {
                this.sfAutoCompleteRoleClaimValue.Value = roleClaim.Value;

            }
            else
            {
                this.roleClaim = new RoleClaim();
            }
        }
    }
}
