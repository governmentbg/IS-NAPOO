using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка между консултант и дейност по консултиране
    /// </summary>
    [Table("Training_ConsultingTrainer")]
    [Comment("Връзка между консултант и дейност по консултиране")]
    public class ConsultingTrainer : IEntity, IModifiable
    {
        [Key]
        public int IdConsultingTrainer { get; set; }
        public int IdEntity => IdConsultingTrainer;

        [Display(Name = "Връзка с Консултант")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdTrainer { get; set; }

        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }

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
