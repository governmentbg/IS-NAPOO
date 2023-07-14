using Data.Models.Common;
using Data.Models.Data.Archive;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Control;
using Data.Models.Data.Framework;
using Data.Models.Data.Request;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Archive;
using ISNAPOO.Core.ViewModels.Common;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.Control;
using ISNAPOO.Core.ViewModels.CPO.ProviderData;
using ISNAPOO.Core.ViewModels.EGovPayment;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Request;
using ISNAPOO.Core.ViewModels.Training;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderVM
    {
        public CandidateProviderVM()
        {
            this.UploadedFileName = string.Empty;
            this.CandidateProviderSpecialities = new HashSet<CandidateProviderSpecialityVM>();
            this.CandidateProviderTrainers = new HashSet<CandidateProviderTrainerVM>();
            this.CandidateProviderPremises = new HashSet<CandidateProviderPremisesVM>();
            this.CandidateProviderDocuments = new HashSet<CandidateProviderDocumentVM>();
            this.PersonsForNotifications = new List<PersonVM>();
            this.RequestReports = new HashSet<RequestReportVM>();
            this.ReportUploadedDocs = new HashSet<ReportUploadedDocVM>();
            this.CandidateProviderPersons = new HashSet<CandidateProviderPersonVM>();
            this.AnnualInfos = new HashSet<AnnualInfoVM>();
            this.Programs = new HashSet<ProgramVM>();
            this.CandidateProviderConsultings = new HashSet<CandidateProviderConsultingVM>();
            this.StartedProcedureIds = new HashSet<int>();
            this.ValidationClients = new HashSet<ValidationClient>();
            this.SelfAssessmentReports = new HashSet<SelfAssessmentReportVM>();
            this.ProviderRequestDocuments = new HashSet<ProviderRequestDocumentVM>();
            this.FollowUpControls = new HashSet<FollowUpControlVM>();
            this.Courses = new HashSet<CourseVM>();
            this.Payments = new HashSet<PaymentVM>();
            this.ApplicationStatusFilter_IN_List = new List<int>();
            this.ApplicationStatusFilter_NOT_IN_List = new List<int>();
            this.LocationCorrespondence = new LocationVM();
        }

        [Key]
        public int IdCandidate_Provider { get; set; }

        public virtual ICollection<CandidateProviderSpecialityVM> CandidateProviderSpecialities { get; set; }

        public string ExportDataAsStr => @$"адрес: {this.ProviderAddressCorrespondence}; лице за контакт: {this.PersonNameCorrespondence}; телефон: {this.ProviderPhoneCorrespondence}; e-mail: {this.ProviderEmailCorrespondence}";

        public virtual ICollection<CandidateProviderTrainerVM> CandidateProviderTrainers { get; set; }

        public virtual ICollection<CandidateProviderPremisesVM> CandidateProviderPremises { get; set; }

        public virtual ICollection<CandidateProviderDocumentVM> CandidateProviderDocuments { get; set; }

        public virtual ICollection<RequestReportVM> RequestReports { get; set; }

        public virtual ICollection<ReportUploadedDocVM> ReportUploadedDocs { get; set; }

        public virtual ICollection<CandidateProviderPersonVM> CandidateProviderPersons { get; set; }
        public virtual ICollection<AnnualInfoVM> AnnualInfos { get; set; }
        public virtual ICollection<SelfAssessmentReportVM> SelfAssessmentReports { get; set; }
        public virtual ICollection<ProgramVM> Programs { get; set; }
        public virtual ICollection<ValidationClient> ValidationClients { get; set; }

        public virtual ICollection<CandidateProviderConsultingVM> CandidateProviderConsultings { get; set; }

        public virtual ICollection<ProviderRequestDocumentVM> ProviderRequestDocuments { get; set; }

        public virtual ICollection<FollowUpControlVM> FollowUpControls { get; set; }

        public virtual ICollection<CourseVM> Courses { get; set; }

        public virtual ICollection<PaymentVM> Payments { get; set; }
        public List<PersonVM> PersonsForNotifications { get; set; }

        public string CandidateProviderTrainerNames { get; set; }

        public bool IsActive { get; set; }

        public bool SkipIsActive { get; set; }

        [Comment("Начин на получаване на административен акт и лицензия")]
        public int? IdReceiveLicense { get; set; }// На  място  в  звеното  за  административно  обслужване  на НАПОО, Чрез  лицензиран  пощенски оператор,  като  вътрешна куриерска пратка, на адреса, изписан като адрес за кореспонденция с центъра за професионално обучение, и декларирам, че пощенските разходи са за моя сметка, като давам съгласие документите да бъдат пренасяни за служебни цели .....

        public string ReceiveLicenseType { get; set; }

        [Comment("Начин на подаване на заявление и документ за платена държавна такса")]
        public int? IdApplicationFiling { get; set; }

        public string ApplicationFilingType { get; set; }

        public string ValidationForAddedSpecialities { get; set; }

        public string LicenceNumberWithDate { get { return $"{LicenceNumber}/{(LicenceDate != null ? LicenceDate.Value.ToString(GlobalConstants.DATE_FORMAT) : "")} г."; } }

        #region Данни за Юридическо лица
        [Required(ErrorMessage = "Полето 'Наименование на юридическото лице' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на юридическото лице' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Наименование на юридическото лице")]
        public string ProviderOwner { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на юридическото лице на латиница' не може да съдържа повече от 255 символа.")]
        [Comment("Наименование на юридическото лице на латиница")]
        public string? ProviderOwnerEN { get; set; }

        [Required(ErrorMessage = "Полето 'ЕИК/Булстат' е задължително!")]
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

        [Display(Name = "Връзка с данни за Процедура за лицензиране")]
        public int? IdStartedProcedure { get; set; }
        public StartedProcedureVM StartedProcedure { get; set; }


        /// <summary>
        /// "Регистър Булстат: ЮЛНЦ в частна полза",
        /// "Регистър Булстат: държавно предприятие",
        /// "Централен регистър на МП: ЮЛНЦ в обществена полза",
        /// "Рeгистри на МОН: средни и висши училища",
        /// "Чуждестранно юридическо лице",
        /// "Търговски регистър" 
        /// </summary>
        [Display(Name = "Регистрирано в")]
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Регистрирано в' е задължително!")]
        public int IdProviderRegistration { get; set; }


        /// <summary>
        /// "държавна"
        /// "общинска"
        /// "частна"
        /// </summary>
        [Display(Name = "Форма на собственост")]
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Форма на собственост' е задължително!")]
        public int IdProviderOwnership { get; set; }
        public KeyValueVM? ProviderOwnership { get; set; }
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
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Вид на обучаващата институция' е задължително!")]
        public int IdProviderStatus { get; set; }

        public LocationVM Location { get; set; }

        public int? IdLocation { get; set; }

        [Display(Name = "Адрес по регистрация")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес по регистрация' не може да съдържа повече от 255 символа.")]
        public string ProviderAddress { get; set; }

        [Display(Name = "Адрес по регистрация на латиница")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес по регистрация' не може да съдържа повече от 255 символа.")]
        public string ProviderAddressEN { get; set; }

        [Display(Name = "Пощенски код")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 4 символа!")]
        public string ZipCode { get; set; }

        [Display(Name = "Административен район за ЮЛ")]
        public int? IdRegionAdmin { get; set; }

        public virtual RegionVM RegionAdmin { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до нотариално пълномощно' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Път до нотариално пълномощно")]
        public string UploadedFileName { get; set; }

        public bool HasUploadedFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts.Last() : string.Empty);
                }
                else
                {
                    return string.Empty;
                }
            }
        }
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

        public string ApplicationDateFormated { get { return this.ApplicationDate.HasValue ? $"{this.ApplicationDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty; } }

        [Display(Name = "Номер на лиценза")]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на лиценз' не може да съдържа повече от 20 символа!")]
        public string? LicenceNumber { get; set; }

        public string LicenceNumberString => LicenceNumber is null ? string.Empty : LicenceNumber.ToString();

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
        public string LicenceStatusName { get; set; }

        [Display(Name = "Телефон на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон на юридическото лице' не може да съдържа повече от 255 символа!")]
        [Required(ErrorMessage = "Полето 'Телефон' е задължително!")]
        public string? ProviderPhone { get; set; }


        [Display(Name = "Факс на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Факс на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderFax { get; set; }

        [Display(Name = "Интернет страница на административния офис на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Интернет страница на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderWeb { get; set; }

        [Display(Name = "E-mail на административния офис на ЦПО,ЦИПО")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail на административния офис на ЦПО,ЦИПО' не може да съдържа повече от 255 символа!")]
        [Required(ErrorMessage = "Полето 'E-mail адрес' е задължително!")]
        public string? ProviderEmail { get; set; }



        [Display(Name = "Допълнителна информация")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Допълнителна информация' не може да съдържа повече от 512 символа!")]
        public string? AdditionalInfo { get; set; }

        [Display(Name = "Достъпност на архитектурна среда за хора с увреждания, жестомимичен превод")]
        public bool AccessibilityInfo { get; set; }


        [Display(Name = "Условия за онлайн обучение")]
        public bool OnlineTrainingInfo { get; set; }

        [Display(Name = "Вид на заявлението")]
        public int? IdTypeApplication { get; set; }//Първоначално лицензиране/Изменение на лицензията

        public string TypeApplication { get; set; }

        [Display(Name = "Статус на обработка на заявлението")]
        public int? IdApplicationStatus { get; set; }//Подадено от ЦПО/ЦПИО,Одобрено НАПОО, Автоматично одобрено, Регистрирано

        public string ApplicationStatus { get; set; }

        /// <summary>
        /// Използва се за филтиране по статус
        /// </summary>
        public List<int> ApplicationStatusFilter_IN_List { get; set; }

        /// <summary>
        /// Използва се за филтиране по статус
        /// </summary>
        public List<int> ApplicationStatusFilter_NOT_IN_List { get; set; }

        public string LicenceTypeValue { get; set; }


        /// <summary>
        /// Очаква се валидация на електронна поща
        /// Успешна валидация на електронна поща
        /// Очаква потвърждение от НАПОО
        /// Одобрена заявка от НАПОО
        /// Автоматично одобрение
        /// Създаден потребител 
        /// </summary>
        [Comment("Статус на регистрация на заявлението")]
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

        [Comment("Уникален идентификатор за връзка с деловодната система на НАПОО")]
        public long? UIN { get; set; }

        [Required(ErrorMessage = "Полето 'Директор на ЦПО/ЦИПО (Име)' е задължително!")]
        [Comment("Име на директор на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Име на директор на ЦПО/ЦИПО' не може да съдържа повече от 50 символа.")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Име на директор на ЦПО/ЦИПО' може да съдържа само текст на български език!")]
        public string? DirectorFirstName { get; set; }

        [Required(ErrorMessage = "Полето 'Директор на ЦПО/ЦИПО (Презиме)' е задължително!")]
        [Comment("Презиме на директор на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Презиме на директор ЦПО/ЦИПО' не може да съдържа повече от 50 символа.")]
        [RegularExpression(@"\p{IsCyrillic}*\s*-*", ErrorMessage = "Полето 'Презиме на директор ЦПО/ЦИПО' може да съдържа само текст на български език!")]
        public string? DirectorSecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Директор на ЦПО/ЦИПО (Фамилия)' е задължително!")]
        [Comment("Фамилия директор на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Фамилия на директор ЦПО/ЦИПО' не може да съдържа повече от 50 символа.")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Фамилия на директор ЦПО/ЦИПО' може да съдържа само текст на български език!")]
        public string? DirectorFamilyName { get; set; }
        #endregion

        #region Данни за контакт с ЦПО
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Лице за контакт' не може да съдържа повече от 100 символа!")]
        [Display(Name = "Лице за контакт/кореспондениця")]
        [Required(ErrorMessage = "Полето 'Лице за контакт' е задължително!")]
        public string? PersonNameCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Лице за контакт/кореспондениця на латиница' не може да съдържа повече от 100 символа.")]
        [Comment("Лице за контакт/кореспондениця на латиница")]
        public string? PersonNameCorrespondenceEN { get; set; }

        [Display(Name = "Населено място за кореспондениця на ЦПО,ЦИПО")]
        public LocationVM LocationCorrespondence { get; set; }

        [Required(ErrorMessage = "Полето 'Населено място' е задължително!")]
        [Range(1, double.MaxValue, ErrorMessage = "Полето 'Населено място' е задължително!")]
        public int? IdLocationCorrespondence { get; set; }

        [Display(Name = "Административен район за кореспондениця на ЦПО,ЦИПО")]
        public int? IdRegionCorrespondence { get; set; }

        public virtual RegionVM RegionCorrespondence { get; set; }

        [Display(Name = "Адрес за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес за кореспонденция' не може да съдържа повече от 255 символа!")]
        [Required(ErrorMessage = "Полето 'Адрес за кореспонденция' е задължително!")]
        public string? ProviderAddressCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес за кореспонденция   на ЦПО,ЦИПО на латиница' не може да съдържа повече от 255 символа.")]
        [Comment("Адрес за кореспонденция   на ЦПО,ЦИПО на латиница")]
        public string? ProviderAddressCorrespondenceEN { get; set; }

        [Display(Name = "Пощенски код за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 4 цифри!")]
        [Required(ErrorMessage = "Полето 'Пощенски код' е задължително!")]
        public string? ZipCodeCorrespondence { get; set; }

        [Display(Name = "Телефон за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон на ЦПО/ЦИПО' не може да съдържа повече от 255 символа!")]
        [Required(ErrorMessage = "Полето 'Телефон на ЦПО/ЦИПО' е задължително!")]
        public string? ProviderPhoneCorrespondence { get; set; }


        [Display(Name = "Факс за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Факс' не може да съдържа повече от 255 символа!")]
        public string? ProviderFaxCorrespondence { get; set; }

        [Display(Name = "E-mail за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail адрес' не може да съдържа повече от 255 символа!")]
        [Required(ErrorMessage = "Полето 'E-mail адрес' е задължително!")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string? ProviderEmailCorrespondence { get; set; }

        [Display(Name = "Токен за валидация ")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Токен' не може да съдържа повече от 255 символа!")]
        public string? Token { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Титла' не може да съдържа повече от 20 символа!")]
        [Display(Name = "Титла")]
        public string? Title { get; set; }

        [Display(Name = "Връзка с активния канидат провайдър")]
        public int? IdCandidateProviderActive { get; set; }

        public virtual CandidateProviderVM CandidateProviderActive { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до електронно подписанато заявление' не може да съдържа повече от 512 символа.")]
        [Comment("Път до електронно подписанато заявление")]
        public string? ESignApplicationFileName { get; set; }
        #endregion

        public string ProviderNameAndOwnerForRegister { get; set; }

        public string CPONameAndOwnerName => $"{this.ProviderName} {this.ProviderOwner}";

        public string CPONameOwnerAndBulstat => $"{this.ProviderName} {this.ProviderOwner} {this.PoviderBulstat}";
        public string CPONameOwnerGrid => $"ЦПО {this.ProviderName} към {this.ProviderOwner}";
        public string DirectorFullName => this.DirectorFirstName + " " + this.DirectorSecondName + " " + this.DirectorFamilyName;
        public string CIPONameOwnerGrid => $"ЦИПО {this.ProviderName} към {this.ProviderOwner}";
        public string CPOLicenceNumberNameOwnerGrid => $"{this.LicenceNumber} - ЦПО {this.ProviderName} към {this.ProviderOwner}";
        public string CIPOLicenceNumberNameOwnerGrid => $"{this.LicenceNumber} - ЦИПО {this.ProviderName} към {this.ProviderOwner}";
        public string ApplicationNumberDate => this.ApplicationNumber is not null && this.ApplicationDate.HasValue ? this.ApplicationNumber + "/" + this.ApplicationDate.Value.ToString(GlobalConstants.DATE_FORMAT) + "г." : ""; 
        public string MixCPOandCIPONameOwner { get; set; }

        public string CPONameAndOwner => !string.IsNullOrEmpty(this.ProviderName) ? $"ЦПО {this.ProviderName} към {this.ProviderOwner}" : $"ЦПО към {this.ProviderOwner}";

        public string CIPONameAndOwner => !string.IsNullOrEmpty(this.ProviderName) ? $"ЦИПО {this.ProviderName} към {this.ProviderOwner}" : $"ЦИПО към {this.ProviderOwner}";

        public string ProviderJoinedInformation => $"{this.ProviderName}{this.ProviderOwner}{this.PoviderBulstat}{this.LicenceNumber}";

        public string ModifyPersonName { get; set; }

        public string CreatePersonName { get; set; }

        public bool HasAnnualInfo { get; set; }

        [Comment("Статус на отчета за годишна информация - Име")]
        public string AnnualInfoStatusName { get; set; }

        [Comment("Статус на отчета за годишна информация - IntCode")]
        public string AnnualInfoStatusIntCode { get; set; }
        public DateTime? AnnualInfoDate { get; set; }

        [Comment("Статус на доклада за самооценка - Име")]
        public string SelfAssessmentReportStatusName { get; set; }

        [Comment("Статус на доклада за самооценка - IntCode")]
        public string SelfAssessmentReportStatusIntCode { get; set; }

        [Comment("Искане на допълнителни документи от Водещ експерт")]
        public bool AdditionalDocumentRequested { get; set; }
        public DateTime? SelfAssessmentReportStatusDate { get; set; }

        public ICollection<int> StartedProcedureIds { get; set; }

        public bool ShowApplicationGridButtonsIfFirstLicensing { get; set; } = true;

        public string fromPage { get; set; }

        public bool HavePayments => Payments.Any();

        public bool HaveFollowUpControls => FollowUpControls.Any();

        public string HaveFinishedCourses { get; set; }

        #region IDataMigration
        public string? MigrationNote { get; set; }
        [Display(Name = "Старо id в старата система на напоо (Само за миграция)")]
        public long OldId { get; set; }
        #endregion

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
