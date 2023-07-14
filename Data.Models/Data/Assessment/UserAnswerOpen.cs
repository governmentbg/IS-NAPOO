using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Data.Models.Data.Assessment
{
    /// <summary>
    /// Отговор на въпрос
    /// </summary>
    [Table("Assess_UserAnswerOpen")]
    [Comment("Отговор на въпрос")]
    public class UserAnswerOpen : IEntity
    {
        public UserAnswerOpen()
        {

            this.UserAnswers = new HashSet<UserAnswer>();
        }


        [Key]
        public int IdUserAnswerOpen { get; set; }
        public int IdEntity => IdUserAnswerOpen;

        [Comment("Връзка с резултати анкета")]
        [ForeignKey(nameof(SurveyResult))]
        public int IdSurveyResult { get; set; }
        public SurveyResult SurveyResult { get; set; }



        [Comment("Връзка с въпрос")]
        [ForeignKey(nameof(Question))]
        public int IdQuestion { get; set; }
        public Question Question { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Отговор на потребител")]
        public string Text { get; set; }



        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        public Decimal? Points { get; set; }


        public ICollection<UserAnswer> UserAnswers { get; set; }
    }
}

