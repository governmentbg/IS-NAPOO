using ISNAPOO.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Control
{
    public class FollowUpControlDocumentNotificationVM
    {
        public int IdFollowUpControlDocumentNotification { get; set; }

        [Display(Name ="Връзка с документ по процедура")]
        public int IdFollowUpControlDocument { get; set; }

        public FollowUpControlDocumentVM FollowUpControlDocument { get; set; }

        [Display(Name = "Връзка с известие")]
        public int IdNotification { get; set; }

        public NotificationVM Notification { get; set; }

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
