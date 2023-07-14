using Data.Models.Data.Common;
using ISNAPOO.Core.ViewModels.Common;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Contracts.Common
{
    public interface IMenuNodeService : IBaseService
    {
        Task<IEnumerable<MenuNodeVM>> GetAllMenuNodesAsync();

        Task<int> CreateMenuNodeAsync(MenuNodeVM menuNodeVM);

        Task<int> UpdateMenuNodeAsync(MenuNodeVM menuNodeVM);

        Task<int> DeleteMenuNodeAsync(int id);

        Task<MenuNodeVM> GetMenuNodeByIdAsync(int id);

        Task<List<MenuNodeVM>> GetAllMenuNodesRootsAsync();

        Task<List<MenuNodeVM>> GetAllParentsByRootIdAsync(int id);

        Task<IEnumerable<MenuNodeVM>> GetAllMenuNodesByUserAsync(System.Security.Claims.ClaimsPrincipal identity);

        Task<List<MenuNodeRoleVM>> getAllMenuNodeRolesAsync();

        Task<string> SaveChangesAsync(List<MenuNodesTreeGridDataVM> change,string ApplicationRoleId);
        Task<IEnumerable<MenuNodeVM>> GetAllMenuNodesAsyncByURL(string URL);
    }
}
