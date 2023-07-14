using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ConsultingTrainerVM
    {
        public int IdConsultingTrainer { get; set; }

        [Display(Name = "Връзка с Консултант")]
        public int IdTrainer { get; set; }

        public virtual CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        [Comment("Връзка с консултирано лице")]
        public int IdConsultingClient { get; set; }

        public virtual ConsultingClientVM ConsultingClient { get; set; }

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
