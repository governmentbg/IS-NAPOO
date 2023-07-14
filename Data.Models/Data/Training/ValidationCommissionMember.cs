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
    /// Членове на изпитна комисия към процедура за валидиране
    /// </summary>
    [Table("Training_ValidationCommissionMember")]
    [Comment("Членове на изпитна комисия към процедура за валидиране")]
    public class ValidationCommissionMember : IEntity, IModifiable
    {
        public ValidationCommissionMember()
        {
            this.ValidationProtocols = new HashSet<ValidationProtocol>();
        }

        [Key]
        public int IdValidationCommissionMember { get; set; }
        public int IdEntity => IdValidationCommissionMember;

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
        [Comment("Връзка с клиента по процедура за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        public ICollection<ValidationProtocol> ValidationProtocols { get; set; }

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
