using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;

using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderTrainerSpecialityVM
    {
        public int IdCandidateProviderTrainerSpeciality { get; set; }

        [Comment("Връзка с Преподавател")]
        public int IdCandidateProviderTrainer { get; set; }

        public CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        [Required]
        [Comment("Връзка с  Специалност")]
        public int IdSpeciality { get; set; }


        public SpecialityVM Speciality { get; set; }

        
        /// <summary>
        /// "обучение по теория"
        /// "обучение по практика"
        /// "обучение по теория и практика"
        /// </summary>
        [Comment("Вид на провежданото обучение")]
        [Required]
        public int IdUsage { get; set; }

        /// <summary>
        /// "Напълно"
        /// "Частично"
        /// "Не съответства
        /// </summary>
        [Comment("Съответствие с DOC")]
        [Required]
        public int IdComplianceDOC { get; set; }


        public int IdCreateUser { get; set; }

 
        public DateTime CreationDate { get; set; }

 
        public int IdModifyUser { get; set; }


        public DateTime ModifyDate { get; set; }
    }
}
