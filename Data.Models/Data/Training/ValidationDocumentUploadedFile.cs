using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    [Table("Training_ValidationDocumentUploadedFile")]
    [Comment("Прикачени файлове за документи на курсисти за процедура по валидиране")]
    public class ValidationDocumentUploadedFile : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdValidationDocumentUploadedFile { get; set; }
        public int IdEntity => IdValidationDocumentUploadedFile;

        [Required]
        [Comment("Връзка с издадени документи на курсисти")]
        [ForeignKey(nameof(ValidationClientDocument))]
        public int IdValidationClientDocument { get; set; }

        public ValidationClientDocument ValidationClientDocument { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion
        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }

        public string? Oid { get; set; }
        #endregion
    }
}
