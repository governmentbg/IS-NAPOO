using System.Text;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Extensions;
using ISNAPOO.WebSystem.Pages.Common.UserManagement;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class EAuthEGovBG : ComponentBase
    {

        [Inject]
        public IPersonService personService { get; set; }

        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }

        [Inject]
        public IApplicationUserService ApplicationUserService { get; set; }

        [Inject]
        public SignInManager<ApplicationUser> signInManager { get; set; }


        private readonly IHttpClientFactory _clientFactory;

        ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

        private EAuthModal eAuthModal = new EAuthModal();


        [Parameter]
        [SupplyParameterFromQuery]
        public string? token
        {
            get; set;
        }

        private bool ShowMsgNoAccount { get; set; } = false;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender) 
            {
                if (!string.IsNullOrEmpty(token))
                {
                    tokenContext = new ResultContext<TokenVM>();
                    tokenContext.ResultContextObject.Token = token;
                    tokenContext = this.CommonService.GetDecodeToken(tokenContext);

                };
                 

                if (tokenContext.ResultContextObject.IsValid)
                {


                    KeyValuePair<string, object> responseStatus = tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "ResponseStatus");


                    if (responseStatus.Key == default || responseStatus.Value.ToString() != "Success")
                    {
                        NavMgr.NavigateTo("/login", true);
                        return;
                    }

                    if (tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "eAuth").Value.ToString() == "Login")
                    {
                        string EGN = tokenContext.ResultContextObject.ListDecodeParams.Where(p => p.Key == "EGN").FirstOrDefault().Value.ToString();
                        var persons = await personService.GetAllPersonsAsync(new PersonVM() { Indent = EGN });

                        if (persons.Count() == 1)
                        {
                            var appUser = await ApplicationUserService.GetAllApplicationUserAsync(
                            new Core.ViewModels.Identity.ApplicationUserVM() { IdPerson = persons.FirstOrDefault().IdPerson }
                            );

                            var user = await userManager.FindByNameAsync(appUser.FirstOrDefault().UserName);

                            try
                            {

                                string key = Guid.NewGuid().ToString();
                                BlazorCookieLoginMiddleware.Logins[key] = new LoginInfo { Email = user.UserName, Password = null, ApplicationUser = user };
                                NavMgr.NavigateTo($"/login?key={key}", true);
                            }
                            catch (Exception e)
                            {
                                throw e;
                            }
                        }
                        else if (persons.Count() > 1)
                        {
                            await eAuthModal.OpenModal(persons);

                        }
                        else
                        {
                            ShowMsgNoAccount = true;
                            this.StateHasChanged();
                            return;
                        }
                    }
                    else if (tokenContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "eAuth").Value.ToString() == "StartRegistration")
                    {
                        ResultContext<TokenVM> StartRegistrationToken = new ResultContext<TokenVM>();

                        foreach (var data in tokenContext.ResultContextObject.ListDecodeParams)
                        {
                            if (data.Key != "exp")
                            {
                                StartRegistrationToken.ResultContextObject.ListDecodeParams.Add(new KeyValuePair<string, object>(data.Key, data.Value));
                            }
                        }

                        var token = this.CommonService.GetTokenWithParams(StartRegistrationToken, GlobalConstants.MINUTE_ONE);


                        NavMgr.NavigateTo($"/StartRegistration?token={token}", true);
                    }
                }
                else
                {
                    NavMgr.NavigateTo("/login", true);
                }
            } 
        } 
    }
}
