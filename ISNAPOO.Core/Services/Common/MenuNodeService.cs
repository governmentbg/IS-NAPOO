using Data.Models.Common;
using Data.Models.Data.Common;
using ISNAPOO.Core.Contracts.Common;
using ISNAPOO.Core.HelperClasses;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace ISNAPOO.Core.Services.Common
{
    public class MenuNodeService : BaseService, IMenuNodeService
    {
        private readonly IRepository repository;

        public MenuNodeService(IRepository repository, AuthenticationStateProvider authenticationStateProvider)
            : base(repository)
        {
            this.repository = repository;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<int> CreateMenuNodeAsync(MenuNodeVM menuNodeVM)
        {
           
            if (menuNodeVM.QueryParams == null)
            {
                menuNodeVM.QueryParams = string.Empty;
            }
            if (menuNodeVM.Target == null)
            {
                menuNodeVM.Target = string.Empty;
            }
            if (menuNodeVM.CssClass == null)
            {
                menuNodeVM.CssClass = string.Empty;
            }
            if (menuNodeVM.CssClassIcon == null)
            {
                menuNodeVM.CssClassIcon = string.Empty;
            }

            MenuNode menuNode = menuNodeVM.To<MenuNode>();
            await this.repository.AddAsync<MenuNode>(menuNode);

            return await this.repository.SaveChangesAsync();
        }

        public async Task<int> DeleteMenuNodeAsync(int id)
        {
            MenuNode menuNode = await this.repository.GetByIdAsync<MenuNode>(id);

            if (menuNode != null)
            {
                await this.repository.HardDeleteAsync<MenuNode>(id);

                return await this.repository.SaveChangesAsync();
            }

            return 0;
        }

        public async Task<IEnumerable<MenuNodeVM>> GetAllMenuNodesByUserAsync(System.Security.Claims.ClaimsPrincipal user)
        {
            //var state = await authenticationStateProvider.GetAuthenticationStateAsync();
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            // IQueryable<MenuNode> menuNodes = this.repository.All<MenuNode>();
            IQueryable<MenuNode> menuNodes =
                                this.repository
                                .All<MenuNode>()
                                .Include(r => r.MenuNodeRoles)
                                .Where(mr => mr.MenuNodeRoles.Any(c => roles.Contains(c.ApplicationRole.Name)));

            return await menuNodes.To<MenuNodeVM>().ToListAsync();
        }

        public async Task<IEnumerable<MenuNodeVM>> GetAllMenuNodesAsync()
        {
            IQueryable<MenuNode> menuNodes = this.repository.All<MenuNode>();

            return await menuNodes.To<MenuNodeVM>().ToListAsync();
        }


        public async Task<IEnumerable<MenuNodeVM>> GetAllMenuNodesAsyncByURL(string URL)
        {
            IQueryable<MenuNode> menuNodes = this.repository.All<MenuNode>().Where(x=>x.URL == URL);

            return await menuNodes.To<MenuNodeVM>().ToListAsync();
        }

        protected Expression<Func<MenuNode, bool>> FilterRootsByRole(System.Security.Claims.ClaimsPrincipal user)
        {
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);

            var predicate = PredicateBuilder.True<MenuNode>();

            // predicate = predicate.And(n => n.MenuNodeRoles.Any());

            return predicate;
        }

        public async Task<List<MenuNodeVM>> GetAllMenuNodesRootsAsync()
        {
            IQueryable<MenuNode> menuNodes = this.repository.All<MenuNode>(this.FilterRootsById());

            return await menuNodes.To<MenuNodeVM>().ToListAsync();
        }

        public async Task<List<MenuNodeVM>> GetAllParentsByRootIdAsync(int id)
        {
            IQueryable<MenuNode> menuNodes = this.repository.All<MenuNode>(this.FilterParentsByRootId(id));

            return await menuNodes.To<MenuNodeVM>().ToListAsync();
        }

        public async Task<MenuNodeVM> GetMenuNodeByIdAsync(int id)
        {
            MenuNode menuNode = await this.repository.GetByIdAsync<MenuNode>(id);

            if (menuNode != null)
            {
                await Task.Run(() => this.repository.Detach<MenuNode>(menuNode));
                return menuNode.To<MenuNodeVM>();
            }

            return null;
        }

        public async Task<int> UpdateMenuNodeAsync(MenuNodeVM menuNodeVM)
        {
            MenuNode menuNode = menuNodeVM.To<MenuNode>();
            this.repository.Update<MenuNode>(menuNode);
            return await this.repository.SaveChangesAsync();
        }

        protected Expression<Func<MenuNode, bool>> FilterRootsById()
        {
            var predicate = PredicateBuilder.True<MenuNode>();

            predicate = predicate.And(n => n.IdParentNode == 0);

            return predicate;
        }

        protected Expression<Func<MenuNode, bool>> FilterParentsByRootId(int id)
        {
            var predicate = PredicateBuilder.True<MenuNode>();

            predicate = predicate.And(n => n.IdParentNode == id);

            return predicate;
        }

        public async Task<List<MenuNodeRoleVM>> getAllMenuNodeRolesAsync()
        {
            var data = this.repository.AllReadonly<MenuNodeRole>();

            return await data.To<MenuNodeRoleVM>(x => x.ApplicationRole).ToListAsync();
        }

        public async Task<string> SaveChangesAsync(List<MenuNodesTreeGridDataVM> change, string ApplicationRoleId)
        {
            string msg = string.Empty;
            try
            {
                var roleAuthentication = this.repository.All<MenuNodeRole>().Where(x => x.ApplicationRole.Id.Equals(ApplicationRoleId)).ToList();

                foreach (var entity in change)
                {
                    var isInDB = roleAuthentication.Where(x => x.IdMenuNode == entity.EntityId).FirstOrDefault();

                    if (isInDB == null)
                    {
                        MenuNodeRole record = new MenuNodeRole();
                        record.IdApplicationRole = ApplicationRoleId;
                        record.IdMenuNode = entity.EntityId;

                        await this.repository.AddAsync<MenuNodeRole>(record);
                    }
                }

                var result = roleAuthentication.Where(p => !change.Any(l => p.IdMenuNode == l.EntityId));

                if (result.Any())
                {
                    this.repository.HardDeleteRange<MenuNodeRole>(result);
                }

                await this.repository.SaveChangesAsync();

                msg = "Записът е успешен!";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }

            return msg;
        }
    }
}
