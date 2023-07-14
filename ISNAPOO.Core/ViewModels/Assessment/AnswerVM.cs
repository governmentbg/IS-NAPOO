using Data.Models.Data.Assessment;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class AnswerVM
    {
        public AnswerVM()
        {
            this.UserAnswers = new HashSet<UserAnswerVM>();
        }

        public int IdAnswer { get; set; }

        [Comment("Връзка с въпрос")]
        public int IdQuestion { get; set; }

        public virtual QuestionVM Question { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Отговор")]
        public string Text { get; set; }

        [Comment("Точки")]

        public Decimal? Points { get; set; }

        public string Symbol { get; set; }

        public string SymbolAndText => $"{this.Symbol}. {this.Text}";
        public string SymbolAndTextPoints => $"{this.Symbol}. {this.Text} <b>({this.Points} т.)</b>";

        public virtual ICollection<UserAnswerVM> UserAnswers { get; set; }

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
