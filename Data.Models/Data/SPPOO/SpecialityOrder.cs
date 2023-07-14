using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{
    /// <summary>
    /// Специалност връзка със Заповед
    /// </summary>
    [Table("SPPOO_SpecialityOrder")]
    [Display(Name = "Специалност връзка със Заповед")]
    public class SpecialityOrder
    {
        [Key]
        public int IdSpecialityOrder { get; set; }
        public int IdEntity => IdSpecialityOrder;

        [Required]
        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }

        [Required]
        [Display(Name = "Заповед")]
        [ForeignKey(nameof(SPPOOOrder))]
        public int IdSPPOOOrder { get; set; }
        public SPPOOOrder SPPOOOrder { get; set; }

        [Required]
        [Display(Name = "Вид на промяната, KeyType - OrderTypeChange ")]
        public int IdTypeChange { get; set; }//Вписване (създаване),Промяна, Отпадане(заличаване)
    }
}