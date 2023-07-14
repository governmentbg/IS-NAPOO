    using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{
    /// <summary>
    /// Квалификация връзка с Преподавател"
    /// </summary>
    [Table("Candidate_ProviderTrainerQualification")]
    [Display(Name = "Квалификация връзка с Преподавател")]
    public class CandidateProviderTrainerQualification : IEntity, IModifiable, IDataMigration
    {
        public CandidateProviderTrainerQualification()
        {

        }

        [Key]
        public int IdCandidateProviderTrainerQualification { get; set; }
        public int IdEntity => IdCandidateProviderTrainerQualification;

        [Display(Name = "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdCandidateProviderTrainer { get; set; }
        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }


        [Required]
        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Наименование на курса' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Наименование на курса")]
        public string QualificationName { get; set; }


        /// <summary>        
        ///обучение по ключови компетентности
        ///обучение, свързано с преподаваната професия
        ///повишаване на педагогическата квалификация
        ///повишаване на андрагогическата квалификация
        ///обучение, свързано с информиране и професионално ориентиране
        /// </summary>

        [Display(Name = "Вид на курса")]        
        public int IdQualificationType { get; set; }


        [Display(Name = "Професия, по която е проведено обучението")]
        [ForeignKey(nameof(Profession))]
        public int? IdProfession { get; set; }
        public virtual Profession Profession { get; set; }

        /// <summary>
        /// Вътрешнофирмено обучение
        /// Външно обучение
        /// </summary>
        [Display(Name = "Вид на обучението")]
        public int IdTrainingQualificationType { get; set; }

        [Display(Name = "Продължителност (в часове)")]
        public int? QualificationDuration { get; set; }

        [Display(Name = "Дата на провеждане на обучението от")]
        public DateTime? TrainingFrom { get; set; }

        [Display(Name = "Дата на провеждане на обучението до")]
        public DateTime? TrainingTo { get; set; }

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