using System;
using Data.Models.Data.ProviderData;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Framework;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Pages.Common.UserManagement
{
    public partial class EAuthModal
    {
        private SfDialog sfDialog = new SfDialog();

        public IEnumerable<CandidateProviderVM> candidates;

        public int? ComboboxValue;

        private bool isVisible = false;

        [Inject]
        public ICandidateProviderService CandidateProviderService { get; set; }
        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }
        [Inject]
        public NavigationManager navigationManager { get; set; }
        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }

        public async Task OpenModal(IEnumerable<PersonVM> persons)
        {
            List<int> ids = new List<int>();

            List<CandidateProviderVM> UserCandidateProviders = new List<CandidateProviderVM>();
            foreach(var person in persons)
            {
                if(person.CandidateProviderPersons.Count!=0)
                ids.Add(person.CandidateProviderPersons.First().IdCandidate_Provider);
            }

            candidates = await CandidateProviderService.GetCandidateProvidersByListIdsWithIncludeAsync(ids);

            this.isVisible = true;
            this.StateHasChanged();
        }

        private void ValueChangeHandler(ChangeEventArgs<int, CandidateProviderVM> args)
        {
            ComboboxValue = args.Value;
        }

        public async Task login() {
            if (ComboboxValue == null && ComboboxValue==0) return;

           var cp = candidates.Where(x => x.IdCandidate_Provider == ComboboxValue).First();

            var userVM = await ApplicationUserService.GetAllApplicationUserAsync(new ApplicationUserVM { IdPerson = cp.CandidateProviderPersons.First().IdPerson});

            var user = await userManager.FindByIdAsync(userVM.First().Id);

            string key = Guid.NewGuid().ToString();
            BlazorCookieLoginMiddleware.Logins[key] = new LoginInfo { Email = user.UserName, Password = null, ApplicationUser = user };
            navigationManager.NavigateTo($"/login?key={key}", true);
        }

    }
}

