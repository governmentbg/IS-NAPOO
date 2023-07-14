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

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Програмa за обучение, предлагани от ЦПО  От стара таблица tb_courses
    /// </summary>
    [Table("Training_Program")]
    [Comment("Програмa за обучение, предлагани от ЦПО")]
    public class Program : IEntity, IModifiable, IDataMigration
    {
        public Program()
        {
            this.Courses = new HashSet<Course>();
            this.TrainingCurriculums = new HashSet<TrainingCurriculum>();
        }

        [Key]
        public int IdProgram { get; set; }
        public int IdEntity => IdProgram;

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Номер на програма")]
        public string? ProgramNumber { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        [Comment("Наименование на програма")]
        public string ProgramName { get; set; }

        [Column(TypeName = "ntext")]
        [Comment("Допълнителна информация")]
        public string? ProgramNote { get; set; }


        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Required]
        [Comment("Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }

        [Comment("Рамкова програма")]
        [ForeignKey(nameof(FrameworkProgram))]
        public int? IdFrameworkProgram { get; set; }
        public FrameworkProgram FrameworkProgram { get; set; }

        [Display(Name = "Минимално образователно равнище")]
        [Comment("Минимално образователно равнище от рамкова програма")]
        public int? IdMinimumLevelEducation { get; set; } // Старо 'int_course_educ_requirement' начален етап на основното образование, успешно завършен курс за ограмотяване, Първи гимназиален етап, X клас

        [Required]
        [Comment("Вид на обучение")]
        public int IdCourseType { get; set; }//Валидиране на степен на професионална квалификация, Валидиране на част от професия, Издаване на дубликат, обучение за ключови компетентности, професионално обучение по част от професия и др.


        [Required]
        [Comment("Задължителни учебни ч.(бр.)")]
        public int MandatoryHours { get; set; } = 0;

        [Required]
        [Comment("Избираеми учебни ч.(бр.)")]
        public int SelectableHours { get; set; } = 0;

        [Comment("Дали е записът е изтрит")]
        public bool IsDeleted { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }

        [Comment("Дали записът е служебен(Създаден заради разлика в рамкова програма)")]
        public bool IsService { get; set; }

        public virtual ICollection<Course> Courses { get; set; }

        public virtual ICollection<TrainingCurriculum> TrainingCurriculums { get; set; }

        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion

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

