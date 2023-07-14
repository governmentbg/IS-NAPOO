using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Common;
using Data.Models.Data.Framework;

namespace Data.Models.Data.Control
{
    /// <summary>
    /// Прикачен файл към последващ контрол
    /// </summary>
    [Table("Control_FollowUpControlUploadedFile")]
    [Comment("Прикачен файл към последващ контрол")]
    public class FollowUpControlUploadedFile : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdFollowUpControlUploadedFile { get; set; }
        public int IdEntity => IdFollowUpControlUploadedFile;

        [Comment("Връзка с последващ контрол")]
        [ForeignKey(nameof(FollowUpControl))]
        public int IdFollowUpControl { get; set; }

        public FollowUpControl FollowUpControl { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Описание на прикачения файл")]
        public string? Description { get; set; }

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
