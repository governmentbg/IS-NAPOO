using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class SurveyResultVM
    {
        public SurveyResultVM()
        {
            this.UserAnswerOpens = new HashSet<UserAnswerOpenVM>();
        }

        public int IdSurveyResult { get; set; }

        [Comment("Връзка с анкета")]
        public int IdSurvey { get; set; }

        public virtual SurveyVM Survey { get; set; }

        [Comment("Токен за валидация ")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Token { get; set; }

        [Comment("Брой точки")]
        public int TotalPointsReceived { get; set; }

        [Comment("Резултатите са прегледани")]
        public bool IsReviewed { get; set; }

        [Comment("Коментар към анкетата")]
        [StringLength(DBStringLength.StringLength1000)]
        public string? FeedBack { get; set; }

        [Comment("Дата на започване")]
        public DateTime? StartDate { get; set; }

        [Comment("Дата на приключване")]
        public DateTime? EndDate { get; set; }

        [Comment("Статус")]
        public int? IdStatus { get; set; } // Номенклатура - KeyTypeIntCode: "SurveyResultStatusType"

        [Comment("Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        public int IdCandidate_Provider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка с обучаем от курс за обучение")]
        public int? IdClientCourse { get; set; }

        public virtual ClientCourseVM ClientCourse { get; set; }

        [Comment("Връзка с консултирано лице")]
        public int? IdConsultingClient { get; set; }

        public virtual ConsultingClientVM ConsultingClient { get; set; }

        [Comment("Връзка с обучаем от курс за валидиране")]
        public int? IdValidationClient { get; set; }

        public virtual ValidationClientVM ValidationClient { get; set; }

        public string EmailTemplateHeader { get; set; }

        public string EmailTemplateText { get; set; }

        [Comment("Дата на активност на анкетата до")]
        public DateTime? SurveyEndDate { get; set; }

        public virtual ICollection<UserAnswerOpenVM> UserAnswerOpens { get; set; }

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
