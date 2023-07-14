using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{


    /// <summary>
    /// Професия връзка със Заповед
    /// </summary>
    [Table("SPPOO_ProfessionOrder")]
    [Display(Name = "Професия връзка със Заповед")]
    public class ProfessionOrder
    {
        [Key]
        public int IdProfessionOrder { get; set; }
        public int IdEntity => IdProfessionOrder;

        [Required]
        [Display(Name = "Професия")]
        [ForeignKey(nameof(Profession))]
        public int IdProfession { get; set; }
        public Profession Profession { get; set; }

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
