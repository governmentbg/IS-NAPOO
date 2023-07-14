using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Предмет от програма за обучение към курс
    /// </summary>
    [Table("Training_CourseSubject")]
    [Comment("Предмет от програма за обучение към курс")]
    public class CourseSubject : IEntity, IModifiable
    {
        public CourseSubject()
        {
            this.CourseSubjectGrades = new HashSet<CourseSubjectGrade>();
        }

        [Key]
        public int IdCourseSubject { get; set; }
        public int IdEntity => IdCourseSubject;

        [Comment("Връзка с курс за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }

        public Course Course { get; set; }

        [Required]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }

        [Display(Name = "Часове по практика")]
        [Comment("Сумарен брой часове по практика")]
        public double PracticeHours { get; set; }

        [Display(Name = "Часове по теория")]
        [Comment("Сумарен брой часове по теория")]
        public double TheoryHours { get; set; }

        public ICollection<CourseSubjectGrade> CourseSubjectGrades { get; set; }

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
