using System;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Връзка на преподавател и специалности, по които преподава
    /// </summary>
    [Table("Candidate_ProviderTrainerSpeciality")]
    [Display(Name = "Връзка на преподавател и специалности, по които преподава")]
    public class CandidateProviderTrainerSpeciality : IEntity, IModifiable
    {
        public CandidateProviderTrainerSpeciality()
        {

        }

        [Key]
        public int IdCandidateProviderTrainerSpeciality { get; set; }
        public int IdEntity => IdCandidateProviderTrainerSpeciality;

        [Comment( "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdCandidateProviderTrainer { get; set; }
        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }


        [Required]
        [Comment("Връзка с  Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }


        /// <summary>
        /// "обучение по теория"
        /// "обучение по практика"
        /// "обучение по теория и практика"
        /// </summary>
        [Comment("Вид на провежданото обучение")]
        public int IdUsage { get; set; }


        /// <summary>
        /// Напълно
        /// Не съответства
        /// Частично
        /// </summary>
        [Comment("Съответствие с ДОС")]
        public int IdComplianceDOC { get; set; }


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