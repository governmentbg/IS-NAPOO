using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Services.Common;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;


namespace ISNAPOO.WebSystem.Extensions
{
    public class PreventPasswordChangeValidator<TUser> : PasswordChangeValidatorBase<TUser> where TUser : IdentityUser<string>
    {
        private readonly IDataSourceService dataSourceService;

        public PreventPasswordChangeValidator(IDataSourceService dataSourceService)
        {
            this.dataSourceService = dataSourceService;
        }
        public override Task<IdentityResult> ValidatePasswordChangeAsync(UserManager<TUser> manager, TUser user, string password)
        {
            

            /*
             *  MinimumCapitalLetters = 2,
                MinimumLowLetters = 2,
                MinimumPasswordLength = 10,
                MinimumNumbers = 2,
                SpecialCharacters = "~@#$%^&*_<>!?|~^/+{}();",
                MinimumSpecialCharacters = 2
             */

            List<Setting> settings = this.dataSourceService.GetAllSettingList();

            PasswordPolicySettingsParameters passwordPolicySettingsParameters = new PasswordPolicySettingsParameters
                (
                    minLength: Int32.Parse(settings.Where(s => s.SettingIntCode == "MinimumPasswordLength").First().SettingValue),
                    minSpecials: Int32.Parse(settings.Where(s => s.SettingIntCode == "MinimumSpecialCharacters").First().SettingValue),
                    minCaps: Int32.Parse(settings.Where(s => s.SettingIntCode == "MinimumCapitalLetters").First().SettingValue),
                    minLows: Int32.Parse(settings.Where(s => s.SettingIntCode == "MinimumLowLetters").First().SettingValue),
                    minNumbers: Int32.Parse(settings.Where(s => s.SettingIntCode == "MinimumNumbers").First().SettingValue),
                    specials: settings.Where(s => s.SettingIntCode == "SpecialCharacters").First().SettingValue
                );


            //return Task.FromResult(IdentityResult.Failed(new IdentityError
            //{
            //    Code = "PasswordChange",
            //    Description = "You cannot change your password"
            //}));

            var result = PasswordPolicySettingsParameters.DoValidateNewPassword(password, passwordPolicySettingsParameters);

            if (result.Count()==0)
            {
                return Task.FromResult(IdentityResult.Success);

            }
            else {
                return Task.FromResult(IdentityResult.Failed(result.ToArray()));
            }




        }
    }

    public abstract class PasswordChangeValidatorBase<TUser>
        : ChangePasswordOnlyValidatorBase<TUser, string>
            where TUser : IdentityUser<string>
    { }

    /// <summary>
    /// A base class that only applies validations when the user is changing their password
    /// </summary>
    public abstract class ChangePasswordOnlyValidatorBase<TUser, TKey>
        : IPasswordValidator<TUser>
            where TUser : IdentityUser<TKey>
            where TKey : IEquatable<TKey>
    {
        /// <inheritdoc />
        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
        {
            if (password == null) { throw new ArgumentNullException(nameof(password)); }
            if (manager == null) { throw new ArgumentNullException(nameof(manager)); }

            var isNewUser = (user == null
                || user.Id == null
                || user.Id.Equals(default(TKey))
                || string.IsNullOrEmpty(user.PasswordHash));

            if (isNewUser)
            {
                return Task.FromResult(IdentityResult.Success);
            }
            return ValidatePasswordChangeAsync(manager, user, password);

        }

        public abstract Task<IdentityResult> ValidatePasswordChangeAsync(UserManager<TUser> manager, TUser user, string password);
    }
}
