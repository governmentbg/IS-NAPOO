using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace Data.Models.Data.Assessment
{
    /// <summary>
    /// Отговор на въпрос
    /// </summary>
    [Table("Assess_UserAnswer")]
    [Comment("Отговор на въпрос")]
    public class UserAnswer : IEntity
    {
        public UserAnswer()
        {
        }


        [Key]
        public int IdUserAnswer { get; set; }
        public int IdEntity => IdUserAnswer;

        [Comment("Връзка с  отговор")]
        [ForeignKey(nameof(Answer))]
        public int? IdAnswer { get; set; }
        public Answer Answer { get; set; }


        [Comment("Връзка с отворен отговор")]
        [ForeignKey(nameof(UserAnswerOpen))]
        public int IdUserAnswerOpen { get; set; }
        public UserAnswerOpen UserAnswerOpen { get; set; }



        [Comment("Връзка с въпрос")]
        [ForeignKey(nameof(Question))]
        public int IdQuestion { get; set; }
        public Question Question { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        public Decimal? Points { get; set; }





     
    }
}


