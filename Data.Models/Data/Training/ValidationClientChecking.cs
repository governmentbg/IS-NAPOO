using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using Data.Models.Data.Control;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Връзка на валидирано лице и направена проверка
    /// </summary>
    [Table("Training_ValidationClientChecking")]
    [Display(Name = "Връзка на валидирано лице и направена проверка")]
    public class ValidationClientChecking : IEntity, IModifiable
    {
        [Key]
        public int IdValidationClientChecking { get; set; }
        public int IdEntity => IdValidationClientChecking;

        [Comment("Връзка с валидирано лице")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

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
