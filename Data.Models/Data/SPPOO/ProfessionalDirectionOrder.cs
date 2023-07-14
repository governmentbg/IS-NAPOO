using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{


    /// <summary>
    /// Професионално направление връзка със Заповед
    /// </summary>
    [Table("SPPOO_ProfessionalDirectionOrder")]
    [Display(Name = "Професионално направление връзка със Заповед")]
    public class ProfessionalDirectionOrder
    {
        [Key]
        public int IdProfessionalDirectionOrder { get; set; }
        public int IdEntity => IdProfessionalDirectionOrder;

        [Required]
        [Display(Name = "Професионално направлени")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int IdProfessionalDirection { get; set; }
        public ProfessionalDirection ProfessionalDirection { get; set; }

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
