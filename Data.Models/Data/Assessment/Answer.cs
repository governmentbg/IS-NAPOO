using Data.Models.Data.Archive;
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
    /// Въпрос към анкета
    /// </summary>
    [Table("Assess_Answer")]
    [Comment("Възможен отговор към върпос")]
    public class Answer : IEntity, IModifiable
    {
        public Answer()
        {
            this.UserAnswers = new HashSet<UserAnswer>();
        }


        [Key]
        public int IdAnswer { get; set; }
        public int IdEntity => IdAnswer;

        [Comment("Връзка с въпрос")]
        [ForeignKey(nameof(Question))]
        public int IdQuestion { get; set; }
        public Question Question { get; set; }


        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Отговор")]
        public string Text { get; set; }


        [Column(TypeName = "decimal(5, 2)")]
        [Comment("Точки")]
        public Decimal? Points { get; set; }

        public ICollection<UserAnswer> UserAnswers { get; set; }

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
