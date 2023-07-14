using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using JWT.Exceptions;
using Microsoft.AspNetCore.Identity;
using NuGet.Common;
using System.Collections.Concurrent;
using System.Security.Claims;


namespace ISNAPOO.WebSystem.Extensions
{
    public class BlazorCookieLoginMiddleware
    {

        private readonly ILogger<BlazorCookieLoginMiddleware> _logger;

        public static IDictionary<string, LoginInfo> Logins { get; private set; } = new ConcurrentDictionary<string, LoginInfo>();
        public static IDictionary<string, LoginInfo> OnlineUsers { get; private set; } = new ConcurrentDictionary<string, LoginInfo>();

        private readonly RequestDelegate _next;
        public BlazorCookieLoginMiddleware(RequestDelegate next, ILogger<BlazorCookieLoginMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context, SignInManager<ApplicationUser> signInMgr, UserManager<ApplicationUser> userManager)
        {

            if ( context.Request.Query.ContainsKey("token"))
            {
                var token = context.Request.Query["token"].ToString();

                ResultContext<TokenVM> tokenContext = new ResultContext<TokenVM>();
                tokenContext.ResultContextObject.Token = token;
                try {
                    tokenContext = BaseHelper.GetDecodeToken(tokenContext);
                }
                catch (TokenExpiredException e)
                {
                    _logger.LogInformation($"Невалиден Token:{token}");
                    _logger.LogInformation(e.Message);
                    
                }
                catch (SignatureVerificationException e)
                {
                    _logger.LogInformation($"Невалиден Token:{token}");
                    _logger.LogInformation(e.Message);
                    

                }
                catch (Exception e)
                {
                    _logger.LogInformation($"Невалиден Token:{token}");
                    _logger.LogInformation(e.Message);
                }

              

                if (!tokenContext.ResultContextObject.IsValid) 
                {
                   
                    context.Response.Redirect("/InValidToken");
                    return;
                }
            }


            if (context.Request.Path == "/exit" && context.Request.Query.ContainsKey("key"))
            {

                var key = context.Request.Query["key"].ToString().Replace("\"", "");
                var logOutUser = BlazorCookieLoginMiddleware.OnlineUsers.FirstOrDefault(c => c.Key.Trim().ToLower() == key);
                if (logOutUser.Value != null)
                {
                    logOutUser.Value.LogoutTime = DateTime.Now;
                    _logger.LogInformation("Потребител {UserName} излезе от системата в {Time}.", logOutUser.Value.ApplicationUser.UserName, DateTime.UtcNow);
                }
                await signInMgr.SignOutAsync();
                context.Response.Redirect("/login");
                return;

            }

            if (context.Request.Path == "/EAuthEGovBG")
            {
                await signInMgr.SignOutAsync();
            }

            if (context.Request.Path == "/login" && context.Request.Query.ContainsKey("key"))
            {
                var key = context.Request.Query["key"];
                LoginInfo info = null;

                if (!Logins.TryGetValue(key, out info))
                {
                    context.Response.Redirect("/login");
                    return;
                }




                if (string.IsNullOrEmpty(info.Password))
                {
                    if (await signInMgr.CanSignInAsync(info.ApplicationUser))
                    {
                        await signInMgr.SignInAsync(info.ApplicationUser, isPersistent: true);
                        var remoteIp = context.Connection.RemoteIpAddress;

                        info.LoginTime = DateTime.Now;
                        info.LastActivity = DateTime.Now;
                        info.Activity = "Вход в системата";
                        info.ClientIP = remoteIp.ToString();


                        if (OnlineUsers.ContainsKey(key))
                        {
                            _logger.LogInformation("Потребител {UserName} влезе в системата в {Time}.", info.ApplicationUser.UserName, DateTime.UtcNow);


                            bool success = OnlineUsers.TryAdd(key, info);
                        }


                        Logins.Remove(key);

                        string returnUrlStr = string.Empty;
                        if (context.Request.Query.ContainsKey("returnUrl"))
                        {
                            returnUrlStr = context.Request.Query["returnUrl"];
                        }

                        context.Response.Redirect($"/");
                        return;
                    }
                    else
                    {

                        Logins.Remove(key);
                        context.Response.Redirect("/login");
                        return;
                    }
                }
                else
                {

                     


                    var result = await signInMgr.PasswordSignInAsync(info.Email, info.Password, false, lockoutOnFailure: true);
                    info.Password = null;
                    if (result.Succeeded)
                    {

                        var remoteIp = context.Connection.RemoteIpAddress;
                        info.LoginTime = DateTime.Now;
                        info.LastActivity = DateTime.Now;
                        info.Activity = "Вход в системата";
                        info.SessionKey = key.ToString();
                        info.ClientIP = remoteIp.ToString();

                        if (!OnlineUsers.ContainsKey(key))
                        {
                            OnlineUsers.Add(key, info);
                            _logger.LogInformation("Потребител {UserName} влезе в системата в {Time}.", info.ApplicationUser.UserName, DateTime.UtcNow);
                        }






                        Logins.Remove(key);
                        string returnUrlStr = string.Empty;
                        if (!string.IsNullOrEmpty(info.ReturnUrl))
                        {
                            returnUrlStr = info.ReturnUrl;
                        }

                        context.Response.Redirect($"/");
                        return;
                    }
                    else
                    {

                        Logins.Remove(key);
                        context.Response.Redirect("/login");
                        return;
                    }
                }
            }
            else
            {
                await _next.Invoke(context);
            }
        }

        public class LoginInfo
        {
            public int Id { get; set; }
            public string SessionKey { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public ApplicationUser ApplicationUser { get; set; }
            public string ReturnUrl { get; set; }
            public DateTime LoginTime { get; set; }
            public DateTime LastActivity { get; set; }
            public DateTime? LogoutTime { get; set; }
            public DateTime ExpireTime { get; set; }
            public string Activity { get; set; }
            public string ClientIP { get; set; }
        }
    }
}
