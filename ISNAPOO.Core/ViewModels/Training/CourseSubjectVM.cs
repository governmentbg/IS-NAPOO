using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CourseSubjectVM
    {
        public CourseSubjectVM()
        {
            this.ClientCourseSubjectGrades = new HashSet<CourseSubjectGradeVM>();
        }

        public int IdCourseSubject { get; set; }

        [Comment("Връзка с курс за обучение, предлагани от ЦПО")]
        public int IdCourse { get; set; }

        public virtual CourseVM Course { get; set; }

        [Required]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }

        public string ProfessionalTrainingName { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }

        [Display(Name = "Часове по практика")]
        [Comment("Сумарен брой часове по практика")]
        public double PracticeHours { get; set; }

        [Display(Name = "Часове по теория")]
        [Comment("Сумарен брой часове по теория")]
        public double TheoryHours { get; set; }

        public int EnteredTheoryGradesCount { get; set; }

        public int EnteredPracticeGradesCount { get; set; }

        public virtual ICollection<CourseSubjectGradeVM> ClientCourseSubjectGrades { get; set; }

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
