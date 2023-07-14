using Data.Models.Data.ProviderData;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace ISNAPOO.WebSystem.Framework
{
    public class AppClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
    {
        public AppClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }
        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var userId = await UserManager.GetUserIdAsync(user);
            var userName = await UserManager.GetUserNameAsync(user);
            var id = new ClaimsIdentity();
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserIdClaimType, userId));
            id.AddClaim(new Claim(Options.ClaimsIdentity.UserNameClaimType, userName));
            if (UserManager.SupportsUserSecurityStamp)
            {
                id.AddClaim(new Claim(Options.ClaimsIdentity.SecurityStampClaimType,
                    await UserManager.GetSecurityStampAsync(user)));
            }

            // code removed that adds the role claims 

            if (UserManager.SupportsUserClaim)
            {
                id.AddClaims(await UserManager.GetClaimsAsync(user));
            }
            return new ClaimsPrincipal(id);
        }
    }


    public class AppUserClaimsPrincipalFactory<TUser, TRole>
 : UserClaimsPrincipalFactory<TUser, TRole>
 where TUser : ApplicationUser
 where TRole : IdentityRole
    {
        public AppUserClaimsPrincipalFactory(
        UserManager<TUser> manager,
        RoleManager<TRole> rolemanager,
        IOptions<IdentityOptions> options)
          : base(manager, rolemanager, options)
        {
        }

        public async override Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            var id = await GenerateClaimsAsync(user);
            if (user != null)
            {
                id.AddClaim(new Claim(ClaimTypes.Name, user.UserName));
            }
            return new ClaimsPrincipal(id);
        }
    }
}
