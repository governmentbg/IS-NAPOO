using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderTrainerQualificationVM
    {
        public int IdCandidateProviderTrainerQualification { get; set; }

        [Display(Name = "Връзка с Преподавател")]
        public int IdCandidateProviderTrainer { get; set; }

        public virtual CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на курса' е задължително!")]
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

        public string QualificationTypeName { get; set; }

        [Display(Name = "Професия, по която е проведено обучението")]
        public int? IdProfession { get; set; }

        public virtual ProfessionVM Profession { get; set; }

        public string ProfessionName { get; set; }

        /// <summary>
        /// Вътрешнофирмено обучение
        /// Външно обучение
        /// </summary>
        [Display(Name = "Вид на обучението")]
        public int IdTrainingQualificationType { get; set; }
        public KeyValueVM? TrainingQualificationType { get; set; }
        public string TrainingQualificationTypeName { get; set; }

        [Display(Name = "Продължителност (в часове)")]
        public int? QualificationDuration { get; set; }

        [Display(Name = "Дата на провеждане на обучението от")]
        public DateTime? TrainingFrom { get; set; }

        [Display(Name = "Дата на провеждане на обучението до")]
        public DateTime? TrainingTo { get; set; }

        public string TrainingFromAsStr => this.TrainingFrom.HasValue ? $"{this.TrainingFrom.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        public string TrainingToAsStr => this.TrainingTo.HasValue ? $"{this.TrainingTo.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;


        public int IdForQualificationModal { get; set; }

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
