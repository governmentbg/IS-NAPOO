using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Data.Models.Data.Framework;
using Data.Models.Data.ExternalExpertCommission;

namespace Data.Models.Data.Control
{
    /// <summary>
    /// Връзка на проследяващ контрол с експерт на НАПОО
    /// </summary>
    [Table("Control_FollowUpControlExpert")]
    [Comment("Връзка на проследяващ контрол с експерт на НАПОО")]
    public class FollowUpControlExpert : IEntity, IModifiable
    {
        [Key]
        public int IdFollowUpControlExpert { get; set; }
        public int IdEntity => IdFollowUpControlExpert;

        [Required]
        [Comment("Връзка с проследяващ контрол")]
        [ForeignKey(nameof(FollowUpControl))]
        public int IdFollowUpControl { get; set; }

        public FollowUpControl FollowUpControl { get; set; }

        [Required]
        [Comment("Връзка с експерт на НАПОО")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }

        public Expert Expert { get; set; }

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
