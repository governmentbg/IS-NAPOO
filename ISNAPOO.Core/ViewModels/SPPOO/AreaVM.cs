using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ValidationAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class AreaVM
    {
        [Key]
        public int IdArea { get; set; }

        [Required(ErrorMessage = "Полето 'Код' е задължително!")]
        [StringLength(100)]
        [Display(Name = "Код на областта")]
        [ValidSPPOOCode]
        public string Code { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на областта на образование' е задължително!")]
        [StringLength(255)]
        [Display(Name = "Наименование на областта")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на областта на образование на английски език' е задължително!")]
        [StringLength(255)]
        [Display(Name = "Наименование на областта - англйски")]
        public string NameEN { get; set; }

        public int IdStatus { get; set; }

        public long oldId { get; set; }

        public ICollection<ProfessionalDirectionVM> ProfessionalDirections { get; set; }

        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }
    }
}
