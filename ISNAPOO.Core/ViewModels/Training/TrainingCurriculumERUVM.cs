using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.DOC;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class TrainingCurriculumERUVM
    {
        public int IdTrainingCurriculumERU { get; set; }

        [Required]
        [Display(Name = "Връзка с Учебна програма")]
        public int IdTrainingCurriculum { get; set; }

        public virtual TrainingCurriculumVM TrainingCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка с ЕРУ")]
        public int IdERU { get; set; }

        public virtual ERUVM ERU { get; set; }
    }
}
