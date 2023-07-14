using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.SPPOO
{


    /// <summary>
    /// Рамкова програма връзка със Форма за обучение
    /// </summary>
    [Table("SPPOO_FrameworkProgramFormEducation")]
    [Display(Name = "Рамкова програма връзка със Форма за обучение")]
    public class FrameworkProgramFormEducation
    {
        [Key]
        public int IdFrameworkProgramFormEducation { get; set; }
        public int IdEntity => IdFrameworkProgramFormEducation;

        [Required]
        [Display(Name = "Рамкова програма")]
        [ForeignKey(nameof(FrameworkProgram))]
        public int IdFrameworkProgram { get; set; }
        public FrameworkProgram FrameworkProgram { get; set; }

        [Required]
        [Display(Name = "Форма на обучение")]
        public int IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална
    }
}
