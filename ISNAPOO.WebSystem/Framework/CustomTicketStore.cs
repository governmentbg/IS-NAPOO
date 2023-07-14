using Data.Models.DB;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System;

namespace ISNAPOO.WebSystem.Framework
{
    public class CustomTicketStore : ITicketStore
    {
        private readonly DbContextOptionsBuilder<ApplicationDbContext> _optionsBuilder;
        private readonly IServiceCollection _services;

        public CustomTicketStore(DbContextOptionsBuilder<ApplicationDbContext> optionsBuilder  )
        {
            _optionsBuilder = optionsBuilder;
            
        }

        public CustomTicketStore(IServiceCollection services)
        {
            _services = services;
        }

        public async Task RemoveAsync(string key)
        {
            //using (var context = new ApplicationDbContext(_optionsBuilder.Options))
            //{
            //    if (Guid.TryParse(key, out var id))
            //    {
            //        var ticket = await context.AuthenticationTickets.SingleOrDefaultAsync(x => x.Id == id);
            //        if (ticket != null)
            //        {
            //            context.AuthenticationTickets.Remove(ticket);
            //            await context.SaveChangesAsync();
            //        }
            //    }
            //}

            using (var scope = _services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (Guid.TryParse(key, out var id))
                {
                    var ticket = await context.AuthenticationTickets.SingleOrDefaultAsync(x => x.Id == id);
                    if (ticket != null)
                    {
                        context.AuthenticationTickets.Remove(ticket);
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task RenewAsync(string key, AuthenticationTicket ticket)
        {
            //using (var context = new ApplicationDbContext(_optionsBuilder.Options))
            //{
            //    if (Guid.TryParse(key, out var id))
            //    {
            //        var authenticationTicket = await context.AuthenticationTickets.FindAsync(id);
            //        if (authenticationTicket != null)
            //        {
            //            authenticationTicket.Value = SerializeToBytes(ticket);
            //            authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
            //            authenticationTicket.Expires = ticket.Properties.ExpiresUtc;
            //            await context.SaveChangesAsync();
            //        }
            //    }
            //}


            using (var scope = _services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (Guid.TryParse(key, out var id))
                {
                    var authenticationTicket = await context.AuthenticationTickets.FindAsync(id);
                    if (authenticationTicket != null)
                    {
                        authenticationTicket.Value = SerializeToBytes(ticket);
                        authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
                        authenticationTicket.Expires = ticket.Properties.ExpiresUtc;
                        await context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<AuthenticationTicket> RetrieveAsync(string key)
        {
            using (var scope = _services.BuildServiceProvider().CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (Guid.TryParse(key, out var id))
                {
                    var authenticationTicket = await context.AuthenticationTickets.FindAsync(id);
                    if (authenticationTicket != null)
                    {
                        authenticationTicket.LastActivity = DateTimeOffset.UtcNow;
                        await context.SaveChangesAsync();

                        return DeserializeFromBytes(authenticationTicket.Value);
                    }
                }
            }

            return null;
        }

        public async Task<string> StoreAsync(AuthenticationTicket ticket)
        {
            var userId = string.Empty;
           

            var nameIdentifier = ticket.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
              
            using (var scope = _services.BuildServiceProvider().CreateScope())
            {
                
                //var nameIdentifier = ticket.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();

                if (ticket.AuthenticationScheme == "Identity.Application")
                {
                    userId = nameIdentifier;
                }
                else if (ticket.AuthenticationScheme == "Identity.External")
                {
                    userId = (await context.UserLogins.SingleAsync(x => x.ProviderKey == nameIdentifier)).UserId;
                }


                var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext != null)
                {
                    var remoteIpAddress = httpContext.Connection.RemoteIpAddress.ToString();
                    var browserInformation = httpContext.Request?.HttpContext?.Request?.Headers["user-agent"].ToString();

                    var claims = new List<Claim>
                            {
                               new Claim("remoteIpAddress",  remoteIpAddress),
                               new Claim("browserInformation", browserInformation),
                               new Claim("currentAction", String.Empty),
                               new Claim("currentActionDescription", String.Empty),
                               new Claim("currentUrl", String.Empty),
                               new Claim("currentMenu", String.Empty)
                            };
                    var appIdentity = new ClaimsIdentity(claims, "RuntimeData");

                    ticket.Principal.AddIdentity(appIdentity);
                }
                 


                var authenticationTicket = new Data.Models.Data.Common.AuthenticationTicket()
                {
                    UserId = userId,
                    LastActivity = DateTimeOffset.UtcNow,
                    Value = SerializeToBytes(ticket)
                };

                var expiresUtc = ticket.Properties.ExpiresUtc;
                if (expiresUtc.HasValue)
                {
                    authenticationTicket.Expires = expiresUtc.Value;
                }

                //var httpContextAccessor = scope.ServiceProvider.GetService<IHttpContextAccessor>();
                //var httpContext = httpContextAccessor?.HttpContext;
                //if (httpContext != null)
                //{
                //    var remoteIpAddress = httpContext.Connection.RemoteIpAddress;
                //    //if (remoteIpAddress != null)
                //    //{
                //    //    authenticationTicket.RemoteIpAddress = remoteIpAddress.ToString();
                //    //}

                //    var userAgent = httpContext.Request.Headers["User-Agent"];
                //    //if (!string.IsNullOrEmpty(userAgent))
                //    //{
                //    //    var uaParser = UAParser.Parser.GetDefault();
                //    //    var clientInfo = uaParser.Parse(userAgent);
                //    //    authenticationTicket.OperatingSystem = clientInfo.OS.ToString();
                //    //    authenticationTicket.UserAgentFamily = clientInfo.UserAgent.Family;
                //    //    authenticationTicket.UserAgentVersion = $"{clientInfo.UserAgent.Major}.{clientInfo.UserAgent.Minor}.{clientInfo.UserAgent.Patch}";
                //    //}
                //}

                context.AuthenticationTickets.Add(authenticationTicket);
                await context.SaveChangesAsync();

                return authenticationTicket.Id.ToString();
            }
        }

        private byte[] SerializeToBytes(AuthenticationTicket source)
            => TicketSerializer.Default.Serialize(source);

        private AuthenticationTicket DeserializeFromBytes(byte[] source)
            => source == null ? null : TicketSerializer.Default.Deserialize(source);
    }
}
