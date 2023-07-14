using Data.Models.Data.DOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.SPPOO
{


    /// <summary>
    /// Специалност връзка със NKPD
    /// </summary>
    [Table("SPPOO_SpecialityNKPD")]
    [Display(Name = "Специалност връзка с  NKPD")]
    public class SpecialityNKPD
    {

        [Key]
        public int IdSpecialityNKPD { get; set; }
        public int IdEntity => IdSpecialityNKPD;

        [Required]
        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }


        [Required]
        [Display(Name = "НКПД")]
        [ForeignKey(nameof(NKPD))]
        public int IdNKPD { get; set; }
        public NKPD NKPD { get; set; }
    }
}
