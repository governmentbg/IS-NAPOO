using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ConsultingPremisesVM
    {
        public int IdConsultingPremises { get; set; }

        [Display(Name = "Връзка с MTB")]
        public int IdPremises { get; set; }

        public virtual CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

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
