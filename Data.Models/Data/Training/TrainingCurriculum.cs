using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.Training
{

    // <summary>
    /// Учебен план за курс/програма
    /// </summary>
    [Table("Training_Curriculum")]
    [Display(Name = "Учебен план за курс/програма")]
    public class TrainingCurriculum : IEntity, IModifiable, IDataMigration
    {
        public TrainingCurriculum()
        {
            this.TrainingCurriculumERUs = new HashSet<TrainingCurriculumERU>();
            this.CourseSchedules = new HashSet<CourseSchedule>();
        }

        [Key]
        public int IdTrainingCurriculum { get; set; }
        public int IdEntity => IdTrainingCurriculum;

        [Display(Name = "Връзка със учебен план")]
        [ForeignKey(nameof(CandidateCurriculum))]
        public int? IdCandidateCurriculum { get; set; }
        public CandidateCurriculum CandidateCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка със Специалност")]
        [ForeignKey(nameof(CandidateProviderSpeciality))]
        public int IdCandidateProviderSpeciality { get; set; }
        public CandidateProviderSpeciality CandidateProviderSpeciality { get; set; }

        [Required]
        [Display(Name = "Връзка с Програмa за обучение")]
        [ForeignKey(nameof(Program))]
        public int IdProgram { get; set; }
        public Program Program { get; set; } 


    
        [Display(Name = "Връзка с Курс за обучение")]
        [ForeignKey(nameof(Course))]
        public int? IdCourse { get; set; }
        public Course Course { get; set; }



        [Required]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
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

        [Display(Name = "Подредба")]
        public int? Order { get; set; }//OldIS => int_curric_order

        public virtual ICollection<TrainingCurriculumERU> TrainingCurriculumERUs { get; set; }

        public virtual ICollection<CourseSchedule> CourseSchedules { get; set; }

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
