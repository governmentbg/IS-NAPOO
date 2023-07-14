using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    /// <summary>
    /// Програмa за обучение, предлагани от ЦПО  От стара таблица tb_courses
    /// </summary>
    [Comment("Програмa за обучение, предлагани от ЦПО")]
    public class ProgramVM
    {
        public ProgramVM()
        {
            this.Courses = new HashSet<CourseVM>();
            this.TrainingCurriculums = new HashSet<TrainingCurriculumVM>();
        }

        [Key]
        public int IdProgram { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на програма' може да съдържа повече до 255 символа.")]
        [Comment("Номер на програма")]
        public string? ProgramNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на програмата' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на програма' може да съдържа повече до 255 символа.")]
        [Comment("Наименование на програма")]
        public string ProgramName { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Допълнителна информация' може да съдържа повече до 4000 символа.")]
        [Comment("Допълнителна информация")]
        public string? ProgramNote { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Специалност' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Специалност' е задължително!")]
        [Comment("Специалност")]
        public int IdSpeciality { get; set; }

        public virtual SpecialityVM Speciality { get; set; }

        [Required(ErrorMessage = "Полето 'Рамкова програма' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Рамкова програма' е задължително!")]
        [Comment("Рамкова програма")]
        public int? IdFrameworkProgram { get; set; }

        public virtual FrameworkProgramVM FrameworkProgram { get; set; }

        [Required]
        [Display(Name = "Минимално образователно равнище")]
        [Comment("Минимално образователно равнище от рамкова програма")]
        public int IdMinimumLevelEducation { get; set; } // Старо 'int_course_educ_requirement' начален етап на основното образование, успешно завършен курс за ограмотяване, Първи гимназиален етап, X клас

        [Required]
        [Comment("Вид на обучение")]
        public int IdCourseType { get; set; }//Валидиране на степен на професионална квалификация, Валидиране на част от професия, Издаване на дубликат, обучение за ключови компетентности, професионално обучение по част от професия и др.
        
        public KeyValueVM CourseType { get; set; }

        public string CourseTypeName { get; set; }

        [Required]
        [Comment("Задължителни учебни ч.(бр.)")]
        public int MandatoryHours { get; set; } = 0;

        [Required]
        [Comment("Избираеми учебни ч.(бр.)")]
        public int SelectableHours { get; set; } = 0;

        public int TotalHours => this.MandatoryHours + this.SelectableHours;

        [Comment("Дали е записът е изтрит")]
        public bool IsDeleted { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }

        public bool IsProgramLegalCapacityOrdinance { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        [Comment("Дали записът е служебен(Създаден заради разлика в рамкова програма)")]
        public bool IsService { get; set; }

        public virtual ICollection<CourseVM> Courses { get; set; }

        public virtual ICollection<TrainingCurriculumVM> TrainingCurriculums { get; set; }

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

