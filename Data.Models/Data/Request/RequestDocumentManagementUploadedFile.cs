using System;
using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Request
{
    /// <summary>

    [Table("Request_RequestDocumentManagementUploadedFile")]
    [Display(Name = "Прикачени документи към Получени/Предадени документи")]
    public class RequestDocumentManagementUploadedFile : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdRequestDocumentManagementUploadedFile { get; set; }
        public int IdEntity => IdRequestDocumentManagementUploadedFile;

        [Comment("Връзка със получени/предадени документи")]
        [ForeignKey(nameof(RequestDocumentManagement))]
        public int IdRequestDocumentManagement { get; set; }
        public RequestDocumentManagement RequestDocumentManagement { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Описание")]
        public string? Description { get; set; }

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
        #endregion
    }
}

