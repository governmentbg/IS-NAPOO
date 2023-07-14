using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderPremisesSpecialityVM
    {
        public int IdCandidateProviderPremisesSpeciality { get; set; }

        [Required]
        [Comment("Метериална техническа база")]
        public int IdCandidateProviderPremises { get; set; }

        public CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Required]
        [Comment("Връзка с  Специалност")]
        public int IdSpeciality { get; set; }


        /// <summary>
        /// "обучение по теория"
        /// "обучение по практика"
        /// "обучение по теория и практика"
        /// </summary>
        [Required]
        [Comment("Връзка със Теория или Практика")]
        public int IdUsage { get; set; }


        /// <summary>
        /// "Напълно"
        /// "Частично"
        /// "Не съответства
        /// </summary>
        [Comment("Съответствие с DOC")]
        [Required]
        public int IdComplianceDOC { get; set; }
        public KeyValueVM ComplianceDOC { get; set; }

        public string CodeAndName { get; set; }

        public SpecialityVM Speciality { get; set; }

        public int IdCreateUser { get; set; }


        public DateTime CreationDate { get; set; }


        public int IdModifyUser { get; set; }


        public DateTime ModifyDate { get; set; }
    }
}
