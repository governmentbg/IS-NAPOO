using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class ProcedureDocumentVM : IMapFrom<ProcedureDocument>, IMapTo<ProcedureDocument>
    {
        public ProcedureDocumentVM()
        {
            this.ProcedureDocumentNotifications = new HashSet<ProcedureDocumentNotificationVM>();
        }

        [Key]
        public int IdProcedureDocument { get; set; }

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        public int IdStartedProcedure { get; set; }
        public StartedProcedureVM StartedProcedure { get; set; }

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

        public string DocumentTypeNameDescription { get; set; }

        [Display(Name = "Дата на прикачане")]
        public DateTime? DateAttachment { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'MimeType' не може да съдържа повече от 255 символа.")]
        [Display(Name = "MimeType")]
        public string? MimeType { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Extension' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Extension")]
        public string? Extension { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'UploadedFileName' не може да съдържа повече от 512 символа.")]
        [Display(Name = "UploadedFileName")]
        public string? UploadedFileName { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'ID на документ в деловодната система на НАПОО' не може да съдържа повече от 20 символа.")]
        [Display(Name = "ID на документ в деловодната система на НАПОО")]
        public int? DS_ID { get; set; }

        [Display(Name = "Дата на документ в деловодната система на НАПОО")]
        public DateTime? DS_DATE { get; set; }

        public DateOnly? DS_DATEOnly { get { return DS_DATE.HasValue ? DateOnly.FromDateTime(DS_DATE.Value.Date) : null; } }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'GUID на работен документ в деловодната система на НАПОО' не може да съдържа повече от 50 символа.")]
        [Display(Name = "GUID на работен документ в деловодната система на НАПОО")]
        public string? DS_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на на работен документ в деловодната система на НАПОО' не може да съдържа повече от 20 символа.")]
        [Display(Name = "Номер на на работен документ в деловодната система на НАПОО")]
        public string? DS_DocNumber { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Официален номер на документ в деловодната система на НАПОО' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Официален номер на документ в деловодната система на НАПОО")]
        public int? DS_OFFICIAL_ID { get; set; }

        [Display(Name = "Дата на официален документ в деловодната система на НАПОО")]
        public DateTime? DS_OFFICIAL_DATE { get; set; }

        public DateOnly? DS_OFFICIAL_DATEOnly { get { return DS_OFFICIAL_DATE.HasValue ? DateOnly.FromDateTime(DS_OFFICIAL_DATE.Value.Date) : null; } }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'GUID на официален документ в деловодната система на НАПОО' не може да съдържа повече от 50 символа.")]
        [Display(Name = "GUID на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_GUID { get; set; }


        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на официален документ в деловодната система на НАПОО' не може да съдържа повече от 20 символа.")]
        [Display(Name = "Номер на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_DocNumber { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Преписка в деловодната система на НАПОО' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Преписка в деловодната система на НАПОО")]
        public string? DS_PREP { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Връзка към документа  в деловодната система на НАПОО' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Връзка към документа  в деловодната система на НАПОО")]
        public string? DS_LINK { get; set; }
        [DefaultValue(false)]
        [Display(Name = "Стойност дали документа е взет от деловодната система на НАПОО")]
        public bool IsFromDS { get; set; }
        //[Required(ErrorMessage = "Полето '№ на документ' е задължително!")]
        public string? ApplicationNumber { get; set; }
        //[Required(ErrorMessage = "Полето 'Дата' е задължително!")]
        public DateTime? ApplicationDate { get; set; }
        public int gridRowCounter { get; set; }

        public int? IdExpert { get; set; }
        public int? Order { get; set; }
        public ExpertVM Expert { get; set; }

        public string TypeLicensing { get; set; }

        public virtual ICollection<ProcedureDocumentNotificationVM> ProcedureDocumentNotifications { get; set; }

        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Серия' е задължително!")]
        [Display(Name = "Пореден номер на документ в преписката, в която е създаден")]
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
    }
}
