using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// Статус на доклад за самооценка
    /// </summary>
    [Table("Arch_SelfAssessmentReportStatus")]
    [Display(Name = "Статус на доклад за самооценка")]
    public class SelfAssessmentReportStatus : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdSelfAssessmentReportStatus { get; set; }
        public int IdEntity => IdSelfAssessmentReportStatus;

        [Comment("Връзка с доклад за самооценка")]
        [ForeignKey(nameof(SelfAssessmentReport))]
        public int IdSelfAssessmentReport { get; set; }

        public SelfAssessmentReport SelfAssessmentReport { get; set; }

        // Номенклатура: "SelfAssessmentReportStatus"
        [Comment("Статус на доклад за самооценка")] 
        public int IdStatus { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Коментар при операция с доклад за самооценка")]
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
