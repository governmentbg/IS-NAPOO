using Data.Models.Data.DOC;
using Data.Models.Data.Training;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.ViewModels.DOC;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationCurriculumERUVM : IMapFrom<ValidationCurriculumERU>
    {
        public int IdValidationCurriculumERU { get; set; }

        [Required]
        [Display(Name = "Връзка с Учебна програма за курс за валидиране")]
        [ForeignKey(nameof(ValidationCurriculum))]
        public int IdValidationCurriculum { get; set; }

        public ValidationCurriculumVM ValidationCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка с ЕРУ")]
        [ForeignKey(nameof(ERU))]
        public int IdERU { get; set; }

        public ERUVM ERU { get; set; }
    }
}
