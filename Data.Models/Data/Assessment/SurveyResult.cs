using Data.Models.Data.Candidate;
using Data.Models.Data.Control;
using Data.Models.Data.Framework;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Assessment
{
    /// <summary>
    /// Анкета
    /// </summary>
    [Table("Assess_SurveyResult")]
    [Display(Name = "Резултати анкета")]
    public class SurveyResult : IEntity, IModifiable
    {
        public SurveyResult()
        {
            this.UserAnswerOpens = new HashSet<UserAnswerOpen>();
        }

        [Key]
        public int IdSurveyResult { get; set; }
        public int IdEntity => IdSurveyResult;

        [Comment("Връзка с анкета")]
        [ForeignKey(nameof(Survey))]
        public int IdSurvey { get; set; }

        public Survey Survey { get; set; }


        [Comment( "Токен за валидация ")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Token { get; set; }

        [Comment("Брой точки")]
        public int TotalPointsReceived { get; set; }

        [Comment("Резултатите са прегледани")]
        public bool IsReviewed { get; set; }


        [Comment("Коментарт към анкетата")]
        [StringLength(DBStringLength.StringLength1000)]
        public string? FeedBack { get; set; }


        [Comment("Дата на започване")]
        public DateTime? StartDate { get; set; }

        [Comment("Дата на приключване")]
        public DateTime? EndDate { get; set; }



        [Comment("Статус")]
        public int? IdStatus { get; set; }//Статус: непопълнена, попълнена, прегледана


        [Comment("Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [Comment("Връзка с обучаем от курс за обучение")]
        [ForeignKey(nameof(ClientCourse))]
        public int? IdClientCourse { get; set; }

        public ClientCourse ClientCourse { get; set; }

        [Comment("Връзка с консултирано лице")]
        [ForeignKey(nameof(ConsultingClient))]
        public int? IdConsultingClient { get; set; }

        public ConsultingClient ConsultingClient { get; set; }

        [Comment("Връзка с обучаем от курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int? IdValidationClient { get; set; }

        public ValidationClient ValidationClient { get; set; }

        public ICollection<UserAnswerOpen> UserAnswerOpens { get; set; }

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

