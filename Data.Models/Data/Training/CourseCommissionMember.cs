using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Членове на изпитна комисия към курс за обучение
    /// </summary>
    [Table("Training_CourseCommissionMember")]
    [Comment("Член на изпитна комисия към курс за обучение")]
    public class CourseCommissionMember : IEntity, IModifiable
    {
        public CourseCommissionMember()
        {
            this.CourseProtocols = new HashSet<CourseProtocol>();
        }

        [Key]
        public int IdCourseCommissionMember { get; set; }
        public int IdEntity => IdCourseCommissionMember;

        [Required]
        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Comment("Име")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Comment("Презиме")]
        public string? SecondName { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Comment("Фамилия")]
        public string FamilyName { get; set; }

        [Comment("Дали е председател на комисия")]
        public bool IsChairman { get; set; }

        [Required]
        [Comment("Връзка с Курса за обучение, предлаган от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }

        public Course Course { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        [Comment("Комисия от стара система на НАПОО")]
        public string? CommissionMembersFromOldIS { get; set; }

        public ICollection<CourseProtocol> CourseProtocols { get; set; }

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
