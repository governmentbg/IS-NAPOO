using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Data.Models.Data.Candidate;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ConsultingClientVM
    {
        public ConsultingClientVM()
        {
            this.ConsultingDocumentUploadedFiles = new HashSet<ConsultingDocumentUploadedFileVM>();
            this.ConsultingTrainers = new HashSet<ConsultingTrainerVM>();
            this.ConsultingPremises = new HashSet<ConsultingPremisesVM>();
            this.Consultings = new HashSet<ConsultingVM>();
        }

        public int IdConsultingClient { get; set; }

        [Comment("Връзка с  Получател на услугата(обучаем)")]
        public int IdClient { get; set; }

        public virtual ClientVM Client { get; set; }

        [Comment("Връзка с CandidateProvider")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Comment("Име")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Име' може да съдържа само текст на български език!")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Comment("Презиме")]
        [RegularExpression(@"\p{IsCyrillic}*\s*-*", ErrorMessage = "Полето 'Презиме' може да съдържа само текст на български език!")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Comment("Фамилия")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Фамилия' може да съдържа само текст на български език!")]
        public string FamilyName { get; set; }

        [Required(ErrorMessage = "Полето 'Пол' е задължително!")]
        [Comment("Пол")]
        public int? IdSex { get; set; }

        [Required(ErrorMessage = "Полето 'Вид на идентификатора' е задължително!")]
        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength10)]
        [Comment("ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на раждане' е задължително!")]
        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Полето 'Гражданство' е задължително!")]
        [Comment("Гражданство")]
        public int? IdNationality { get; set; }

        [Required(ErrorMessage = "Полето 'Начин на финансиране' е задължително!")]
        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)

        [Comment("Приключване на консултация")]
        public int? IdFinishedType { get; set; }//Таблица 'code_education' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат

        [Required(ErrorMessage = "Полето 'Дата на стартиране' е задължително!")]
        [Comment("Дата на стартиране на консултацията")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на приключване' е задължително!")]
        [Comment("Дата на приключване на консултацията")]
        public DateTime? EndDate { get; set; }

        [Comment("Съгласие за използване на информацията за контакт от НАПОО")]
        public bool IsContactAllowed { get; set; }

        [Comment("Лице с увреждания")]
        public bool IsDisabledPerson { get; set; }

        [Comment("Лице в неравностойно положение")]
        public bool IsDisadvantagedPerson { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес' може да съдържа до 255 символа!")]
        public string? Address { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'E-mail адрес' може да съдържа до 100 символа!")]
        public string? EmailAddress { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Телефон' може да съдържа до 20 символа!")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Полето 'Учащ/Неучащ' е задължително!")]
        [Comment("Учащ")]
        public bool? IsStudent { get; set; }

        [Required(ErrorMessage = "Полето 'Заето лице/Безработно лице' е задължително!")]
        [Comment("Заето лице")]
        public bool? IsEmployedPerson { get; set; }

        [Comment("Вид на регистрация в бюрото по труда")]
        public int? IdRegistrationAtLabourOfficeType { get; set; } // Номенклатура: KeyTypeIntCode - "RegistrationAtLabourOfficeType"

        [Required(ErrorMessage = "Полето 'Насочен към услугите на ЦИПО' е задължително!")]
        [Comment("Вид на насочен към услугите на ЦИПО")]
        public int? IdAimAtCIPOServicesType { get; set; } // Номенклатура: KeyTypeIntCode - "AimAtCIPOServicesType"

        public string StartDateAsStr => this.StartDate.HasValue ? $"{this.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        public string EndDateAsStr => this.EndDate.HasValue ? $"{this.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        public string Period => $"{this.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г. - {this.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";

        public string IndentType { get; set; }

        public string FullName => !string.IsNullOrEmpty(this.SecondName) ? $"{this.FirstName} {this.SecondName} {this.FamilyName}" : $"{this.FirstName} {this.FamilyName}";

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

        public virtual ICollection<ConsultingDocumentUploadedFileVM> ConsultingDocumentUploadedFiles { get; set; }

        public virtual ICollection<ConsultingVM> Consultings { get; set; }

        public virtual ICollection<ConsultingPremisesVM> ConsultingPremises { get; set; }

        public virtual ICollection<ConsultingTrainerVM> ConsultingTrainers { get; set; }
        public int? IdConsultingType { get; set; }
        public int? IdConsultingReceiveType { get; set; }

        [Comment("Дали консултираното лице е архивирано")]
        public bool IsArchived { get; set; }

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
