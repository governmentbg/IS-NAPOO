using Data.Models.Data.SPPOO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.DOC
{ 
    /// <summary>
    /// ЕРУ връзка със Специалност
    /// </summary>
    [Table("DOC_ERUSpeciality")]
    [Display(Name = "ЕРУ връзка със Специалност")]
    public class ERUSpeciality
    {
        [Key]
        public int IdERUSpeciality { get; set; }
        public int IdEntity => IdERUSpeciality;

        [Required]
        [Display(Name = "ЕРУ")]
        [ForeignKey(nameof(ERU))]
        public int IdERU { get; set; }
        public ERU ERU { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }

       
    }
}
