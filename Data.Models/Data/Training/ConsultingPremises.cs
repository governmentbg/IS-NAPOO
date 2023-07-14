using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка между MTB и консултация по дейност
    /// </summary>
    [Table("Training_ConsultingPremises")]
    [Comment("Връзка между MTB и консултация по дейност")]
    public class ConsultingPremises : IEntity, IModifiable
    {
        [Key]
        public int IdConsultingPremises { get; set; }
        public int IdEntity => IdConsultingPremises;

        [Required]
        [Display(Name = "Връзка с MTB")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdPremises { get; set; }

        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        [Comment("Връзка с консултирано лице")]
        [ForeignKey(nameof(ConsultingClient))]
        public int IdConsultingClient { get; set; }

        public ConsultingClient ConsultingClient { get; set; }

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
