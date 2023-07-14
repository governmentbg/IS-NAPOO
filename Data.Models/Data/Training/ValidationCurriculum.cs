using Data.Models.Data.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;

namespace Data.Models.Data.Training
{
    // <summary>
    /// Учебен план за курс за валидиране
    /// </summary>
    [Table("Training_ValidationCurriculum")]
    [Display(Name = "Учебен план за курс за валидиране")]
    public class ValidationCurriculum : IEntity, IModifiable
    {
        public ValidationCurriculum()
        {
            this.ValidationCurriculumERUs = new HashSet<ValidationCurriculumERU>();
        }

        [Key]
        public int IdValidationCurriculum { get; set; }
        public int IdEntity => IdValidationCurriculum;

        [Required]
        [Display(Name = "Връзка със Специалност")]
        [ForeignKey(nameof(CandidateProviderSpeciality))]
        public int IdCandidateProviderSpeciality { get; set; }

        public CandidateProviderSpeciality CandidateProviderSpeciality { get; set; }

        [Display(Name = "Връзка с Курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        [Required]
        [Display(Name = "Вид професионална подготовка")] //А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }

        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 1000 символа.")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }// Предмет

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Тема' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Тема")]
        public string Topic { get; set; }// Тема

        [Display(Name = "Tеория")]
        public double? Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        public double? Practice { get; set; }//Практика

        public virtual ICollection<ValidationCurriculumERU> ValidationCurriculumERUs { get; set; }

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
