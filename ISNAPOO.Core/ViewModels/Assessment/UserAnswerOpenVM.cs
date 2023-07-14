using Data.Models.Data.Assessment;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class UserAnswerOpenVM
    {
        public UserAnswerOpenVM()
        {
            this.UserAnswers = new HashSet<UserAnswerVM>();
        }
        public int IdUserAnswerOpen { get; set; }

        [Comment("Връзка с резултати анкета")]
        public int IdSurveyResult { get; set; }

        public virtual SurveyResultVM SurveyResult { get; set; }

        [Comment("Връзка с въпрос")]
        public int IdQuestion { get; set; }

        public virtual QuestionVM Question { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Отговор на потребител")]
        public string Text { get; set; }

        [Comment("Точки")]
        public decimal? Points { get; set; }

        public ICollection<UserAnswerVM> UserAnswers { get; set; }


    }
}
