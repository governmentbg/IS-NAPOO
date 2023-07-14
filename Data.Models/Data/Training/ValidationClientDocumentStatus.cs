using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Common.Constants;
using Data.Models.Data.Framework;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// История на статуса на документ за завършване на курсист за валидиране
    /// </summary>
    [Table("Training_ValidationClientDocumentStatus")]
    [Comment("История на статуса на документ за завършване на курсист за валидиране")]
    public class ValidationClientDocumentStatus : IEntity, IModifiable
    {
        [Key]
        public int IdValidationClientDocumentStatus { get; set; }
        public int IdEntity => IdValidationClientDocumentStatus;

        [Required]
        [Comment("Връзка с издаден документ на курсист за валидиране")]
        [ForeignKey(nameof(ValidationClientDocument))]
        public int IdValidationClientDocument { get; set; }

        public ValidationClientDocument ValidationClientDocument { get; set; }

        [Comment("Статус на документа за завършване на курсист")]
        public int IdClientDocumentStatus { get; set; } // номенклатура: KeyTypeIntCode - "ClientDocumentStatusType"

        [Column(TypeName = "ntext")]
        [Comment("Коментар при подаване към НАПОО")]
        public string? SubmissionComment { get; set; }

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
        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
