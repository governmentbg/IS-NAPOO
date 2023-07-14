using System.ComponentModel.DataAnnotations;
using Blazored.LocalStorage;
using Data.Models.Data.ProviderData;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.DropDowns;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class ChangeProviderUserToProviderModal : BlazorBaseComponent
    {
        private SfAutoComplete<int?, CandidateProviderVM> cpAutoComplete = new SfAutoComplete<int?, CandidateProviderVM>();

        private Model model = new Model();
        private IEnumerable<CandidateProviderVM> providersSource = new List<CandidateProviderVM>();

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }

        [Inject]
        public IDataSourceService DataSourceService { get; set; }

        [Inject]
        public UserManager<ApplicationUser> UserManager { get; set; }

        [Inject]
        public SignInManager<ApplicationUser> SignInManager { get; set; }

        [Inject]
        public ILocalStorageService LocalStorage { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }

        protected override void OnInitialized()
        {
            this.editContext = new EditContext(this.model);
        }

        public async Task OpenModal()
        {
            this.model = new Model();
            this.editContext = new EditContext(this.model);

            this.providersSource = await this.CandidateProviderService.GetAllCandidateProvidersForAutoComplete("LicensingCPO", true);
            var kvTypeCPO = await this.DataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");
            foreach (var provider in this.providersSource)
            {
                if (provider.IdTypeLicense == kvTypeCPO.IdKeyValue)
                {
                    provider.MixCPOandCIPONameOwner = !string.IsNullOrEmpty(provider.ProviderName) ? $"ЦПО {provider.ProviderName} към {provider.ProviderOwner}" : $"ЦПО към {provider.ProviderOwner}";
                }
                else
                {
                    provider.MixCPOandCIPONameOwner = !string.IsNullOrEmpty(provider.ProviderName) ? $"ЦИПО {provider.ProviderName} към {provider.ProviderOwner}" : $"ЦИПО към {provider.ProviderOwner}";
                }
            }

            this.isVisible = true;
            this.StateHasChanged();
        }

        private async Task Submit()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    var result = await this.CandidateProviderService.ChangeProviderToUserAsync(this.model.IdCandidateProvider!.Value);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        await this.ShowSuccessAsync(string.Join("", result.ListMessages));
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task SubmitAndLogIn()
        {
            this.SpinnerShow();

            if (this.loading)
            {
                return;
            }
            try
            {
                this.loading = true;

                this.editContext = new EditContext(this.model);
                this.editContext.EnableDataAnnotationsValidation();

                if (this.editContext.Validate())
                {
                    var result = await this.CandidateProviderService.ChangeProviderToUserAsync(this.model.IdCandidateProvider!.Value);
                    if (result.HasErrorMessages)
                    {
                        await this.ShowErrorAsync(string.Join("", result.ListErrorMessages));
                    }
                    else
                    {
                        var user = await this.UserManager.FindByNameAsync("provider");
                        try
                        {
                            string key = Guid.NewGuid().ToString();
                            BlazorCookieLoginMiddleware.Logins[key] = new LoginInfo { Email = user.UserName, Password = null, ApplicationUser = user };
                            this.NavigationManager.NavigateTo($"/login?key={key}", true);

                            await this.LocalStorage.RemoveItemAsync("menu-id");
                        }
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                }
            }
            finally
            {
                this.loading = false;
            }

            this.SpinnerHide();
        }

        private async Task OnFilterCandidateProviderHandler(FilteringEventArgs args)
        {
            args.PreventDefaultAction = true;

            var query = new Query().Where(new WhereFilter() { Field = "ProviderJoinedInformation", Operator = "contains", value = args.Text, IgnoreCase = true });

            query = !string.IsNullOrEmpty(args.Text) ? query : new Query();

            await this.cpAutoComplete.FilterAsync(this.providersSource, query);
        }

        private class Model
        {
            [Required(ErrorMessage = "Полето 'Център' е задължително!")]
            public int? IdCandidateProvider { get; set; }
        }
    }
}


