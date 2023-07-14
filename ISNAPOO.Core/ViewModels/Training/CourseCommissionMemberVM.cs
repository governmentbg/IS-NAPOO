using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CourseCommissionMemberVM
    {
        public CourseCommissionMemberVM()
        {
            this.CourseProtocols = new HashSet<CourseProtocolVM>();
        }

        public int IdCourseCommissionMember { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Име' не може да съдържа повече от 4000 символа.")]
        [Comment("Име")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Име' може да съдържа само текст на български език!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето 'Презиме' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Comment("Презиме")]
        [RegularExpression(@"\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Презиме' може да съдържа само текст на български език!")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Comment("Фамилия")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Фамилия' може да съдържа само текст на български език!")]
        public string FamilyName { get; set; }

        [Comment("Дали е председател на комисия")]
        public bool IsChairman { get; set; }

        [Comment("Връзка с Курса за обучение, предлаган от ЦПО")]
        public int IdCourse { get; set; }

        [Comment("Комисия от стара система на НАПОО")]
        public string? CommissionMembersFromOldIS { get; set; }

        public string FullName => $"{FirstName} {FamilyName}";

        public string WholeName => $"{FirstName} {SecondName} {FamilyName}";

        public string IsChairmanAsStr => this.IsChairman ? "Да" : "Не";

        public virtual CourseVM Course { get; set; }

        public virtual ICollection<CourseProtocolVM> CourseProtocols { get; set; }

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
