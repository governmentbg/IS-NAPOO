using Data.Models.Data.Assessment;
using Data.Models.Data.DOC;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{
    /// <summary>
    /// Връзка учебна програма с ЕРУ
    /// </summary>
    [Table("Candidate_CurriculumERU")]
    [Display(Name = "Връзка учебна програма с ЕРУ")]
    public class CandidateCurriculumERU : IEntity
    {
        public CandidateCurriculumERU()
        {
            
        }

        [Key]
        public int IdCandidateCurriculumERU { get; set; }
        public int IdEntity => IdCandidateCurriculumERU;

        [Required]
        [Display(Name = "Връзка с Учебна програма")]
        [ForeignKey(nameof(CandidateCurriculum))]
        public int IdCandidateCurriculum { get; set; }
        public CandidateCurriculum CandidateCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка с ЕРУ")]
        [ForeignKey(nameof(ERU))]
        public int IdERU { get; set; }
        public ERU ERU { get; set; }


    }
}
