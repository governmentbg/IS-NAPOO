using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// История на статуса на документ за завършване на курсист
    /// </summary>
    [Table("Training_ClientCourseDocumentStatus")]
    [Comment("История на статуса на документ за завършване на курсист")]
    public class ClientCourseDocumentStatus : IEntity, IModifiable
    {
        [Key]
        public int IdClientCourseDocumentStatus { get; set; }
        public int IdEntity => IdClientCourseDocumentStatus;

        [Required]
        [Comment("Връзка с издадени документ на курсист")]
        [ForeignKey(nameof(ClientCourseDocument))]
        public int IdClientCourseDocument { get; set; }

        public ClientCourseDocument ClientCourseDocument { get; set; }

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
