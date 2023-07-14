using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// Специалност връзка с CPO,CIPO - Обучаваща институция
    /// </summary>
    [Table("Arch_Candidate_ProviderSpeciality")]
    [Display(Name = "Архив - Специалност връзка с CPO, CIPO - Обучаваща институция")]
    public class ArchCandidateProviderSpeciality : IEntity, IModifiable, IDataMigration
    {
        public ArchCandidateProviderSpeciality()
        {
            this.CandidateCurriculums = new HashSet<ArchCandidateCurriculum>();
        }


        [Key]
        public int IdArchCandidateProviderSpeciality { get; set; }

        public int IdEntity => IdArchCandidateProviderSpeciality;

        [Display(Name = "АРХИВ Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(ArchCandidateProvider))]
        public int IdArchCandidateProvider { get; set; }
        public ArchCandidateProvider ArchCandidateProvider { get; set; }


        public int IdCandidateProviderSpeciality { get; set; } //ArchCandidateProviderIdArchCandidateProvider


        [Required]
        [Display(Name = "CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Required]
        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }


      
        [Comment( "Рамкова програма")]
        [ForeignKey(nameof(FrameworkProgram))]
        public int? IdFrameworkProgram { get; set; }
        public FrameworkProgram FrameworkProgram { get; set; }

      
        [Comment( "Форма на обучение")]
        public int? IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална



        [Comment("Дата на получаване на лицензия за специалността")]
        public DateTime? LicenceData { get; set; }


        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на протокол/заповед за лицензиране на специалността' не може да съдържа повече от 20 символа.")]
        [Comment("Номер на протокол/заповед за лицензиране на специалността")]
        public string? LicenceProtNo { get; set; }


        public virtual ICollection<ArchCandidateCurriculum> CandidateCurriculums { get; set; }

        


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
