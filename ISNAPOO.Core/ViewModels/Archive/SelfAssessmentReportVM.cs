using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.Candidate;
using Data.Models.Data.Assessment;
using ISNAPOO.Core.ViewModels.Assessment;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class SelfAssessmentReportVM
    {
        public SelfAssessmentReportVM()
        {
            this.SelfAssessmentReportStatuses = new HashSet<SelfAssessmentReportStatusVM>();
        }

        public int IdSelfAssessmentReport { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Година на доклада за самооценка")]
        public int Year { get; set; } = DateTime.Now.Year;

        [Comment("Дата на подавате на доклада за самооценка")]
        public DateTime? FilingDate { get; set; }

        public string FilingDateAsStr => this.FilingDate.HasValue ? $"{this.FilingDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        public string Status { get; set; }
        public int IdStatus { get; set; }
        public string StatusIntCode { get; set; }


        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        public int? IdSurveyResult { get; set; }
        public SurveyResultVM SurveyResult { get; set; }

        [Comment("Коментар на статус на доклада за самооценка")]
        public string CommentSelfAssessmentReportStatus { get; set; }

        public virtual ICollection<SelfAssessmentReportStatusVM> SelfAssessmentReportStatuses { get; set; }

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
    }
}
