using Data.Models.Data.SPPOO;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class SpecialityNKPDVM : IMapFrom<SpecialityNKPD>, IMapTo<SpecialityNKPD>
    {
        [Key]
        public int IdSpecialityNKPD { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        public int IdSpeciality { get; set; }
        public SpecialityVM Speciality { get; set; }


        [Required]
        [Display(Name = "НКПД")]
        public int IdNKPD { get; set; }
        public NKPDVM NKPD { get; set; }
    }
}
