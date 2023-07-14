using System;
using Data.Models.Data.Common;
using Data.Models.Data.Role;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using ISNAPOO.Core.ViewModels.Identity;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class MenuNodeRoleVM
    { 
    
        public string IdApplicationRole { get; set; }
        public ApplicationRoleVM ApplicationRole { get; set; }

        public int IdMenuNode { get; set; }
        public MenuNodeVM MenuNode { get; set; }

       
    }
}

