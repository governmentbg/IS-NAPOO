using Data.Models.Data.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.DOC;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка учебна програма с ЕРУ за курс за валидиране
    /// </summary>
    [Table("Training_ValidationCurriculumERU")]
    [Display(Name = "Връзка учебна програма за курс за валидиране с ЕРУ")]
    public class ValidationCurriculumERU : IEntity
    {
        [Key]
        public int IdValidationCurriculumERU { get; set; }
        public int IdEntity => IdValidationCurriculumERU;

        [Required]
        [Display(Name = "Връзка с Учебна програма за курс за валидиране")]
        [ForeignKey(nameof(ValidationCurriculum))]
        public int IdValidationCurriculum { get; set; }

        public ValidationCurriculum ValidationCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка с ЕРУ")]
        [ForeignKey(nameof(ERU))]
        public int IdERU { get; set; }

        public ERU ERU { get; set; }
    }
}
