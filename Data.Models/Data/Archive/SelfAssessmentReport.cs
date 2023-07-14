using Data.Models.Data.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Candidate;
using Microsoft.EntityFrameworkCore;
using ISNAPOO.Common.Constants;
using Data.Models.Data.Assessment;

namespace Data.Models.Data.Archive
{
    /// <summary>
    /// Доклад за самооценка
    /// </summary>
    [Table("Arch_SelfAssessmentReport")]
    [Display(Name = "Доклад за самооценка")]
    public class SelfAssessmentReport : IEntity, IModifiable
    {
        public SelfAssessmentReport()
        {
            this.SelfAssessmentReportStatuses = new HashSet<SelfAssessmentReportStatus>();
        }

        [Key]
        public int IdSelfAssessmentReport { get; set; }
        public int IdEntity => IdSelfAssessmentReport;

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Година на доклада за самооценка")]
        public int Year { get; set; }

        [Comment("Дата на подавате на доклада за самооценка")]
        public DateTime? FilingDate { get; set; }

        public ICollection<SelfAssessmentReportStatus> SelfAssessmentReportStatuses { get; set; }


        [Comment("Връзка с анкета")]
        [ForeignKey(nameof(SurveyResult))]
        public int? IdSurveyResult { get; set; }
        public SurveyResult SurveyResult { get; set; }
        [Comment("Статус на доклад")]
        public int IdStatus { get; set; }        // Номенклатура: "SelfAssessmentReportStatus"

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
