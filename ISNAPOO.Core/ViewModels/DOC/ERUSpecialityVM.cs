using ISNAPOO.Core.ViewModels.SPPOO;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.DOC
{
    public class ERUSpecialityVM
    {
        [Key]
        public int IdERUSpeciality { get; set; }

        [Required]
        [Display(Name = "ЕРУ")]
        public int IdERU { get; set; }

        public ERUVM ERU { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        public int IdSpeciality { get; set; }

        public SpecialityVM Speciality { get; set; }
    }
}
