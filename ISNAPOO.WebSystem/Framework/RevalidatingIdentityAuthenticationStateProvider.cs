using System.Security.Claims;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Data.Models.Data.ProviderData;
using ISNAPOO.WebSystem.Extensions;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using static ISNAPOO.WebSystem.Extensions.BlazorCookieLoginMiddleware;

namespace ISNAPOO.WebSystem.Framework
{
    public class RevalidatingIdentityAuthenticationStateProvider<TUser> : RevalidatingServerAuthenticationStateProvider where TUser : class
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IdentityOptions _options;
        private readonly ILocalStorageService _localStorage;
        private readonly ISessionStorageService _sessionStorage;
        private readonly ILogger _logger;

        public string KEY { get; set; }

        public RevalidatingIdentityAuthenticationStateProvider(
                    ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory,
                    IOptions<IdentityOptions> optionsAccessor,
                    ILocalStorageService localStorage, ISessionStorageService sessionStorage) : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _options = optionsAccessor.Value;

            _localStorage = localStorage;
            _sessionStorage = sessionStorage;
            _logger = loggerFactory.CreateLogger<RevalidatingIdentityAuthenticationStateProvider<TUser>>();


            
        

        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(10);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
                                 AuthenticationState authenticationState,
                                 CancellationToken cancellationToken)
        {
            _logger.LogInformation($"ValidateAuthenticationStateAsync. authenticationState:{authenticationState.User.Identity.Name}");

            // Get the user manager from a new scope to ensure it fetches fresh data
            var scope = _scopeFactory.CreateScope();
            try
            {
                
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                var signInManager = scope.ServiceProvider.GetRequiredService<SignInManager<TUser>>();
                //NotifyAuthenticationStateChanged(Task.FromResult(authenticationState));
                return await ValidateSecurityStampAsync(userManager, signInManager, authenticationState.User);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, SignInManager<TUser> signInManager, ClaimsPrincipal principal)
        {
         


            var LOCAL_LOGIN_KEY = await _localStorage.GetItemAsync<string>("LOCAL_LOGIN_KEY");
            // var SESSION_LOGIN_KEY = await _sessionStorage.GetItemAsync<string>("SESSION_LOGIN_KEY");


           

            if (!string.IsNullOrEmpty(LOCAL_LOGIN_KEY))
            {
                 
                LoginInfo loginInfo = null;
                if (BlazorCookieLoginMiddleware.OnlineUsers.TryGetValue(LOCAL_LOGIN_KEY, out loginInfo))
                {
                    KEY = LOCAL_LOGIN_KEY;
                   _logger.LogInformation($"LOCAL_LOGIN_KEY:{LOCAL_LOGIN_KEY}");
                    _logger.LogInformation($"loginInfo.UserName:{loginInfo.ApplicationUser.UserName}");
                }
                else 
                {
                    _logger.LogError($"LOCAL_LOGIN_KEY NOT FOUND");
                  //  await signInManager.SignOutAsync();
                    _logger.LogInformation("User logged out during RevalidatingIdentity.");
                    return false;
                }
                
            //    //_logger.LogError($"SESSION_LOGIN_KEY:{SESSION_LOGIN_KEY}");
            //    //_logger.LogError($"OnlineUsers{LOCAL_LOGIN_KEY}.Email:{user1.Email}");
            //    //_logger.LogError($"OnlineUsers{LOCAL_LOGIN_KEY}.ApplicationUser.UserName:{user1.ApplicationUser.UserName}");
            }
            else 
            {
                KEY = string.Empty;
                _logger.LogError($"LOCAL_LOGIN_KEY IS EMPTY");
               // await signInManager.SignOutAsync();
                _logger.LogInformation("User logged out during RevalidatingIdentity.");


                return false;
               
            }

            var user = await userManager.GetUserAsync(principal);

            if (user == null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
