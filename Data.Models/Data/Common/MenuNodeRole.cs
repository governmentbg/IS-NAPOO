using Data.Models.Data.Role;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// Експерт
    /// </summary>
    [Table("MenuNodeRole")]
    [Display(Name = "Връзка между роля и меню")]
    public class MenuNodeRole
    {

        [Key]
        public int IdMenuNodeRole { get; set; }
        public int IdEntity => IdMenuNodeRole;

        [Comment("Връзка с Роля")]
        [ForeignKey(nameof(ApplicationRole))]
        public string IdApplicationRole { get; set; }
        public ApplicationRole ApplicationRole { get; set; }


        [Comment("Връзка с елемент от менюто")]
        [ForeignKey(nameof(MenuNode))]
        public int IdMenuNode { get; set; }
        public MenuNode MenuNode { get; set; }
    }
}
