using System.ComponentModel.DataAnnotations;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Syncfusion.Blazor.Popups;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Pages.Common
{
    public partial class Login : ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? returnUrl { get; set; }

        [Inject]
        public ILocalStorageService localStorage { get; set; }

        [Inject]
        public ISessionStorageService sessionStorage { get; set; }

        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }

        [Inject]
        public SignInManager<ApplicationUser> signInManager { get; set; }

        [Inject]
        public NavigationManager NavMgr { get; set; }

        [Inject]
        public IDataProtectionProvider dataProtectionProvider { get; set; }

        [Inject]
        public ILogger<Login> Logger { get; set; }

        [Inject]
        public IConfiguration Configuration  { get; set; }

        [Inject]
        public ICommonService CommonService { get; set; }
        [Inject]
        public IDataSourceService dataSourceService { get; set; }
        [Inject]
        public ISettingService settingService { get; set; }

        private bool loading = false;

        public SignInModel signInModel = new SignInModel() { UserName = "", Email = "", Password = "" };
 
        private bool showSignInError = false; 

        public string Email { get; set; }
        public bool IsContinue { get; set; } = false;

        private string password;
        private string error = string.Empty;
        private string inputPasswordType = "password";
        private string passwordIconClass = "fa fa-solid fa-eye password-icon";
        int attemptSetting;
        protected DialogEffect AnimationEffect = DialogEffect.Zoom;
        ApplicationSetting appSetting;

        public string SessionKey { get; set; } = Guid.NewGuid().ToString();

        protected override Task OnInitializedAsync()
        {
            localStorage.ClearAsync();

            attemptSetting = Int32.Parse(( settingService.GetSettingByIntCodeAsync("MaxFailedAccessAttempts").Result.SettingValue));

            return base.OnInitializedAsync();
        }

        private void ShowPassword()
        {
            if (this.inputPasswordType == "password")
            {
                this.inputPasswordType = "text";
                this.passwordIconClass = "fa fa-solid fa-eye-slash password-icon";
            }
            else
            {
                this.inputPasswordType = "password";
                this.passwordIconClass = "fa fa-solid fa-eye password-icon";
            }
        }

        private async Task Cancel()
        {
            await signInManager.SignOutAsync();
        }

        private async Task eAuth()
        {

            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.ListDecodeParams =
                new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("eAuth", "Login") };


            string InternalEAuthURL = this.dataSourceService.GetSettingByIntCodeAsync("AppSettingInternalEAuthURL").Result.SettingValue;


            var token = this.CommonService.GetTokenWithParams(tokenContext, GlobalConstants.MINUTE_FIVE);
            NavMgr.NavigateTo($"{InternalEAuthURL}?token={token}", true);
        }

     
        private async Task StartRegistration()
        {
            //ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            //tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() {new KeyValuePair<string, object>("eAuth", "NewRegistration")};

            //ApplicationSetting appSetting = Configuration.GetRequiredSection("ApplicationSetting").Get<ApplicationSetting>();


            //var token = this.CommonService.GetTokenWithParams(tokenContext, 1);
            //NavMgr.NavigateTo(appSetting.InternalEAuthURL, true);

            //NavMgr.NavigateTo($"/StartRegistration", true);


            ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
            tokenContext.ResultContextObject.ListDecodeParams =
                new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("eAuth", "StartRegistration") };



            string InternalEAuthURL = this.dataSourceService.GetSettingByIntCodeAsync("AppSettingInternalEAuthURL").Result.SettingValue;

            var token = this.CommonService.GetTokenWithParams(tokenContext, GlobalConstants.MINUTE_FIVE);
            NavMgr.NavigateTo($"{InternalEAuthURL}?token={token}", true);
        }

        public async Task RegisterUser()
        {
            error = string.Empty;

            if (loading) return;

            try
            {
                loading = true;
                if(signInModel.UserName == null || signInModel.UserName == "")
                {
                    error = "Моля, въведете потребителско име!";
                    clearFields();
                    return;
                }

                if (signInModel.Password == null || signInModel.Password == "")
                {
                    error = "Моля, въведете парола!";

                    signInModel.Password = "";

                    return;
                }

                var user = await userManager.FindByNameAsync(signInModel.UserName);
                //var password = await userManager.CheckPasswordAsync(user, "udzkLzPyJdKWtV3");

                if (user == null)
                {
                    error = "Грешно потребителско име.";
                    clearFields();
                    return;
                }
                if((dataSourceService.GetAllKeyValueList().Where(x => x.IdKeyValue == user.IdUserStatus).First()).KeyValueIntCode.Equals("InActive"))
                {
                    error = "Вашият достъп до информационната система на НАПОО е спрян! Моля, свържете се с отговорните лица за контакт, за да получите повече информация.";
                    clearFields();
                    return;
                }

                if (await signInManager.CanSignInAsync(user))
                {
                    var result = await signInManager.CheckPasswordSignInAsync(user, signInModel.Password, true);
                    if (result == Microsoft.AspNetCore.Identity.SignInResult.Success)
                    {
                        //Guid key = Guid.NewGuid();
                        BlazorCookieLoginMiddleware.Logins[SessionKey] = new LoginInfo {
                                                SessionKey = SessionKey.ToString(),
                                                Email = signInModel.UserName,
                                                Password = signInModel.Password,
                                                ApplicationUser = user,
                                                ReturnUrl = returnUrl
                        };


                        await this.localStorage.SetItemAsync("LOCAL_LOGIN_KEY", SessionKey.ToString());
                        await this.sessionStorage.SetItemAsync("SESSION_LOGIN_KEY", SessionKey.ToString());

                        NavMgr.NavigateTo($"/login?key={SessionKey}", true);
                    }
                    else if(result == Microsoft.AspNetCore.Identity.SignInResult.LockedOut)
                    {
                        error = " Вашият акаунт е заключен!";
                        clearFields();
                    }
                    else
                    {
                        error = $"Грешна парола. Имате още {attemptSetting - user.AccessFailedCount} опита!";
                        signInModel.Password = "";
                    }
                }
                else
                {
                    error = "Акаунта ви е блокиран.";
                    clearFields();
                }
            }
            finally
            {
                loading = false;
            }



        }

        private void clearFields()
        {
            this.signInModel = new SignInModel();
        }
        private async Task ForgotPassword()
        {
            NavMgr.NavigateTo($"/ForgotPassword", true);
        }
    }

    public class SignInModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
       
        public string Password { get; set; }
    }

}
