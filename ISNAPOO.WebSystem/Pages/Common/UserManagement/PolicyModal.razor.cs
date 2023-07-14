using System;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Framework;
using ISNAPOO.WebSystem.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Syncfusion.Blazor.Popups;
using Syncfusion.Blazor.Spinner;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class PolicyModal : BlazorBaseComponent
    {
        [Inject]
        public IPolicyService PolicyService { get; set; }

        [Parameter]
        public EventCallback CallbackAfterSubmit { get; set; }

        private SfDialog sfDialog = new SfDialog();

        PolicyVM policy = new PolicyVM();

        string description;

        SfSpinner spinner = new SfSpinner();
        private ToastMsg toast = new ToastMsg();

        public override bool IsContextModified => this.editContext.IsModified();

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.policy);
        }

        public async Task openModal(PolicyVM policyVM)
        {


            this.editContext = new EditContext(this.policy);

            this.policy = policyVM;

            this.isVisible = true;

            this.StateHasChanged();
        }

        public async Task ChangePolicy()
        {

            this.SpinnerShow();

            bool hasPermission = await CheckUserActionPermission("ManagePolicyData", false);
            if (!hasPermission) { return; }

            this.editContext = new EditContext(this.policy);
            try
            {
                await PolicyService.UpdatePolicy(policy);
                await this.ShowSuccessAsync("Записът е успешен!");


            }
            catch
            {
                await this.ShowErrorAsync("Неуспешен запис!");
            }

            await this.CallbackAfterSubmit.InvokeAsync();

            this.SpinnerHide();
        }
    }
}

