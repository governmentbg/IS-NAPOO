using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class QuestionVM
    {
        public QuestionVM()
        {
            this.Answers = new HashSet<AnswerVM>();
            this.UserAnswers = new HashSet<UserAnswerVM>();
            this.UserAnswerOpens = new HashSet<UserAnswerOpenVM>();
        }

        public int IdQuestion { get; set; }

        [Comment("Връзка с анкета")]
        public int IdSurvey { get; set; }

        public virtual SurveyVM Survey { get; set; }

        [Required(ErrorMessage = "Полето 'Текст на въпроса' е задължително!")]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Текст на въпроса' може да съдържа до 1000 символа!")]
        [Comment("Върпос")]
        public string Text { get; set; }

        [Required(ErrorMessage = "Полето 'Тип на въпроса' е задължително!")]
        [Comment("Тип въпрос")]
        public int? IdQuestType { get; set; } // Номенклатура: "QuestionType" - Множествен, Единствен, Множествен/отворен, Единствен/отворен, Отворен

        [Comment("Следващ въпрос")]
        public int? IdNext { get; set; }

        [Comment("Предишен въпрос")]
        public int? IdPrev { get; set; }

        [Comment("Задължителен въпрос")]
        public bool IsRequired { get; set; }

        [Required(ErrorMessage = "Полето 'Поредност' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Поредност' може да има само стойност по-голяма от 0!")]
        [Comment("Поредност")]
        public int? Order { get; set; }

        [Required(ErrorMessage = "Полето 'Брой отговори' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Брой отговори' може да има само стойност по-голяма от 0!")]
        [Comment("Брой отговори към въпрос")]
        public int? AnswersCount { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        public virtual ICollection<AnswerVM> Answers { get; set; }

        public virtual ICollection<UserAnswerVM> UserAnswers { get; set; }

        public virtual ICollection<UserAnswerOpenVM> UserAnswerOpens { get; set; }



        #region САМООЦЕНЯВАНЕ

        [Comment("Област на самооценяване")]
        public int? IdAreaSelfAssessment { get; set; } // Номенклатура: "AreaSelfAssessment" - Област на самооценяване:Достъп до професионално обучение, Придобиване на професионална квалификация,Реализация на лицата, придобили професионална квалификация


        [Comment("Област на самооценяване наименование")]
        public KeyValueVM AreaSelfAssessment { get; set; }

        [Comment("Вид на индикатор")]
        public int? IdRatingIndicatorType { get; set; } // Номенклатура: "RatingIndicatorType" - Вид на индикатор:Осигурена достъпна архитектурна среда, Предоставяне на възможност за професионално обучение в различни форми на обучение, 



        [Comment("Описание към въпроса")]
        public string Description { get; set; }

        public int CurrentNumber { get; set; }

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
