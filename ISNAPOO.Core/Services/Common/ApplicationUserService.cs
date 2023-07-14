using Data.Models.Common;
using Data.Models.Data.Archive;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Role;
using Data.Models.Data.Training;
using Data.Models.Framework;
using ISNAPOO.Common.Constants;
using ISNAPOO.Common.Framework;
using ISNAPOO.Common.HelperClasses;
using ISNAPOO.Core.Contracts.Candidate;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.Contracts.Mailing;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Identity;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Syncfusion.DocIO.DLS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class ApplicationUserService : BaseService, IApplicationUserService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<ApplicationUserService> _logger;
        private readonly IMailService MailService;
        private readonly IRepository repository;
        private readonly IDataSourceService dataSourceService;
        public IPersonService personService { get; set; }

        public ApplicationUserService(
            IRepository repository,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IDataSourceService dataSourceService,
            ILogger<ApplicationUserService> logger,
            IPersonService personService, AuthenticationStateProvider authenticationStateProvider,
            IMailService mailService) : base(repository, authenticationStateProvider)
        {
            this.repository = repository;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.dataSourceService = dataSourceService;
            this.signInManager = signInManager;
            this._logger = logger;
            this.personService = personService;
            this.MailService = mailService;
        }
        public async Task<ResultContext<ApplicationUserVM>> CreateApplicationUserAsync(ResultContext<ApplicationUserVM> resultContext)
        {
            var user = CreateUser();


            user.IdPerson = resultContext.ResultContextObject.IdPerson;
            user.Email = resultContext.ResultContextObject.Email;
            resultContext.ResultContextObject.Password = GenerateRandomPassword();
            user.UserName = await GenerateUserName(resultContext.ResultContextObject);
            user.IdUser = (int)(await GetSequenceNextValue("APPLICATION_USER_ID"));
            user.IdUserStatus = resultContext.ResultContextObject.IdUserStatus;

            user.CreationDate = DateTime.Now;
            user.ModifyDate = DateTime.Now;

            user.IdCreateUser = UserProps.UserId;
            user.IdModifyUser = UserProps.UserId;



            var result = await userManager.CreateAsync(user, resultContext.ResultContextObject.Password);

            resultContext.ResultContextObject.UserName = user.UserName;

            if (result.Succeeded)
            {


                if (resultContext.ResultContextObject.IdCandidateProvider != 0)
                {
                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.ID_CANDIDATE_PROVIDER,
                            resultContext.ResultContextObject.IdCandidateProvider.ToString()));

                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.ID_USER,
                            user.IdUser.ToString()));

                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.ID_PERSON,
                            resultContext.ResultContextObject.IdPerson.ToString()));

                    await userManager.AddClaimAsync(user,
                      new System.Security.Claims.Claim(
                          GlobalConstants.PERSON_FULLNAME,
                         resultContext.ResultContextObject.FullName));

                    var candidateProvder = await this.repository.GetByIdAsync<CandidateProvider>(resultContext.ResultContextObject.IdCandidateProvider);

                    var currentUser = await this.userManager.FindByIdAsync(UserProps.ID);

                    var registrationStatus = await this.dataSourceService.GetKeyValueByIntCodeAsync("CandidateProviderStatus", "AWAITING_CONFIRMATION_FROM_NAPOO");

                    if(!resultContext.ResultContextObject.IsFirstReistration)
                    {
                        var roles = await this.userManager.GetRolesAsync(currentUser);

                        foreach (var role in roles)
                        {
                            await userManager.AddToRoleAsync(user, role);
                        }
                    }
                    else
                    {
                        var licenceCIPOKv = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCIPO");
                        var licenceCPOKv = await this.dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");

                        var role = string.Empty;

                        if(candidateProvder is not null)
                        {
                            if(candidateProvder.IdTypeLicense == licenceCPOKv.IdKeyValue)
                            {
                                role = "Candidate_CPO";
                            }
                            else if (candidateProvder.IdTypeLicense == licenceCIPOKv.IdKeyValue)
                            {
                                role = "Candidate_CIPO";
                            }

                            await userManager.AddToRoleAsync(user, role);
                        }

                    }
                    //var candidateProvder = await this.repository.GetByIdAsync<CandidateProvider>(resultContext.ResultContextObject.IdCandidateProvider);

                    //var keyValue = await dataSourceService.GetKeyValueByIntCodeAsync("ApplicationStatus", "ProcedureCompleted");

                    //var licenseTypeKeyValue = await dataSourceService.GetKeyValueByIntCodeAsync("LicensingType", "LicensingCPO");

                    //Setting setting;

                    //if (!candidateProvder.IdApplicationStatus.HasValue)
                    //{
                    //    if (licenseTypeKeyValue.IdKeyValue == candidateProvder.IdTypeLicense)
                    //    {
                    //        setting = await dataSourceService.GetSettingByIntCodeAsync("LicensedCPO");

                    //    }
                    //    else
                    //    {
                    //        setting = await dataSourceService.GetSettingByIntCodeAsync("LicensedCIPO");
                    //    }
                    //}
                    //else
                    //{
                    //    if (licenseTypeKeyValue.IdKeyValue == candidateProvder.IdTypeLicense)
                    //    {
                    //        setting = await dataSourceService.GetSettingByIntCodeAsync("UnicensedCPO");

                    //    }
                    //    else
                    //    {
                    //        setting = await dataSourceService.GetSettingByIntCodeAsync("UnicensedCIPO");
                    //    }
                    //}


                    //foreach (var role in setting.SettingValue.Split(","))
                    //{
                    //    await userManager.AddToRoleAsync(user, role);
                    //}
                }

                _logger.LogInformation($"User created an account with UserName:{user.UserName}.");

            }

            return resultContext;
        }

        public async Task<ResultContext<ApplicationUserVM>> CreateApplicationExpertUserAsync(ResultContext<ApplicationUserVM> resultContext)
        {
            var user = CreateUser();


            user.IdPerson = resultContext.ResultContextObject.IdPerson;
            user.IdUserStatus = resultContext.ResultContextObject.IdUserStatus;
            user.Email = resultContext.ResultContextObject.Email;
            resultContext.ResultContextObject.Password = GenerateRandomPassword();
            user.UserName = await GenerateUserName(resultContext.ResultContextObject);
            user.IdUser = (int)(await GetSequenceNextValue("APPLICATION_USER_ID"));

            user.IdCreateUser = UserProps.UserId;
            user.IdModifyUser = UserProps.UserId;
            user.CreationDate = DateTime.Now;
            user.ModifyDate = DateTime.Now;

            var result = await userManager.CreateAsync(user, resultContext.ResultContextObject.Password);

            resultContext.ResultContextObject.UserName = user.UserName;

            if (result.Succeeded)
            {
                if (resultContext.ResultContextObject.IdPerson != 0)
                {

                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.ID_USER,
                            user.IdUser.ToString()));

                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.ID_PERSON,
                            resultContext.ResultContextObject.IdPerson.ToString()));

                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.PERSON_FULLNAME,
                           resultContext.ResultContextObject.FullName));

                    if (resultContext.ResultContextObject.IsCommissionExpert && resultContext.ResultContextObject.IsExternalExpert)
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.EXPERT_COMMITTEES);
                        await userManager.AddToRoleAsync(user, GlobalConstants.EXTERNAL_EXPERTS);
                    }
                    else if (resultContext.ResultContextObject.IsCommissionExpert)
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.EXPERT_COMMITTEES);
                    }
                    else if (resultContext.ResultContextObject.IsExternalExpert)
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.EXTERNAL_EXPERTS);
                    }
                    else if (resultContext.ResultContextObject.IsNapooExpert)
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.NAPOO_Expert);
                    }
                    else if (resultContext.ResultContextObject.IsDOCExpert)
                    {
                        await userManager.AddToRoleAsync(user, GlobalConstants.EXPERT_DOS);
                    }
                }
                _logger.LogInformation($"User created an account with UserName:{user.UserName}.");

            }

            return resultContext;
        }




        public async Task<ResultContext<ApplicationUserVM>> CreateApplicationUserAsAdminAsync(ResultContext<ApplicationUserVM> resultContext)
        {

            var isUnique = await personService.CheckPersonIdentIsUniqueForCandidateProvider(resultContext.ResultContextObject.IndentTypeName, resultContext.ResultContextObject.IdPerson, resultContext.ResultContextObject.IdCandidateProvider);

            if (!isUnique)
            {
                resultContext.AddErrorMessage("В базата има записан потребител със същото 'ЕГН/ЛНЧ/ИДН'!");
                return resultContext;
            }

            Person person = new Person();
            person.FirstName = resultContext.ResultContextObject.FirstName;
            person.SecondName = resultContext.ResultContextObject.MiddleName;
            person.FamilyName = resultContext.ResultContextObject.FamilyName;
            person.Indent = resultContext.ResultContextObject.IndentTypeName;
            person.IdIndentType = resultContext.ResultContextObject.IdIndentType;
            person.Email = resultContext.ResultContextObject.Email;
            person.Phone = resultContext.ResultContextObject.Phone;
            person.TaxOffice = String.Empty;

            await this.repository.AddAsync<Person>(person);
            int result = await this.repository.SaveChangesAsync();

            this.repository.Detach(person);


            if (result > 0)
            {
                var user = CreateUser();
                user.IdPerson = person.IdPerson;
                user.Email = resultContext.ResultContextObject.Email;
                resultContext.ResultContextObject.Password = GenerateRandomPassword();
                user.UserName = await GenerateUserName(resultContext.ResultContextObject);
                user.IdUser = (int)(await GetSequenceNextValue("APPLICATION_USER_ID"));
                user.IdUserStatus = resultContext.ResultContextObject.IdUserStatus;
                user.IdCreateUser = person.IdCreateUser;
                user.CreationDate = person.CreationDate;
                user.IdModifyUser = person.IdModifyUser;
                user.ModifyDate = person.ModifyDate;
                var resultUser = await userManager.CreateAsync(user, resultContext.ResultContextObject.Password);

                if (resultUser.Succeeded)
                {
                    await userManager.AddClaimAsync(user,
                       new System.Security.Claims.Claim(
                           GlobalConstants.ID_USER,
                           user.IdUser.ToString()));

                    await userManager.AddClaimAsync(user,
                        new System.Security.Claims.Claim(
                            GlobalConstants.ID_PERSON,
                            user.IdPerson.ToString()));

                    await userManager.AddClaimAsync(user,
                       new System.Security.Claims.Claim(
                           GlobalConstants.PERSON_FULLNAME,
                          resultContext.ResultContextObject.FullName));

                    resultContext.ResultContextObject = user.To<ApplicationUserVM>();
                    resultContext.AddMessage("Записът е успешен!");
                }
                foreach (var r in resultContext.ResultContextObject.Roles)
                {
                    await userManager.AddToRoleAsync(user, r.Name);
                }
            }



            return resultContext;
        }



        private async Task<string> GenerateUserName(ApplicationUserVM createUserVM)
        {
            string userName = string.Empty;

            string _firstName = BaseHelper.ConvertCyrToLatin(createUserVM.FirstName);
            string _lastName = BaseHelper.ConvertCyrToLatin(createUserVM.FamilyName);

            userName = _firstName.Substring(0, 1).ToLower() + _lastName.ToLower();

            if (createUserVM.EIK == null)
            {
                createUserVM.EIK = "0000";
            }
            userName = userName + "_" + createUserVM.EIK.Substring(createUserVM.EIK.Length - 4);

            var seq = await this.GetSequenceNextValue("APPLICATIONUSER#" + userName);

            if (seq != 1)
            {
                userName = userName + seq;
            }

            return userName;

        }



        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the external login page in /Areas/Identity/Pages/Account/ExternalLogin.cshtml");
            }
        }

        public static string GenerateRandomPassword()
        {
            PasswordPolicySettingsParameters settingParams = PasswordPolicySettingsParameters.GetPasswordPolicySettingsParameters();



            string passwordChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789" + settingParams.SpecialCharacters;
            string capLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowLetters = "abcdefghijklmnopqrstuvwxyz";
            string numbers = "0123456789";

            char[] password = new char[settingParams.MinimumPasswordLength];

            Random rand = new Random();
            int position = rand.Next(0, settingParams.MinimumPasswordLength);

            for (int i = 0; i < settingParams.MinimumCapitalLetters; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = capLetters[rand.Next(0, capLetters.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }

            for (int i = 0; i < settingParams.MinimumLowLetters; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = lowLetters[rand.Next(0, lowLetters.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }


            for (int i = 0; i < settingParams.MinimumNumbers; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = numbers[rand.Next(0, numbers.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }
            for (int i = 0; i < settingParams.MinimumSpecialCharacters; i++)
            {
                while (password[position] != '\0')
                {
                    position = rand.Next(0, settingParams.MinimumPasswordLength);

                }
                password[position] = settingParams.SpecialCharacters[rand.Next(0, settingParams.SpecialCharacters.Length - 1)];
                position = rand.Next(0, settingParams.MinimumPasswordLength);
            }

            for (int i = 0; i < password.Length; i++)
            {
                if (password[i] == '\0')
                {
                    password[i] = passwordChars[rand.Next(0, passwordChars.Length - 1)];
                }
            }
            return new string(password);

        }

        public async Task<IEnumerable<ApplicationUserVM>> GetAllApplicationUserAsync(ApplicationUserVM applicationUserVM)
        {
            var applicationUsers = this.repository.AllReadonly<ApplicationUser>(FilterApplicationUser(applicationUserVM)).Include(u => u.Person).ThenInclude(u => u.CandidateProviderPersons).AsNoTracking().Include(u => u.Roles).AsNoTracking();
            var applicationRoles = await GetAllApplicationRoleAsync(new ApplicationRoleVM());
            var kvSource = dataSourceService.GetAllKeyValueList();

            List<ApplicationUserVM> applicationUsersVM = new List<ApplicationUserVM>();

            foreach (var applicationUser in applicationUsers)
            {
                if (applicationUser.IdPerson != null)
                {
                    applicationUsersVM.Add(new ApplicationUserVM()
                    {
                        Id = applicationUser.Id,
                        UserName = applicationUser.UserName,
                        FirstName = applicationUser.Person?.FirstName,
                        FamilyName = applicationUser.Person?.FamilyName,
                        Phone = applicationUser.Person?.Phone,
                        Email = applicationUser.Person?.Email,
                        MiddleName = applicationUser.Person?.SecondName,
                        IndentTypeName = applicationUser.Person?.Indent,
                        IdPerson = applicationUser.Person.IdPerson,
                        Person = applicationUser.Person.To<PersonVM>(),
                        Roles = applicationRoles.Where(ur => applicationUser.Roles.Select(r => r.RoleId).Contains(ur.Id)).ToList(),
                        RolesInfo = string.Join(", ", applicationRoles.Where(ur => applicationUser.Roles.Select(r => r.RoleId).Contains(ur.Id)).Select(ur => ur.RoleName)),
                        UserStatusName = kvSource.FirstOrDefault(k => k.IdKeyValue == applicationUser.IdUserStatus).Name,
                        IdIndentType = applicationUser.Person?.IdIndentType,
                        IdUserStatus = (int)applicationUser.IdUserStatus,
                        IdCreateUser = applicationUser.IdCreateUser,
                        IdModifyUser = applicationUser.IdModifyUser,
                        ModifyDate = applicationUser.ModifyDate,
                        CreationDate = applicationUser.CreationDate,
                        
                    });
                }
                else
                {
                    applicationUsersVM.Add(new ApplicationUserVM()
                    {
                        Id = applicationUser.Id,
                        UserName = applicationUser.UserName,
                        FirstName = applicationUser.Person?.FirstName,
                        FamilyName = applicationUser.Person?.FamilyName,
                        Phone = applicationUser.Person?.Phone,
                        Email = applicationUser.Person?.Email,
                        MiddleName = applicationUser.Person?.SecondName,
                        IndentTypeName = applicationUser.Person?.Indent,
                        Roles = applicationRoles.Where(ur => applicationUser.Roles.Select(r => r.RoleId).Contains(ur.Id)).ToList(),
                        RolesInfo = string.Join(", ", applicationRoles.Where(ur => applicationUser.Roles.Select(r => r.RoleId).Contains(ur.Id)).Select(ur => ur.RoleName)),
                        UserStatusName = kvSource.FirstOrDefault(k => k.IdKeyValue == applicationUser.IdUserStatus).Name,
                        IdIndentType = applicationUser.Person?.IdIndentType,
                        IdUserStatus = (int)applicationUser.IdUserStatus,
                        ModifyDate = applicationUser.ModifyDate,
                        CreationDate = applicationUser.CreationDate,
                      //  IdCandidateProvider = applicationUser.Person.CandidateProviderPersons.FirstOrDefault()
                    });
                }

            }

            return applicationUsersVM;
        }

        protected Expression<Func<ApplicationUser, bool>> FilterApplicationUser(ApplicationUserVM modelFilter)
        {
            var predicate = PredicateBuilder.True<ApplicationUser>();

            if (modelFilter.IdPerson != GlobalConstants.INVALID_ID
               && modelFilter.IdPerson != GlobalConstants.INVALID_ID_ZERO)
            {
                predicate = predicate.And(p => p.IdPerson == modelFilter.IdPerson);
            }

            if (modelFilter.IdUser != 0)
            {
                predicate = predicate.And(p => p.IdUser == modelFilter.IdUser);
            }

            return predicate;
        }



        public async Task<ApplicationUserVM> GetAllApplicationRolePerUserAsync(ApplicationUserVM applicationUserVM)
        {
            applicationUserVM.Roles.Clear();
            if (applicationUserVM.UserName == null)
            {
                return null;
            }
            var roles = await userManager.GetRolesAsync(await userManager.FindByNameAsync(applicationUserVM.UserName));


            foreach (var applicationRole in roles)
            {
                var role = await this.roleManager.FindByNameAsync(applicationRole);
                applicationUserVM.Roles.Add(new ApplicationRoleVM()
                {
                    Id = role.Id,
                    Name = role.Name,
                    RoleName = role.RoleName
                });
            }

            return applicationUserVM;
        }


        public async Task<IEnumerable<ApplicationRoleVM>> GetAllApplicationRoleAsync(ApplicationRoleVM аpplicationRoleVM)
        {
            var applicationRoles = this.repository.AllReadonly<ApplicationRole>(FilterApplicationRole(аpplicationRoleVM));

            List<ApplicationRoleVM> аpplicationRolesVM = new List<ApplicationRoleVM>();

            foreach (var applicationRole in applicationRoles)
            {
                аpplicationRolesVM.Add(new ApplicationRoleVM()
                {
                    Id = applicationRole.Id,
                    Name = applicationRole.Name,
                    RoleName = applicationRole.RoleName,

                });
            }


            return аpplicationRolesVM;
        }
        public async Task<IEnumerable<ApplicationRoleVM>> GetAllApplicationRoleAsync()
        {
            var applicationRoles = this.repository.AllReadonly<ApplicationRole>();

            List<ApplicationRoleVM> аpplicationRolesVM = new List<ApplicationRoleVM>();

            foreach (var applicationRole in applicationRoles)
            {
                var Claims = await this.roleManager.GetClaimsAsync(applicationRole);




                аpplicationRolesVM.Add(new ApplicationRoleVM()
                {
                    Id = applicationRole.Id,
                    Name = applicationRole.Name,
                    RoleName = applicationRole.RoleName,
                    RoleClaims = Claims.Select(c => new RoleClaim() { Type = c.Type, Value = c.Value }).ToList()
                });
            }


            return аpplicationRolesVM;
        }

        protected Expression<Func<ApplicationRole, bool>> FilterApplicationRole(ApplicationRoleVM аpplicationRoleVM)
        {
            var predicate = PredicateBuilder.True<ApplicationRole>();


            return predicate;
        }


        public async Task<ApplicationRoleVM> GetApplicationRoleByIdAsync(ApplicationRoleVM applicationRoleVM)
        {
            //IQueryable<ApplicationRole> applicationRoleList = this.repository.AllReadonly<ApplicationRole>(x => x.Id == applicationRoleVM.Id);

            var role = await this.roleManager.FindByIdAsync(applicationRoleVM.Id);

            var Claims = await this.roleManager.GetClaimsAsync(role);

            applicationRoleVM = role.To<ApplicationRoleVM>();

            applicationRoleVM.RoleClaims.AddRange(Claims.Select(c => new RoleClaim() { Type = c.Type, Value = c.Value }));

            return applicationRoleVM;
        }

        public async Task<ResultContext<ApplicationRoleVM>> CreateApplicationRoleAsync(ApplicationRoleVM applicationRoleVM)
        {
            ResultContext<ApplicationRoleVM> outputContext = new ResultContext<ApplicationRoleVM>();



            IdentityResult identityResult = await this.roleManager.CreateAsync(new ApplicationRole() { Name = applicationRoleVM.Name, RoleName = applicationRoleVM.RoleName, IdCreateUser = this.UserProps.UserId, CreationDate = DateTime.Now });

            if (identityResult.Succeeded)
            {
                var role = await this.roleManager.FindByNameAsync(applicationRoleVM.Name);

                foreach (var claim in applicationRoleVM.RoleClaims)
                {
                    await this.roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, string.Empty) { });
                }

                applicationRoleVM.Id = role.Id;

                outputContext.ResultContextObject = applicationRoleVM;
            }


            //try
            //{
            //    applicationRoleVM.ModifyDate = DateTime.Now;
            //    applicationRoleVM.CreationDate = DateTime.Now;
            //    applicationRoleVM.IdCreateUser = Users.UserId;
            //    applicationRoleVM.IdModifyUser = Users.UserId;

            //    var entryForDb = applicationRoleVM.To<ApplicationRole>();

            //    await this.repository.AddAsync<ApplicationRole>(entryForDb);
            //    await this.repository.SaveChangesAsync();

            //    outputContext.AddMessage("Записът е успешен!");
            //    applicationRoleVM.Id = entryForDb.Id;
            //    outputContext.ResultContextObject = applicationRoleVM;

            //    this.repository.Detach<ApplicationRole>(entryForDb);
            //}
            //catch (Exception ex)
            //{
            //    outputContext.AddErrorMessage(ex.Message);
            //}

            return outputContext;
        }


        public async Task<ResultContext<ApplicationUserVM>> UpdateApplicationUserAsync(ResultContext<ApplicationUserVM> resultContext)
        {
            var isUnique = await personService.CheckPersonIdentIsUniqueForCandidateProvider(resultContext.ResultContextObject.IndentTypeName, resultContext.ResultContextObject.IdPerson, resultContext.ResultContextObject.IdCandidateProvider);

            if (!isUnique)
            {
                resultContext.AddErrorMessage("В базата има записан потребител със същото 'ЕГН/ЛНЧ/ИДН'!");
                return resultContext;
            }

            ResultContext<ApplicationUserVM> outputContext = new ResultContext<ApplicationUserVM>();
            ApplicationUser user = await userManager.FindByNameAsync(resultContext.ResultContextObject.UserName);
            //var user = await this.repository.GetByIdAsync<ApplicationUser>(resultContext.ResultContextObject.Id);
            var roles = await userManager.GetRolesAsync(user);



            foreach (var r in resultContext.ResultContextObject.Roles)
            {
                if (!roles.Contains(r.Name))
                {
                    try
                    {
                        await userManager.AddToRoleAsync(user, r.Name);
                    }
                    catch (Exception e)
                    {


                    }

                }
            }

            var person = await this.repository.GetByIdAsync<Person>((int)user.IdPerson);
            person.FirstName = resultContext.ResultContextObject.FirstName;
            person.SecondName = resultContext.ResultContextObject.MiddleName;
            person.FamilyName = resultContext.ResultContextObject.FamilyName;
            person.IdIndentType = resultContext.ResultContextObject.IdIndentType;
            person.Indent = resultContext.ResultContextObject.IndentTypeName;
            person.Phone = resultContext.ResultContextObject.Phone;
            person.Email = resultContext.ResultContextObject.Email;

            this.repository.Update<Person>(person);
            await this.repository.SaveChangesAsync();

            user.Email = resultContext.ResultContextObject.Email;
            user.NormalizedEmail = resultContext.ResultContextObject.Email.ToUpper();
            user.IdUserStatus = resultContext.ResultContextObject.IdUserStatus;

            if (user.Person != null)
            {
                this.repository.Detach<Person>(user.Person);
                user.Person = null;
            }

            this.repository.Update<ApplicationUser>(user);
            await this.repository.SaveChangesAsync();

            outputContext.ResultContextObject = user.To<ApplicationUserVM>();
            outputContext.AddMessage("Записът е успешен!");


            return outputContext;


        }
        public async Task<ResultContext<ApplicationUserVM>> UpdateApplicationExpertUserAsync(ResultContext<ApplicationUserVM> resultContext)
        {
            ResultContext<ApplicationUserVM> outputContext = new ResultContext<ApplicationUserVM>();
            ApplicationUser user = await userManager.FindByNameAsync(resultContext.ResultContextObject.UserName);
            //var user = await this.repository.GetByIdAsync<ApplicationUser>(resultContext.ResultContextObject.Id);
            var roles = await userManager.GetRolesAsync(user);

            foreach (var r in resultContext.ResultContextObject.Roles)
            {
                if (!roles.Contains(r.Name))
                {
                    try
                    {
                        await userManager.AddToRoleAsync(user, r.Name);
                    }
                    catch (Exception e)
                    {


                    }

                }
            }
            //EXPERT_COMMITTEES
            if (resultContext.ResultContextObject.IsCommissionExpert)
            {
                if (!roles.Contains(GlobalConstants.EXPERT_COMMITTEES))
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.EXPERT_COMMITTEES);
                }
            }
            else
            {
                if (roles.Contains(GlobalConstants.EXPERT_COMMITTEES))
                {
                    await userManager.RemoveFromRoleAsync(user, GlobalConstants.EXPERT_COMMITTEES);
                }
            }
            //EXTERNAL_EXPERTS
            if (resultContext.ResultContextObject.IsExternalExpert)
            {
                if (!roles.Contains(GlobalConstants.EXTERNAL_EXPERTS))
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.EXTERNAL_EXPERTS);
                }
            }
            else
            {
                if (roles.Contains(GlobalConstants.EXTERNAL_EXPERTS))
                {
                    await userManager.RemoveFromRoleAsync(user, GlobalConstants.EXTERNAL_EXPERTS);
                }
            }

            //EXPERT_DOS
            if (resultContext.ResultContextObject.IsDOCExpert)
            {
                if (!roles.Contains(GlobalConstants.EXPERT_DOS))
                {
                    await userManager.AddToRoleAsync(user, GlobalConstants.EXPERT_DOS);
                }
            }
            else
            {
                if (roles.Contains(GlobalConstants.EXPERT_DOS))
                {
                    await userManager.RemoveFromRoleAsync(user, GlobalConstants.EXPERT_DOS);
                }
            }


            user.Email = resultContext.ResultContextObject.Email;
            if (resultContext.ResultContextObject.Email != null)
            {
                user.NormalizedEmail = resultContext.ResultContextObject.Email.ToUpper();
            }
            user.IdUserStatus = resultContext.ResultContextObject.IdUserStatus;

            this.repository.Update(user);

            var result = await this.repository.SaveChangesAsync();
            outputContext.AddMessage("Записът е успешен!");


            return outputContext;


        }
        public async Task<ResultContext<ApplicationRoleVM>> UpdateApplicationRoleAsync(ApplicationRoleVM applicationRoleVM)
        {
            ResultContext<ApplicationRoleVM> outputContext = new ResultContext<ApplicationRoleVM>();




            var role = await this.roleManager.FindByIdAsync(applicationRoleVM.Id);

            if (role != null)
            {
                role.Name = applicationRoleVM.Name;
                role.RoleName = applicationRoleVM.RoleName;
                role.IdModifyUser = this.UserProps.UserId;
                role.ModifyDate = DateTime.Now;


                await this.roleManager.UpdateAsync(role);

                var Claims = await this.roleManager.GetClaimsAsync(role);

                foreach (var claim in applicationRoleVM.RoleClaims)
                {
                    if (!Claims.Any(c => c.Type == claim.Type))
                    {
                        await this.roleManager.AddClaimAsync(role, new System.Security.Claims.Claim(claim.Type, string.Empty) { });
                    }

                }

                foreach (var removeClaim in Claims)
                {
                    var isRemoved = applicationRoleVM.RoleClaims.Where(x => x.Type == removeClaim.Type).FirstOrDefault();

                    if (isRemoved == null)
                    {
                        await this.roleManager.RemoveClaimAsync(role, removeClaim);
                    }
                }
            }



            //try
            //{
            //    var entity = await this.repository.GetByIdAsync<ApplicationRole>(applicationRoleVM.Id);
            //    this.repository.Detach<ApplicationRole>(entity);

            //    applicationRoleVM.ModifyDate = DateTime.Now;
            //    applicationRoleVM.IdModifyUser = Users.UserId;

            //    entity = applicationRoleVM.To<ApplicationRole>();

            //    this.repository.Update<ApplicationRole>(entity);
            //    await this.repository.SaveChangesAsync();
            //    this.repository.Detach<ApplicationRole>(entity);

            //    outputContext.AddMessage("Записът е успешен!");
            //    outputContext.ResultContextObject = applicationRoleVM;
            //}
            //catch (Exception ex)
            //{
            //    outputContext.AddErrorMessage(ex.Message);
            //}

            return outputContext;
        }

        public async Task<ResultContext<ApplicationRoleVM>> RemoveRoleClaims(ResultContext<ApplicationRoleVM> resultContext)
        {
            ResultContext<ApplicationRoleVM> outputContext = new ResultContext<ApplicationRoleVM>();

            var role = await this.roleManager.FindByIdAsync(resultContext.ResultContextObject.Id);




            foreach (var claim in resultContext.ResultContextObject.RoleClaimsForRemove)
            {
                await this.roleManager.RemoveClaimAsync(role, new System.Security.Claims.Claim(type: claim.Type, value: String.Empty));
            }

            return outputContext;
        }

        public async Task<IEnumerable<ApplicationRoleVM>> GetAllRolesExceptAsync(ApplicationUserVM applicationUserVM)
        {
            List<ApplicationRoleVM> applicationRoles = new List<ApplicationRoleVM>();

            var roles = await roleManager.Roles.ToListAsync();


            foreach (var applicationRole in roles)
            {
                if (!applicationUserVM.Roles.Any(r => r.Id == applicationRole.Id))
                {
                    applicationRoles.Add(new ApplicationRoleVM()
                    {
                        Id = applicationRole.Id,
                        Name = applicationRole.Name,
                        RoleName = applicationRole.RoleName
                    });
                }


            }

            return applicationRoles;



        }

        public async Task<ResultContext<ApplicationUserVM>> RemoveUserRoles(ResultContext<ApplicationUserVM> resultContext, List<ApplicationRoleVM> removeRoles)
        {


            ResultContext<ApplicationUserVM> outputContext = new ResultContext<ApplicationUserVM>();

            var username = await userManager.FindByNameAsync(resultContext.ResultContextObject.UserName);
            if (username.Person != null)
            {
                this.repository.Detach<Person>(username.Person);
            }

            foreach (var role in removeRoles)
            {
                await userManager.RemoveFromRoleAsync(username, role.Name);
                if (username.Person != null)
                {
                    this.repository.Detach<Person>(username.Person);
                }
            }

            outputContext.ResultContextObject = await GetAllApplicationRolePerUserAsync(resultContext.ResultContextObject);

            return outputContext;
        }

        public async Task<List<ApplicationUserVM>> GetAllApplicationUsersAsync()
        {
            var users = this.repository.AllReadonly<ApplicationUser>();

            var result = users.To<ApplicationUserVM>();

            return await result.ToListAsync();
        }


        public async Task<ApplicationUserVM> GetApplicationUsersById(string id)
        {
            var user = await userManager.FindByIdAsync(id);

            return user.To<ApplicationUserVM>();
        }
        public async Task<ApplicationUserVM> GetApplicationUsersAndPersonById(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            var person = await this.repository.GetByIdAsync<Person>((int)user.IdPerson);
            ApplicationUserVM аpplicationUserVM = user.To<ApplicationUserVM>();
            аpplicationUserVM.FirstName = person?.FirstName;
            аpplicationUserVM.FamilyName = person?.FamilyName;
            аpplicationUserVM.Phone = person?.Phone;
            аpplicationUserVM.Email = person?.Email;
            аpplicationUserVM.MiddleName = person?.SecondName;
            аpplicationUserVM.IndentTypeName = person?.Indent;
            аpplicationUserVM.IdPerson = person.IdPerson;
            аpplicationUserVM.Person = person.To<PersonVM>();
            аpplicationUserVM.IdIndentType = person.IdIndentType;

            return аpplicationUserVM;
        }

        public async Task<string> GetApplicationUsersPersonNameAsync(int userID)
        {
            if (userID == 0)
            {
                return string.Empty;
            }
            var applicationUsers = this.repository.AllReadonly<ApplicationUser>(FilterApplicationUser(new ApplicationUserVM() { IdUser = userID })).Include(u => u.Person).AsNoTracking();

            var user = applicationUsers.FirstOrDefault();

            if (user != null)
            {
                return user.Person != null ? user.Person.FirstName + " " + user.Person.FamilyName : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<string> GetPersonTitleAsync(int userID)
        {
            if (userID == 0)
            {
                return string.Empty;
            }
            var applicationUser = this.repository.AllReadonly<ApplicationUser>(x => x.IdUser == userID).Include(u => u.Person).FirstOrDefault();

            if (applicationUser != null)
            {
                return applicationUser.Person != null ? applicationUser.Person.Position : string.Empty;
            }
            else
            {
                return string.Empty;
            }
        }
        public async Task SendPassword(string id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id);

                var password = ApplicationUserService.GenerateRandomPassword();

                var token = await userManager.GeneratePasswordResetTokenAsync(user);

                this.repository.Detach(user);

                await userManager.ResetPasswordAsync(user, token, password);

                ResultContext<ApplicationUserVM> resultContextVM = new ResultContext<ApplicationUserVM>();

                resultContextVM.ResultContextObject = user.To<ApplicationUserVM>();

                resultContextVM.ResultContextObject.Password = password;

                await MailService.SendUserNewPass(resultContextVM);
            }
            catch (Exception e)
            { }
        }      
        public async Task<ResultContext<CandidateProviderLicenceChangeVM>> UpdateApplicationStatusUserAsync(List<ApplicationUser> activeApplicationUsersList, string ActiveOrUnAktive, int kvStatusUser)
        {
            ResultContext<CandidateProviderLicenceChangeVM> result = new ResultContext<CandidateProviderLicenceChangeVM>();
            try
            {
                 foreach (var user in activeApplicationUsersList)
                    {
                        user.IdUserStatus = kvStatusUser;
                        user.IdModifyUser = this.UserProps.UserId;
                        user.ModifyDate= DateTime.Now;
                        user.Person = null;
                        this.repository.Update<ApplicationUser>(user);
                    }
                await this.repository.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                result.AddErrorMessage("Грешка при промяна на статуса на потребителя в базата!");
            }
            return result;
        }
    }


        public class PasswordPolicySettingsParameters
        {
            public int MinimumPasswordLength { get; set; }
            public int MinimumCapitalLetters { get; set; }
            public int MinimumLowLetters { get; set; }
            public int MinimumNumbers { get; set; }
            public int MinimumSpecialCharacters { get; set; }
            public string SpecialCharacters { get; set; }

            public PasswordPolicySettingsParameters()
            {
            }

            public static List<IdentityError> DoValidateNewPassword(string newPassword, PasswordPolicySettingsParameters settingParams)
            {
                Dictionary<string, int> symbolCount = new Dictionary<string, int>();
                List<IdentityError> errors = new List<IdentityError>();
                symbolCount.Add("specialChars", 0);
                symbolCount.Add("capitalLetters", 0);
                symbolCount.Add("numbers", 0);
                for (int i = 0; i < newPassword.Length; i++)
                {
                    if (settingParams.SpecialCharacters.Contains(newPassword[i]))
                    {
                        symbolCount["specialChars"]++;
                    }
                    if ((int)newPassword[i] >= 48 && (int)newPassword[i] <= 57)
                    {
                        symbolCount["numbers"]++;
                    }
                    if ((int)newPassword[i] >= 65 && (int)newPassword[i] <= 90)
                    {
                        symbolCount["capitalLetters"]++;
                    }
                }
           
                if (symbolCount["specialChars"] < settingParams.MinimumSpecialCharacters)
                if (settingParams.MinimumSpecialCharacters == 1)
                {
                    errors.Add(new IdentityError()
                    {

                        Code = "PasswordRequiresUniqueChars",
                        Description = $"Паролата трябва да съдържа поне 1 специален символ! ({settingParams.SpecialCharacters})"
                    });
                }
                else
                {
                    errors.Add(new IdentityError()
                    {

                        Code = "PasswordRequiresUniqueChars",
                        Description = $"Паролата трябва да съдържа поне {settingParams.MinimumSpecialCharacters} специални символа! ({settingParams.SpecialCharacters})"
                    });
                }
                if (symbolCount["numbers"] < settingParams.MinimumNumbers)
                    errors.Add(new IdentityError()
                    {
                        Code = "PasswordMinimumNumbers",
                        Description = settingParams.MinimumNumbers == 1 ? $"Паролата трябва да съдъража поне {settingParams.MinimumNumbers} цифра!" : $"Паролата трябва да съдъража поне {settingParams.MinimumNumbers} цифри!"
                    });
                if (symbolCount["capitalLetters"] < settingParams.MinimumCapitalLetters)
                    errors.Add(new IdentityError()
                    {
                        Code = "PasswordMinimumCapitalLetters",
                        Description = settingParams.MinimumCapitalLetters == 1 ? $"Паролата трябва да съдържа поне {settingParams.MinimumCapitalLetters} главна буква!" : $"Паролата трябва да съдъража поне {settingParams.MinimumCapitalLetters} главни букви!"
                    });
                if (newPassword.Length < settingParams.MinimumPasswordLength)
                    errors.Add(new IdentityError()
                    {
                        Code = "PasswordMinimumPasswordLength",
                        Description = $"Паролата трябва да е поне {settingParams.MinimumPasswordLength} символа!"
                    });

                return errors;
            }

            public static PasswordPolicySettingsParameters GetPasswordPolicySettingsParameters()
            {
                return new PasswordPolicySettingsParameters()
                {
                    MinimumCapitalLetters = 2,
                    MinimumLowLetters = 2,
                    MinimumPasswordLength = 10,
                    MinimumNumbers = 2,
                    SpecialCharacters = "~@#$%^&*_!?|~^/+;",
                    MinimumSpecialCharacters = 2
                };
            }

            public PasswordPolicySettingsParameters(int minLength, int minCaps = 0, int minLows = 0, int minNumbers = 0, int minSpecials = 0, string specials = "")
            {
                MinimumPasswordLength = minLength;
                MinimumCapitalLetters = minCaps;
                MinimumLowLetters = minLows;
                MinimumNumbers = minNumbers;
                MinimumSpecialCharacters = minSpecials;
                SpecialCharacters = specials;
            }      
    }
}
