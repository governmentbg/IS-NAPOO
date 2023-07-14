using Data.Models.Data.Framework;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.Candidate
{
    /// <summary>
    /// CPO,CIPO - Кандидат Обучаваща институция, Заявление
    /// </summary>
    [Table("Candidate_ProviderConsulting")]
    [Display(Name = "Връзка на ЦИПО с услуга по лицензиране")]
    public class CandidateProviderConsulting : IEntity, IModifiable
    {
        [Key]
        public int IdCandidateProviderConsulting { get; set; }
        public int IdEntity => IdCandidateProviderConsulting;

        [Display(Name = "Връзка с ЦИПО")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        // Номенклатура: KeyTypeIntCode: "ConsultingType"
        [Display(Name = "Връзка със стойност на услуга по консултиране")]
        public int IdConsultingType { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion
    }
}
