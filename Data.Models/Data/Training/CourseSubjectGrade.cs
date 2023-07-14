using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Оценка по предмет от курс за обучение на курсист
    /// </summary>
    [Table("Training_CourseSubjectGrade")]
    [Comment("Оценка по предмет за даден курсист")]
    public class CourseSubjectGrade : IEntity, IModifiable
    {
        [Key]
        public int IdCourseSubjectGrade { get; set; }
        public int IdEntity => IdCourseSubjectGrade;

        [Required]
        [Comment("Връзка с Получател на услугата(обучаем) връзка с курс")]
        [ForeignKey(nameof(ClientCourse))]
        public int IdClientCourse { get; set; }

        public ClientCourse ClientCourse { get; set; }

        [Comment("Предмет от програма за обучение към курс")]
        [ForeignKey(nameof(CourseSubject))]
        public int? IdCourseSubject { get; set; }

        public CourseSubject CourseSubject { get; set; }

        [Display(Name = "Оценка по предмет за теория")]
        [Comment("Оценка по предмет за теория")]
        public double? TheoryGrade { get; set; }

        [Display(Name = "Оценка по предмет за практика")]
        [Comment("Оценка по предмет за практика")]
        public double? PracticeGrade { get; set; }

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
