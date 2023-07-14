using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Данни за Процедура за лицензиране - връзка с външен експерт
    /// </summary>
    [Table("Procedure_ExternalExpert")]
    [Display(Name = " Данни за Процедура за лицензиране - връзка с външен експерт")]
    public class ProcedureExternalExpert : AbstractUploadFile, IEntity
    {
        public ProcedureExternalExpert()
        {

        }

        [Key]
        public int IdProcedureExternalExpert { get; set; }
        public int IdEntity => IdProcedureExternalExpert;

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        [ForeignKey(nameof(StartedProcedure))]
        public int IdStartedProcedure { get; set; }
        public StartedProcedure StartedProcedure { get; set; }


        [Display(Name = "Връзка с  Експерт")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }
        public Expert Expert { get; set; }

       
        [Display(Name = "Професионално направление")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int? IdProfessionalDirection { get; set; }
        public ProfessionalDirection ProfessionalDirection { get; set; }

        [Display(Name = "Данни за Процедура за лицензиране - прогрес")]
        [ForeignKey(nameof(ProcedureDocument))]
        public int? IdProcedureDocument { get; set; }
        public ProcedureDocument ProcedureDocument { get; set; }

        [Display(Name = "Активен/Неактивен")]
        [Comment("Показва статуса на външния експерт спрямо процедурата")]
        public bool IsActive { get; set; }

        [Comment("Дата на прикачване на доклада")]
        public DateTime? UploadDate { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string? UploadedFileName { get; set; }

        public override string? MigrationNote { get; set; }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion
    }
}
