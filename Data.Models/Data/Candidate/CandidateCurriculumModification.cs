using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using Data.Models.Common;
using Microsoft.EntityFrameworkCore;
using ISNAPOO.Common.Constants;

namespace Data.Models.Data.Candidate
{
    // <summary>
    /// Промяна на учебна програма за специалност
    /// </summary>
    [Table("Candidate_CurriculumModification")]
    [Display(Name = "Промяна на учебна програма за специалност")]
    public class CandidateCurriculumModification : AbstractUploadFile, IEntity, IModifiable
    {
        public CandidateCurriculumModification()
        {
            this.CandidateCurriculums = new HashSet<CandidateCurriculum>();
        }

        [Key]
        public int IdCandidateCurriculumModification { get; set; }
        public int IdEntity => IdCandidateCurriculumModification;

        [Comment("Връзка с лицензирана специалност")]
        [ForeignKey(nameof(CandidateProviderSpeciality))]
        public int IdCandidateProviderSpeciality { get; set; }

        public CandidateProviderSpeciality CandidateProviderSpeciality { get; set; }

        [Comment("Вид на причина за промяна на учебната програма")] // Номенклатура - KeyTypeIntCode: "CurriculumModificationReasonType"
        public int IdModificationReason { get; set; }

        [Comment("Статус на промяната на учебната програма")] // Номенклатура - KeyTypeIntCode: "CurriculumModificationStatusType"
        public int IdModificationStatus { get; set; }

        [Comment("Дата на влизане в сила на промяната на учебната програма")]
        public DateTime? ValidFromDate { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл с учебната програма")]
        public override string? UploadedFileName { get; set; }

        public ICollection<CandidateCurriculum> CandidateCurriculums { get; set; }

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
