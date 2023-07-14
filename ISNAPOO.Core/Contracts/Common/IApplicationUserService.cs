using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Framework;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IApplicationUserService
    {
        Task<ResultContext<ApplicationUserVM>> CreateApplicationUserAsync(ResultContext<ApplicationUserVM> resultContext);
        Task<ResultContext<ApplicationUserVM>> CreateApplicationUserAsAdminAsync(ResultContext<ApplicationUserVM> resultContext);
        Task<ResultContext<ApplicationUserVM>> CreateApplicationExpertUserAsync(ResultContext<ApplicationUserVM> resultContext);
        Task<ResultContext<ApplicationUserVM>> UpdateApplicationUserAsync(ResultContext<ApplicationUserVM> resultContext);
        Task<IEnumerable<ApplicationUserVM>> GetAllApplicationUserAsync(ApplicationUserVM applicationUserVM);

        Task<IEnumerable<ApplicationRoleVM>> GetAllApplicationRoleAsync(ApplicationRoleVM аpplicationRoleVM);
        Task<IEnumerable<ApplicationRoleVM>> GetAllApplicationRoleAsync();
        Task<ApplicationRoleVM> GetApplicationRoleByIdAsync(ApplicationRoleVM applicationRoleVM);
        Task<ResultContext<ApplicationRoleVM>> CreateApplicationRoleAsync(ApplicationRoleVM applicationRoleVM);
        Task<ResultContext<ApplicationRoleVM>> UpdateApplicationRoleAsync(ApplicationRoleVM applicationRoleVM);
        Task<ResultContext<ApplicationUserVM>> UpdateApplicationExpertUserAsync(ResultContext<ApplicationUserVM> resultContext);
        Task<ResultContext<ApplicationRoleVM>> RemoveRoleClaims(ResultContext<ApplicationRoleVM> resultContext);
        Task<ApplicationUserVM> GetAllApplicationRolePerUserAsync(ApplicationUserVM applicationUserVM);
        Task<IEnumerable<ApplicationRoleVM>> GetAllRolesExceptAsync(ApplicationUserVM applicationUserVM);
        Task<ResultContext<ApplicationUserVM>> RemoveUserRoles(ResultContext<ApplicationUserVM> resultContext, List<ApplicationRoleVM> removeRoles);
        Task<string> GetApplicationUsersPersonNameAsync(int userID);
        Task<string> GetPersonTitleAsync(int userID);
        Task SendPassword(string id);
        Task<ApplicationUserVM> GetApplicationUsersById(string id);
        Task<ApplicationUserVM> GetApplicationUsersAndPersonById(string id);
        Task<ResultContext<CandidateProviderLicenceChangeVM>> UpdateApplicationStatusUserAsync(List<ApplicationUser> activeApplicationUsersList, string ActiveOrUnAktived, int kvStatusUser);
    }
}
