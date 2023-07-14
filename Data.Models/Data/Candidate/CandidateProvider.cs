using Data.Models.Common;
using Data.Models.Data.Archive;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.EGovPayment;
using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using Data.Models.Data.Request;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{

    /// <summary>
    /// CPO,CIPO - Кандидат Обучаваща институция, Заявление
    /// </summary>
    [Table("Candidate_Provider")]
    [Display(Name = "CPO,CIPO - Обучаваща институция")]
    public class CandidateProvider : AbstractUploadFile, IEntity, IModifiable, IDataMigration
    {
        public CandidateProvider()
        {
            this.UploadedFileName = string.Empty;
            this.CandidateProviderSpecialities = new HashSet<CandidateProviderSpeciality>();
            this.CandidateProviderTrainers = new HashSet<CandidateProviderTrainer>();
            this.CandidateProviderStatuses = new HashSet<CandidateProviderStatus>();
            this.CandidateProviderPremises  = new HashSet<CandidateProviderPremises>();
            this.CandidateProviderDocuments = new HashSet<CandidateProviderDocument>();
            this.RequestReports = new HashSet<RequestReport>();
            this.ProviderDocumentOffers = new HashSet<ProviderDocumentOffer>();
            this.CandidateProviderPersons = new HashSet<CandidateProviderPerson>();
            this.AnnualInfos = new HashSet<AnnualInfo>();
            this.Programs = new HashSet<Program>();
            this.CandidateProviderConsultings = new HashSet<CandidateProviderConsulting>();
            this.Courses = new HashSet<Course>();
            this.ValidationClients = new HashSet<ValidationClient>();
            this.SelfAssessmentReports = new HashSet<SelfAssessmentReport>();
            this.CandidateProviderCPOStructureAndActivities = new HashSet<CandidateProviderCPOStructureActivity>();
            this.CandidateProviderCIPOStructureAndActivities = new HashSet<CandidateProviderCIPOStructureActivity>();
            this.ProviderRequestDocuments = new HashSet<ProviderRequestDocument>();
            this.Courses = new HashSet<Course>();
            this.Payments = new HashSet<Payment>();
            this.FollowUpControls= new HashSet<FollowUpControl>();
        }

        [Key]
        public int IdCandidate_Provider { get; set; }
        public int IdEntity => IdCandidate_Provider;

        #region Данни за Юридическо лица
        [Required]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на юридическото лице' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Наименование на юридическото лице")]
        public string ProviderOwner { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на юридическото лице на латиница' не може да съдържа повече от 255 символа.")]
        [Comment("Наименование на юридическото лице на латиница")]
        public string? ProviderOwnerEN { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'ЕИК/Булстат' не може да съдържа повече от 20 символа.")]
        [Display(Name = "ЕИК/Булстат")]
        public string PoviderBulstat { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Представлявано от' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Представлявано от")]
        public string? ManagerName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Упълномощено лице' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Упълномощено лице")]
        public string? AttorneyName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'ЕГН, ИТН, ИДН' не може да съдържа повече от 100 символа.")]
        [Display(Name = "ЕГН, ИТН, ИДН")]
        public string? Indent { get; set; }


        /// <summary>
        /// "Регистър Булстат: ЮЛНЦ в частна полза",
        /// "Регистър Булстат: държавно предприятие",
        /// "Централен регистър на МП: ЮЛНЦ в обществена полза",
        /// "Рeгистри на МОН: средни и висши училища",
        /// "Чуждестранно юридическо лице",
        /// "Търговски регистър" 
        /// </summary>
        [Display(Name = "Регистрирано в")]
        public int IdProviderRegistration { get; set; }


        /// <summary>
        /// "държавна"
        /// "общинска"
        /// "частна"
        /// </summary>
        [Display(Name = "Форма на собственост")]
        public int IdProviderOwnership { get; set; }

        /// <summary>
        /// "ЦПО - самостоятелен"
        /// "ЦПО към търговско дружество или ЕТ"
        /// "ЦПО към фондация или неправителствена организация"
        /// "ЦИПО"
        /// "друга институция, предлагаща обучение"
        /// "ЦПО - към ЮЛНЦ в обществена полза"
        /// "ЦПО - към ЮЛНЦ в частна полза"
        /// "ЦИПО - самостоятелен"
        /// "ЦИПО - към търговско дружество или ЕТ"
        /// "ЦИПО - към ЮЛНЦ в обществена полза"
        /// "ЦИПО - към ЮЛНЦ в частна полза"
        /// "ЦИПО - друг"
        /// </summary>
        [Display(Name = "Вид на обучаващата институция")]
        public int IdProviderStatus { get; set; }

        [Display(Name = "Населено място")]
        [ForeignKey(nameof(Location))]
        public int? IdLocation { get; set; }
        public Location Location { get; set; }

        [Display(Name = "Адрес по регистрация")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес по регистрация' не може да съдържа повече от 255 символа.")]
        public string ProviderAddress { get; set; }

        [Display(Name = "Адрес по регистрация на латиница")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес по регистрация' не може да съдържа повече от 255 символа.")]
        public string ProviderAddressEN { get; set; }

        [Display(Name = "Пощенски код")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 4 символа.")]
        public string ZipCode { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до нотариално пълномощно' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Път до нотариално пълномощно")]
        public override string UploadedFileName { get; set; }


        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Титла' не може да съдържа повече от 20 символа.")]
        [Display(Name = "Титла")]
        public string? Title { get; set; }

        [Display(Name = "Административен район за ЮЛ")]
        [ForeignKey(nameof(RegionAdmin))]
        public int? IdRegionAdmin { get; set; }

        public Region RegionAdmin { get; set; }

        #endregion

        #region Данни за центъра за професионално обучение
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Име на ЦПО,ЦИПО' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Име на ЦПО,ЦИПО")]
        public string? ProviderName { get; set; }


        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Име на ЦПО,ЦИПО на Латиница' не може да съдържа повече от 512 символа.")]
        [Comment("Име на ЦПО,ЦИПО на Латиница")]
        public string? ProviderNameEN { get; set; }


        [Display(Name = "Вид на лицензията")]
        public int IdTypeLicense { get; set; }//Лицензия за ЦПО, Лицензия за ЦИПО

        [Display(Name = "Номер на заявление")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Номер на заявление' не може да съдържа повече от 255 символа.")]
        public string? ApplicationNumber { get; set; }
         
        [Display(Name = "Дата на заявление")]
        public DateTime? ApplicationDate { get; set; }



        [Comment("Номер на лиценза")]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на лиценза' не може да съдържа повече от 20 символа.")]
        public string? LicenceNumber { get; set; }

        [Comment("Дата на получаване на лицензия")]
        public DateTime? LicenceDate { get; set; }

        //active
        //erased
        //revoked
        //temporarily revoked for a period of 6 moths
        //temporarily revoked for a period of 3 moths
        //suspended
        //temporarily revoked for a period of 4 moths
        [Comment("Статус на  лицензията")]
        public int? IdLicenceStatus { get; set; }//Таблица 'code_licence_status', Стойности:активна,заличена,окончателно отнета,временно отнета за срок от 6 месеца,временно отнета за срок от 3 месеца, прекратена, временно отнета за срок от 4 месеца



        [Display(Name = "Телефон на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderPhone { get; set; }


        [Display(Name = "Факс на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Факс на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderFax { get; set; }

        [Display(Name = "Интернет страница на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Интернет страница на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderWeb { get; set; }

        [Display(Name = "E-mail на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderEmail { get; set; }



        [Display(Name = "Допълнителна информация")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Допълнителна информация' не може да съдържа повече от 512 символа.")]
        public string? AdditionalInfo { get; set; }

        [Display(Name = "Достъпност на архитектурна среда за хора с увреждания, жестомимичен превод")]       
        public bool AccessibilityInfo { get; set; }


        [Display(Name = "Условия за онлайн обучение")]        
        public bool OnlineTrainingInfo { get; set; }


        [Display(Name = "Вид на заявлението")]
        public int? IdTypeApplication { get; set; }//Първоначално лицензиране/Изменение на лицензията

        [Comment("Статус на  заявлението")]
        public int? IdApplicationStatus { get; set; }//Регистрирано,Отказано заявление за лицензиране на нов център, Водещият експерт е дал положителна оценка, Водещият експерт е дал отрицателна оценка



        /// <summary>
        /// Очаква се валидация на електронна поща
        /// Успешна валидация на електронна поща
        /// Очаква потвърждение от НАПОО
        /// Одобрена заявка от НАПОО
        /// Автоматично одобрение
        /// Създаден потребител 
        /// </summary>
        [Comment( "Статус на регистрация на заявлението")]
        public int? IdRegistrationApplicationStatus { get; set; }// 

        [Comment("Причина за отказ")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Причина за отказ' не може да съдържа повече от 512 символа.")]
        public string? RejectionReason { get; set; }


        [Display(Name = "Дата на заявката за регистрация")]
        public DateTime? DateRequest { get; set; }

        [Display(Name = "Дата на валидност на заявката за потвърждаване на e-mail")]
        public DateTime? DueDateRequest { get; set; }

        [Display(Name = "Дата на потвърждаване на заявка от НАПОО, след избор на Пълномощник")]
        public DateTime? DateConfirmRequestNAPOO { get; set; }

        [Display(Name = "Дата на потвърждаване на e-mail")]
        public DateTime? DateConfirmEMail { get; set; }


        [Comment( "Начин на получаване на административен акт и лицензия")]
        public int? IdReceiveLicense { get; set; }// На  място  в  звеното  за  административно  обслужване  на НАПОО, Чрез  лицензиран  пощенски оператор,  като  вътрешна куриерска пратка, на адреса, изписан като адрес за кореспонденция с центъра за професионално обучение, и декларирам, че пощенските разходи са за моя сметка, като давам съгласие документите да бъдат пренасяни за служебни цели .....

        [Comment("Начин на подаване на заявление и документ за платена държавна такса")]
        public int? IdApplicationFiling { get; set; }// На място в звеното за административно обслужване, Чрез лицензиран пощенски оператор, като вътрешна куриерска пратка....



        [Comment("Определя активния запис за CandidateProvider")]
        public bool IsActive { get; set; }


        [Comment("Уникален идентификатор за връзка с деловодната система на НАПОО")]                
        public long? UIN { get; set; }

        [Comment("Име на директор на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Име на директор на ЦПО/ЦИПО' не може да съдържа повече от 50 символа.")]
        public string? DirectorFirstName { get; set; }

        [Comment("Презиме на директор на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Презиме на директор ЦПО/ЦИПО' не може да съдържа повече от 50 символа.")]
        public string? DirectorSecondName { get; set; }

        [Comment("Фамилия директор на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Фамилия на директор ЦПО/ЦИПО' не може да съдържа повече от 50 символа.")]
        public string? DirectorFamilyName { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Comment("Информация къде се съхранява архивът на ЦПО/ЦИПО при отнемане на лицензия")]
        public string? Archive { get; set; }


		[Comment("Искане на допълнителни документи от Водещ експерт")]
		public bool AdditionalDocumentRequested { get; set; }

		#endregion

		#region Данни за контакт с ЦПО
		[StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Лице за контакт/кореспондениця' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Лице за контакт/кореспондениця")]
        public string? PersonNameCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Лице за контакт/кореспондениця на латиница' не може да съдържа повече от 100 символа.")]
        [Comment( "Лице за контакт/кореспондениця на латиница")]
        public string? PersonNameCorrespondenceEN { get; set; }


        [Display(Name = "Населено място за кореспондениця на ЦПО,ЦИПО")]
        [ForeignKey(nameof(LocationCorrespondence))]
        public int? IdLocationCorrespondence { get; set; }
        public Location LocationCorrespondence { get; set; }

        [Display(Name = "Административен район за кореспондениця на ЦПО,ЦИПО")]
        [ForeignKey(nameof(RegionCorrespondence))]
        public int? IdRegionCorrespondence { get; set; }
        public Region RegionCorrespondence { get; set; }

        [Display(Name = "Адрес за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес за кореспонденция   на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderAddressCorrespondence { get; set; }


        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес за кореспонденция   на ЦПО,ЦИПО на латиница' не може да съдържа повече от 255 символа.")]
        [Comment( "Адрес за кореспонденция   на ЦПО,ЦИПО на латиница")] 
        public string? ProviderAddressCorrespondenceEN { get; set; }

        [Display(Name = "Пощенски код за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код за кореспонденция   на ЦПО,ЦИПО' не може да съдържа повече от 4 символа.")]
        public string? ZipCodeCorrespondence { get; set; }

        [Display(Name = "Телефон за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон за кореспонденция с ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderPhoneCorrespondence { get; set; }


        [Display(Name = "Факс за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Факс за кореспонденция с ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderFaxCorrespondence { get; set; }

        [Display(Name = "E-mail за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail за кореспонденция с ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderEmailCorrespondence { get; set; }

        [Display(Name = "Токен за валидация ")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Токен за валидация ' не може да съдържа повече от 255 символа.")]
        public string? Token { get; set; }


        [Display(Name = "Връзка с данни за Процедура за лицензиране")]
        [ForeignKey(nameof(StartedProcedure))]
        public int? IdStartedProcedure { get; set; }
        public StartedProcedure StartedProcedure { get; set; }


        [Comment("Връзка с активния канидат провайдър")]
        [ForeignKey(nameof(CandidateProviderActive))]
        public int? IdCandidateProviderActive { get; set; }
        public CandidateProvider CandidateProviderActive { get; set; }


        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до електронно подписанато заявление' не може да съдържа повече от 512 символа.")]
        [Comment("Път до електронно подписанато заявление")]
        public string? ESignApplicationFileName { get; set; }


        #endregion

        public virtual ICollection<CandidateProviderSpeciality> CandidateProviderSpecialities { get; set; }
        public virtual ICollection<CandidateProviderPerson> CandidateProviderPersons { get; set; }
        public virtual ICollection<CandidateProviderTrainer> CandidateProviderTrainers { get; set; }
        public virtual ICollection<CandidateProviderStatus> CandidateProviderStatuses { get; set; }
        public virtual ICollection<CandidateProviderPremises> CandidateProviderPremises { get; set; }
        public virtual ICollection<CandidateProviderDocument> CandidateProviderDocuments { get; set; }
        public virtual ICollection<RequestReport> RequestReports { get; set; }

        public virtual ICollection<ProviderDocumentOffer> ProviderDocumentOffers { get; set; }

        public virtual ICollection<AnnualInfo> AnnualInfos { get; set; }
        public virtual ICollection<Program> Programs { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<CandidateProviderConsulting> CandidateProviderConsultings { get; set; }

        public virtual ICollection<ValidationClient> ValidationClients { get; set; }
        public virtual ICollection<SelfAssessmentReport> SelfAssessmentReports { get; set; }
        public virtual ICollection<CandidateProviderCPOStructureActivity> CandidateProviderCPOStructureAndActivities { get; set; }
        public virtual ICollection<CandidateProviderCIPOStructureActivity> CandidateProviderCIPOStructureAndActivities { get; set; }
        public virtual ICollection<ProviderRequestDocument> ProviderRequestDocuments { get; set; }
        public virtual ICollection<FollowUpControl> FollowUpControls { get; set; }

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

        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion
    }
}
