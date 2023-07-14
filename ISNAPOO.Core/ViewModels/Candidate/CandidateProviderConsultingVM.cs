using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderConsultingVM
    {
        public int IdCandidateProviderConsulting { get; set; }

        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        // Номенклатура: KeyTypeIntCode: "ConsultingType"
        [Display(Name = "Връзка със стойност на услуга по консултиране")]
        public int IdConsultingType { get; set; }

        public string ConsultingTypeValue { get; set; }

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
