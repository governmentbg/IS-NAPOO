using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Протокол към процедура за валидиране
    /// </summary>
    [Table("Training_ValidationProtocol")]
    [Comment("Протокол към процедура за валидиране")]
    public class ValidationProtocol : AbstractUploadFile, IEntity, IModifiable
    {
        public ValidationProtocol()
        {
            this.ValidationClientDocuments = new HashSet<ValidationClientDocument>();
            this.ValidationProtocolGrades = new HashSet<ValidationProtocolGrade>();
        }

        [Key]
        public int IdValidationProtocol { get; set; }
        public int IdEntity => IdValidationProtocol;

        [Comment("Връзка с клиент по процедура за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        [Comment("Връзка с ЦПО")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Връзка с член на изпитна комисия към курс за валидиране")]
        [ForeignKey(nameof(ValidationCommissionMember))]
        public int? IdValidationCommissionMember { get; set; }

        public ValidationCommissionMember ValidationCommissionMember { get; set; }

        [Comment("Описание на протокол")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Description { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength50)]
        [Comment("Номер на протокол")]
        public string ValidationProtocolNumber { get; set; }

        [Required]
        [Comment("Дата на протокол")]
        public DateTime? ValidationProtocolDate { get; set; }

        [Required]
        [Comment("Вид на протокол")]
        public int IdValidationProtocolType { get; set; } // Номенклатура 'CourseProtocolType'

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }

        public ICollection<ValidationClientDocument> ValidationClientDocuments { get; set; }

        public ICollection<ValidationProtocolGrade> ValidationProtocolGrades { get; set; }

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
