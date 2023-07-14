using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class MenuNodeVM : IMapFrom<MenuNode>, IMapTo<MenuNode>
    {
        public int IdMenuNode { get; set; }


        [Required(ErrorMessage = "Полето 'Меню' е задължително!")]
        public int? IdParentNode { get; set; }

        /// <summary>
        /// Наименование на елемет от менюто
        /// </summary>
        [Required(ErrorMessage = "Полето 'Наименование' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Наименование' не може да съдържа повече от 100 символа.")]
        public string Name { get; set; }

        /// <summary>
        /// Подредба
        /// </summary>
        [Required(ErrorMessage = "Полето 'Подредба' е задължително!")]
        public int NodeOrder { get; set; }

        /// <summary>
        /// root, parent, link
        /// </summary>
        [Required(ErrorMessage = "Полето 'Тип' е задължително!")]
        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Type' не може да съдържа повече от 10 символа.")]
        public string Type { get; set; }


        /// <summary>
        /// Линк
        /// </summary>
        [Required(ErrorMessage = "Полето 'URL' е задължително!")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'URL' не може да съдържа повече от 512 символа.")]
        public string URL { get; set; }



        /// <summary>
        /// Допълнителни параметри
        /// </summary>
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'QueryParams' не може да съдържа повече от 512 символа.")]
        public string? QueryParams { get; set; }


        /// <summary>
        /// _blank, _parent, _self, _top
        /// </summary>
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Target' не може да съдържа повече от 100 символа.")]
        public string? Target { get; set; }


        /// <summary>
        /// CssClassIcon
        /// </summary>
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'CssClassIcon' не може да съдържа повече от 100 символа.")]
        public string? CssClassIcon { get; set; }


        /// <summary>
        /// CssClass
        /// </summary>
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'CssClass' не може да съдържа повече от 100 символа.")]
        public string? CssClass { get; set; }
    }
}
