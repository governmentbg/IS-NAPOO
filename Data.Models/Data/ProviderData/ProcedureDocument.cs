using Data.Models.Data.Common;
using Data.Models.Data.ExternalExpertCommission;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Data.Models.Data.ProviderData
{


    /// <summary>
    /// Данни за Процедура за лицензиране - прогрес
    /// </summary>
    [Table("Procedure_Document")]
    [Display(Name = " Данни за Процедура за лицензиране - прогрес")]
    public class ProcedureDocument : IEntity, IModifiable
    {
        public ProcedureDocument()
        {
        }

        [Key]
        public int IdProcedureDocument { get; set; }
        public int IdEntity => IdProcedureDocument;

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        [ForeignKey(nameof(StartedProcedure))]
        public int IdStartedProcedure { get; set; }
        public StartedProcedure StartedProcedure { get; set; }

        [Display(Name = "Документа е валиден")]        
        public bool IsValid { get; set; }

        /// <summary>
        /// Заявление за лицензиране на ЦПО
        /// Прил. 1 - Доклад за резултата от проверката на редовността на подаденото заявление и документи за издаване на лицензия на ЦПО
        /// Прил. 2 - Заповед за оценка на възможностите на заявителя да извършва професионално обучение 
        /// Прил. 3 - Уведомително писмо до заявителя във връзка с предстоящата лицензионна експертиза
        /// ........
        /// От таблица code_stage_documents
        /// </summary>

        [Display(Name = "Тип документ")]
        public int? IdDocumentType { get; set; }

        [Display(Name = "Дата на прикачане")]
        public DateTime? DateAttachment { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "MimeType")]
        public string? MimeType { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Extension")]
        public string? Extension { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "UploadedFileName")]
        public string? UploadedFileName { get; set; }
        
        [Display(Name = "ID на работен документ в деловодната система на НАПОО")]
        public int? DS_ID { get; set; }

        [Display(Name = "Дата на работен документ в деловодната система на НАПОО")]
        public DateTime? DS_DATE { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на работен документ в деловодната система на НАПОО")]
        public string? DS_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на на работен документ в деловодната система на НАПОО")]
        public string? DS_DocNumber  { get; set; }


        [Display(Name = "Официален номер на документ в деловодната система на НАПОО")]

        public int? DS_OFFICIAL_ID { get; set; }

        [Display(Name = "Дата на официален документ в деловодната система на НАПОО")]
        public DateTime? DS_OFFICIAL_DATE { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_GUID { get; set; }


        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_DocNumber { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Преписка в деловодната система на НАПОО")]
        public string? DS_PREP { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Връзка към документа  в деловодната система на НАПОО")]
        public string? DS_LINK { get; set; }
        [DefaultValue(false)]
        [Display(Name = "Стойност дали документа е взет от деловодната система на НАПОО")]
        public bool IsFromDS { get; set; }


        [Comment("Връзка на документа с  Експерт")]
        [ForeignKey(nameof(Expert))]
        public int? IdExpert { get; set; }
        public Expert Expert { get; set; }


       
        [Comment( "Пореден номер на документ в преписката, в която е създаден")]
        public int? DeloSerial { get; set; }

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
        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion

    }
}
