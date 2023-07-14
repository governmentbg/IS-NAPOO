using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Връзка документи по процедури и известия
    /// </summary>
    [Table("Procedure_DocumentNotification")]
    [Display(Name = "Връзка документи по процедури и известия")]
    public class ProcedureDocumentNotification : IEntity, IModifiable
    {
        public ProcedureDocumentNotification()
        {
        } 

        [Key]
        public int IdProcedureDocumentNotification { get; set; }
        public int IdEntity => IdProcedureDocumentNotification;

        [Comment( "Връзка с документ по процедура")]
        [ForeignKey(nameof(ProcedureDocument))]
        public int IdProcedureDocument { get; set; }
        public ProcedureDocument ProcedureDocument { get; set; }


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
