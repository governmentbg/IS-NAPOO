using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.Mapping;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationCommissionMemberVM : IMapFrom<ValidationCommissionMember>
    {
        [Key]
        public int IdValidationCommissionMember { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Comment("Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето 'Презиме' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Comment("Презиме")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Comment("Фамилия")]
        public string FamilyName { get; set; }

        [Comment("Дали е председател на комисия")]
        public bool IsChairman { get; set; }

        [Required]
        [Comment("Връзка с клиента по процедура за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }
        public ValidationClientVM ValidationClient { get; set; }

        public string IsChairmanAsStr => IsChairman ? "Да" : "Не";

        public string FullName => $"{FirstName} {FamilyName}";
        public string WholeName => $"{FirstName} {SecondName} {FamilyName}";

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
