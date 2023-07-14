using System;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Връзка на материална база и специалности
    /// </summary>
    [Table("Candidate_ProviderPremisesSpeciality")]
    [Display(Name = "Връзка на материална база и специалности")]
    public class CandidateProviderPremisesSpeciality : IEntity, IModifiable
    {
        public CandidateProviderPremisesSpeciality()
        {

        }

        [Key]
        public int IdCandidateProviderPremisesSpeciality { get; set; }
        public int IdEntity => IdCandidateProviderPremisesSpeciality;

        [Required]
        [Comment("Метериална техническа база")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremises CandidateProviderPremises { get; set; }


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
        /// Да
        /// Не
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