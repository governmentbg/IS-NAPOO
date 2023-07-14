using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ProviderData
{
    /// <summary>
    /// Управление на срокове по процедури
    /// </summary>
    [Table("Procedure_ManagementDeadlineProcedure")]
    [Display(Name = "Такси за лицензиране")]
    public class ManagementDeadlineProcedure : IEntity, IModifiable
    {
        [Key]
        public int IdManagementDeadlineProcedure { get; set; }
        public int IdEntity => IdManagementDeadlineProcedure;

        [Comment("Вид лицензия")]
        public int IdLicensingType { get; set; }

        [Comment("Статус/Етап на процедурата по лицензиране")]
        public int IdApplicationStatus { get; set; } 

        [Comment("Срок")]
        public int Term { get; set; }



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

