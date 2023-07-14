using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    [Table("Training_CourseDocumentUploadedFile")]
    [Comment("Прикачени файлове за документи на курсисти")]
    public class CourseDocumentUploadedFile : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdCourseDocumentUploadedFile { get; set; }
        public int IdEntity => IdCourseDocumentUploadedFile;

        [Required]
        [Comment("Връзка с издадени документи на курсисти")]
        [ForeignKey(nameof(ClientCourseDocument))]
        public int IdClientCourseDocument { get; set; }
        public ClientCourseDocument ClientCourseDocument { get; set; }

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
