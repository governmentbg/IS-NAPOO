using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using ISNAPOO.Core.ViewModels.Common;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class ProcedureDocumentNotificationVM
    {
        public int IdProcedureDocumentNotification { get; set; }

        [Comment("Връзка с документ по процедура")]
        public int IdProcedureDocument { get; set; }

        public virtual ProcedureDocumentVM ProcedureDocument { get; set; }

        [Comment("Връзка с известие")]
        public int IdNotification { get; set; }

        public virtual NotificationVM Notification { get; set; }

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
