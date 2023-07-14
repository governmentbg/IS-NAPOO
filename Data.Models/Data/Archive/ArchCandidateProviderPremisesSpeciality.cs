using System;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Data.Models.Data.Candidate;

namespace Data.Models.Data.Archive
{


    /// <summary>
    /// АРХИВ - Връзка на материална база и специалности
    /// </summary>
    [Table("Arch_Candidate_ProviderPremisesSpeciality")]
    [Display(Name = "Връзка на материална база и специалности")]
    public class ArchCandidateProviderPremisesSpeciality : IEntity, IModifiable
    {
        public ArchCandidateProviderPremisesSpeciality()
        {

        }

        [Key]
        public int IdArchCandidateProviderPremisesSpeciality { get; set; }

        public int IdEntity => IdArchCandidateProviderPremisesSpeciality;
        public int IdCandidateProviderPremisesSpeciality { get; set; }

        [Required]
        [Comment("АРХИВ - Метериална техническа база")]
        [ForeignKey(nameof(ArchCandidateProviderPremises))]
        public int IdArchCandidateProviderPremises { get; set; }
        public ArchCandidateProviderPremises ArchCandidateProviderPremises { get; set; }

        [Comment("Метериална техническа база")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int? IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremises? CandidateProviderPremises { get; set; }


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
        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}