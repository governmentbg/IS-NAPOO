using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Оценка от протокол към курс за валидиране
    /// </summary>
    [Table("Training_ValidationProtocolGrade")]
    [Comment("Протокол към курс за валидиране")]
    public class ValidationProtocolGrade : IEntity, IModifiable
    {
        [Key]
        public int IdValidationProtocolGrade { get; set; }
        public int IdEntity => IdValidationProtocolGrade;

        [Comment("Връзка с протокол към курс за валидиране")]
        [ForeignKey(nameof(ValidationProtocol))]
        public int IdValidationProtocol { get; set; }

        public ValidationProtocol ValidationProtocol { get; set; }

        [Comment("Връзка с курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        [Display(Name = "Оценка от протокол")]
        [Comment("Оценка от протокол от курс за валидиране")]
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
