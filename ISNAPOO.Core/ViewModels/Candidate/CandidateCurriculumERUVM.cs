using ISNAPOO.Core.ViewModels.DOC;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateCurriculumERUVM
    {
        public int IdCandidateCurriculumERU { get; set; }

        [Required]
        [Display(Name = "Връзка с Учебна програма")]
        public int IdCandidateCurriculum { get; set; }
        public CandidateCurriculumVM CandidateCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка с ЕРУ")]
        public int IdERU { get; set; }
        public ERUVM ERU { get; set; }
    }
}
