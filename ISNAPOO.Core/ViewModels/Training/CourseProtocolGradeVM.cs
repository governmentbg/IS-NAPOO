using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CourseProtocolGradeVM
    {
        public int IdCourseProtocolGrade { get; set; }

        [Comment("Връзка с протокол към курс за обучение, предлаган от ЦПО")]
        public int IdCourseProtocol { get; set; }

        public virtual CourseProtocolVM CourseProtocol { get; set; }

        [Comment("Връзка с протокол към курс за обучение, предлаган от ЦПО")]
        public int IdClientCourse { get; set; }

        public virtual ClientCourseVM ClientCourse { get; set; }

        [Display(Name = "Оценка от протокол")]
        [Comment("Оценка от протокол от курс за обучение")]
        public double? Grade { get; set; }

        public string GradeAsStr { get; set; }

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
