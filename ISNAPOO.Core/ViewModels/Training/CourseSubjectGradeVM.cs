using Data.Models.Data.Training;
using ISNAPOO.Core.Mapping;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CourseSubjectGradeVM : IMapFrom<CourseSubjectGrade>
    {
        public int IdCourseSubjectGrade { get; set; }

        [Required]
        [Comment("Връзка с Получател на услугата(обучаем) връзка с курс")]
        public int IdClientCourse { get; set; }

        public virtual ClientCourseVM ClientCourse { get; set; }

        [Required]
        [Comment("Предмет от програма за обучение към курс")]
        public int IdCourseSubject { get; set; }

        public virtual CourseSubjectVM CourseSubject { get; set; }

        [Display(Name = "Оценка по предмет за теория")]
        [Comment("Оценка по предмет за теория")]
        public double? TheoryGrade { get; set; }

        [Display(Name = "Оценка по предмет за практика")]
        [Comment("Оценка по предмет за практика")]
        public double? PracticeGrade { get; set; }

        public string TheoryGradeAsStr { get; set; }

        public string PracticeGradeAsStr { get; set; }

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
