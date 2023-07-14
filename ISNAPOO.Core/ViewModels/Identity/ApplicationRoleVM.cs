using Data.Models.Data.Framework;
using Data.Models.Data.Role;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Identity
{
    public class ApplicationRoleVM : IMapTo<ApplicationRole>, IMapFrom<ApplicationRole>, IModifiable
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string RoleName { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }


        public List<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

        public List<RoleClaim> RoleClaimsForRemove { get; set; } = new List<RoleClaim>();
        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion
    }

    public class RoleClaim 
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
