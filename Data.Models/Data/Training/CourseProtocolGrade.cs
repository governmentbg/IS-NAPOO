using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Оценка от протокол към курс за обучение, предлаган от ЦПО
    /// </summary>
    [Table("Training_CourseProtocolGrade")]
    [Comment("Протокол към курс за обучение, предлагани от ЦПО")]
    public class CourseProtocolGrade : IEntity, IModifiable
    {
        [Key]
        public int IdCourseProtocolGrade { get; set; }
        public int IdEntity => IdCourseProtocolGrade;

        [Comment("Връзка с протокол към курс за обучение, предлаган от ЦПО")]
        [ForeignKey(nameof(CourseProtocol))]
        public int IdCourseProtocol { get; set; }

        public CourseProtocol CourseProtocol { get; set; }

        [Comment("Връзка с протокол към курс за обучение, предлаган от ЦПО")]
        [ForeignKey(nameof(ClientCourse))]
        public int IdClientCourse { get; set; }

        public ClientCourse ClientCourse { get; set; }

        [Display(Name = "Оценка от протокол")]
        [Comment("Оценка от протокол от курс за обучение")]
        public double? Grade { get; set; }

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
