using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Data.Models.Data.Control;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка на Курс и направена проверка
    /// </summary>
    [Table("Training_CourseChecking")]
    [Display(Name = "Връзка на Курс и направена проверка")]
    public class CourseChecking : IEntity, IModifiable
    {
        [Key]
        public int IdCourseChecking { get; set; }
        public int IdEntity => IdCourseChecking;

        [Comment("Връзка с курс за обучение")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }

        public Course Course { get; set; }

        [Comment("Последващ контрол, изпълняван от служител/и на НАПОО")]
        [ForeignKey(nameof(FollowUpControl))]
        public int? IdFollowUpControl { get; set; }

        public FollowUpControl FollowUpControl { get; set; }

        [Comment("Извършена проверка от експерт на НАПОО")]
        public bool CheckDone { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Коментар' не може да съдържа повече от 512 символа.")]
        [Comment("Коментар")]
        public string? Comment { get; set; }

        [Comment("Дата на проверка")]
        public DateTime? CheckingDate { get; set; }

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
