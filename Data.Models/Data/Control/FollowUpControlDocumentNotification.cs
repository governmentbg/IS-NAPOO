using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Control
{
    /// <summary>
    /// Връзка с Протокол/доклад/заповед във връзка с проследяващ контрол
    /// </summary>
    [Table("Control_FollowUpControlDocumentNotification")]
    [Display(Name = "Връзка документи за проследяващ контрол и известия")]
    public class FollowUpControlDocumentNotification : IEntity, IModifiable
    {
        [Key]
        public int IdFollowUpControlDocumentNotification { get; set; }
        public int IdEntity => IdFollowUpControlDocumentNotification;

        [Comment("Връзка с документ по процедура")]
        [ForeignKey(nameof(FollowUpControlDocument))]
        public int IdFollowUpControlDocument { get; set; }

        public FollowUpControlDocument FollowUpControlDocument { get; set; }

        [Comment("Връзка с известие")]
        [ForeignKey(nameof(Notification))]
        public int IdNotification { get; set; }

        public Notification Notification { get; set; }

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
