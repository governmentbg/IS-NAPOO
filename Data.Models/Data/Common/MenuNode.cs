using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{

    /// <summary>
    /// Струкура на менюто
    /// </summary>
    [Table("MenuNode")]
    [Display(Name = "Струкура на менюто")]
    public class MenuNode : IEntity
    {
        public MenuNode()
        {
            this.MenuNodeRoles = new List<MenuNodeRole>();
        }

        [Key]
        public int IdMenuNode { get; set; }
        public int IdEntity => IdMenuNode;

        [Required]
        public int IdParentNode { get; set; }

        /// <summary>
        /// Наименование на елемет от менюто
        /// </summary>
        [Required]
        [StringLength(DBStringLength.StringLength100)]
        public string Name { get; set; }

        /// <summary>
        /// Подредба
        /// </summary>
        [Required]
        public int NodeOrder { get; set; }

        /// <summary>
        /// root, parent, link
        /// </summary>
        [Required]
        [StringLength(DBStringLength.StringLength10)]
        public string Type { get; set; }


        /// <summary>
        /// Линк
        /// </summary>
        [Required]
        [StringLength(DBStringLength.StringLength512)]
        public string URL { get; set; }



        /// <summary>
        /// Допълнителни параметри
        /// </summary>
        
        [StringLength(DBStringLength.StringLength512)]
        public string? QueryParams { get; set; }


        /// <summary>
        /// _blank, _parent, _self, _top
        /// </summary>
        
        [StringLength(DBStringLength.StringLength100)]
        public string? Target { get; set; }


        /// <summary>
        /// CssClassIcon
        /// </summary>
        
        [StringLength(DBStringLength.StringLength100)]
        public string? CssClassIcon { get; set; }


        /// <summary>
        /// CssClass
        /// </summary>
        
        [StringLength(DBStringLength.StringLength100)]
        public string? CssClass { get; set; }


        public virtual ICollection<MenuNodeRole> MenuNodeRoles { get; set; }






    }
}