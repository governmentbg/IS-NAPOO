using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    [Table("Training_ConsultingDocumentUploadedFile")]
    [Comment("Прикачени файлове за документи на консултирано лице")]
    public class ConsultingDocumentUploadedFile : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdConsultingDocumentUploadedFile { get; set; }
        public int IdEntity => IdConsultingDocumentUploadedFile;

        [Comment("Връзка с консултирано лице")]
        [ForeignKey(nameof(ConsultingClient))]
        public int IdConsultingClient { get; set; }

        public ConsultingClient ConsultingClient { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        public override string? MigrationNote { get; set; }

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
    }
}
