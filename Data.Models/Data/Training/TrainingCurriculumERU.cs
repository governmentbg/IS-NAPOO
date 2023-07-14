using Data.Models.Data.DOC;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка учебна програма с ЕРУ за курс
    /// </summary>
    [Table("Training_CurriculumERU")]
    [Display(Name = "Връзка учебна програма с ЕРУ за курс ")]
    public class TrainingCurriculumERU : IEntity
    {
        public TrainingCurriculumERU()
        {

        }

        [Key]
        public int IdTrainingCurriculumERU { get; set; }
        public int IdEntity => IdTrainingCurriculumERU;

        [Required]
        [Display(Name = "Връзка с Учебна програма")]
        [ForeignKey(nameof(TrainingCurriculum))]
        public int IdTrainingCurriculum { get; set; }
        public TrainingCurriculum TrainingCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка с ЕРУ")]
        [ForeignKey(nameof(ERU))]
        public int IdERU { get; set; }
        public ERU ERU { get; set; }


    }
}
