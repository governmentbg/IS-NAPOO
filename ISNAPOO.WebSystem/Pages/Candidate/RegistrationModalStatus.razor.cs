using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using Microsoft.AspNetCore.Components;

namespace ISNAPOO.WebSystem.Pages.Candidate
{
    public partial class RegistrationModalStatus : ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Token { get; set; }

        [Inject]
        private NavigationManager NavMgr { get; set; }

        public string Messages { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ResultContext<TokenVM> currentContext = new ResultContext<TokenVM>();

            currentContext.ResultContextObject = new TokenVM();
            currentContext.ResultContextObject.Token = Token;
            currentContext = BaseHelper.GetDecodeToken(currentContext);

            this.Messages = currentContext.ResultContextObject.ListDecodeParams.FirstOrDefault(l => l.Key == "messages").Value.ToString().Replace('"', ' ').Trim('[', ']'); ;
        }

        protected override async void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        protected void RedirectToLogin()
        {
            this.NavMgr.NavigateTo("/Login", true);
        }
    }
}
