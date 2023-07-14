using System.Reflection.Metadata;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.WebSystem.Pages.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.IdentityModel.Tokens;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class ConfirmNewRegistration : ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Token { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

        private ToastMsg toast;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

            currentContext.ResultContextObject = new TokenVM();
            currentContext.ResultContextObject.Token = Token;
            currentContext = this.CommonService.GetDecodeToken(currentContext);

            if (currentContext.HasMessages)
            {
                // toast.sfErrorToast.Timeout = 0;
                //toast.sfErrorToast.Position.X = "Center";
                //toast.sfErrorToast.Content = $"Грешка при потвърждаване на електронна поща.\n {string.Join(", ", currentContext.ListMessages)}";
                // toast.sfErrorToast.ShowAsync();
                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("messages", currentContext.ListMessages) };

                this.NavMgr.NavigateTo($"RegistrationModalStatus?token={BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams)}", true);
                return;
            }

            if (currentContext.ResultContextObject.IsValid)
            {
                ResultContext<NoResult> currentNoResult = await this.CandidateProviderService.CreateCandidateProviderUserAsync(currentContext);

                if (currentNoResult.HasErrorMessages)
                {
                    //toast.sfErrorToast.Timeout = 0;
                    ////toast.sfErrorToast.Position.X = "Center";
                    //toast.sfErrorToast.Content = $"{string.Join(',', currentNoResult.ListErrorMessages)}";

                    //toast.sfErrorToast.ShowAsync();
                    ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("messages", $"{string.Join(',', currentNoResult.ListErrorMessages)}") };

                    this.NavMgr.NavigateTo($"RegistrationModalStatus?token={BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams)}", true);


                }
                else
                {
                    ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();

                    tokenContext.ResultContextObject.ListDecodeParams = new List<KeyValuePair<string, object>>() { new KeyValuePair<string, object>("messages", "Вие успешно потвърдихте Вашия e-mail адрес! След одобрение на електронната регистрация от служител на НАПОО, ще получите e-mail с информация за предоставения за Вас достъп до информационната система.") };

                    this.NavMgr.NavigateTo($"RegistrationModalStatus?token={BaseHelper.GetTokenWithParams(tokenContext.ResultContextObject.ListDecodeParams)}", true);

                    //toast.sfSuccessToast.Content = $"Вие успешно потвърдихте Вашия e-mail адрес! След одобрение на електронната регистрация от служител на НАПОО, ще получите e-mail с информация за предоставения за Вас достъп до информационната система.";

                    //toast.sfSuccessToast.ShowAsync();
                }
            }
        }
    }
}
