using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using Data.Models.Common;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// История на статуса на отчета за годишна информация
    [Table("Arch_AnnualInfoStatus")]
    [Display(Name = "История на статуса на отчета за годишна информация")]
    public class AnnualInfoStatus : AbstractUploadFile,IEntity
    {
        [Key]
        public int IdAnnualInfoStatus { get; set; }
        public int IdEntity => IdAnnualInfoStatus;

        [Required]
        [Comment("Връзка с отчета за годишна информация")]
        [ForeignKey(nameof(AnnualInfo))]
        public int IdAnnualInfo { get; set; }

        public AnnualInfo AnnualInfo { get; set; }

        [Comment("Статус на отчета за годишна информация")]
        public int IdStatus { get; set; } // Номенклатура - KeyTypeIntCode: "AnnualInfoStatusType"

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Коментар при операция с отчета за годишна информация")]
        public string? Comment { get; set; }
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
        #endregion
    }
}
