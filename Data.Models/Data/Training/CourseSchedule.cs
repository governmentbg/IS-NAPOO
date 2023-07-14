using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;

namespace Data.Models.Data.Training
{
    // <summary>
    /// График за провеждане на обучение по тема от учебна програма към курс
    /// </summary>
    [Table("Training_CourseSchedule")]
    [Display(Name = "График за провеждане на обучение по тема от учебна програма към курс")]
    public class CourseSchedule : IEntity, IModifiable
    {
        [Key]
        public int IdCourseSchedule { get; set; }
        public int IdEntity => IdCourseSchedule;

        [Required]
        [Display(Name = "Връзка с тема от учебна програма")]
        [ForeignKey(nameof(TrainingCurriculum))]
        public int IdTrainingCurriculum { get; set; }

        public TrainingCurriculum TrainingCurriculum { get; set; }

        [Display(Name = "Връзка с МТБ")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int? IdCandidateProviderPremises { get; set; }

        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        [Display(Name = "Връзка с преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int? IdCandidateProviderTrainer { get; set; }

        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }

        //Нова номенклатура: Вид на обучение - "TrainingScheduleType" (Теория, Практика)
        [Required]
        [Display(Name = "Вид на провежданото обучение")]
        public int IdTrainingScheduleType { get; set; }

        [Required]
        [Display(Name = "Брой часове на провеждано обучение")]
        public double Hours { get; set; }

        [Required]
        [Display(Name = "Дата на провеждано обучение")]
        public DateTime ScheduleDate { get; set; }

        [Display(Name = "Продължителност от")]
        public DateTime? TimeFrom { get; set; }

        [Display(Name = "Продължителност до")]
        public DateTime? TimeTo { get; set; }

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
