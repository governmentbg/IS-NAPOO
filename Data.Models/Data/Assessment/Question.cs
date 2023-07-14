using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Data.Models.Data.Assessment
{
    /// <summary>
    /// Въпрос към анкета
    /// </summary>
    [Table("Assess_Question")]
    [Comment( "Въпрос към анкета")]
    public class Question : IEntity, IModifiable
    {
        public Question()
        { 
            this.Answers = new HashSet<Answer>();
            this.UserAnswers = new HashSet<UserAnswer>();
            this.UserAnswerOpens = new HashSet<UserAnswerOpen>();
        }


        [Key]
        public int IdQuestion { get; set; }
        public int IdEntity => IdQuestion;

        [Comment("Връзка с анкета")]
        [ForeignKey(nameof(Survey))]
        public int IdSurvey { get; set; }
        public Survey Survey { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Върпос")]
        public string Text { get; set; }


        [Comment("Тип въпрос")]
        public int? IdQuestType { get; set; } // Номенклатура: "QuestionType" - Тип въпрос: Един отговор, Много отговори, Отворен,Един отговор + Отворен, Много отговори + Отворен
      
        [Comment("Следващ въпрос")]
        [ForeignKey(nameof(Question))]
        public int? IdNext { get; set; }

        [Comment("Предишен въпрос")]
        [ForeignKey(nameof(Question))]
        public int? IdPrev { get; set; }

        [Comment("Задължителен въпрос")]
        public bool  IsRequired { get; set; }

        [Comment("Поредност")]
        public int Order { get; set; }

        [Comment("Брой отговори към въпрос")]
        public int AnswersCount { get; set; }

        public ICollection<Answer> Answers { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }

        public ICollection<UserAnswerOpen> UserAnswerOpens { get; set; }


        #region САМООЦЕНЯВАНЕ

        [Comment("Област на самооценяване")]
        public int? IdAreaSelfAssessment { get; set; } // Номенклатура: "AreaSelfAssessment" - Област на самооценяване:Достъп до професионално обучение, Придобиване на професионална квалификация,Реализация на лицата, придобили професионална квалификация


        [Comment("Вид на индикатор")]
        public int? IdRatingIndicatorType { get; set; } // Номенклатура: "RatingIndicatorType" - Вид на индикатор:Осигурена достъпна архитектурна среда, Предоставяне на възможност за професионално обучение в различни форми на обучение, 


        [StringLength(DBStringLength.StringLength4000)]
        [Comment("Описание към въпроса")]
        public string? Description { get; set; }

        #endregion

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
