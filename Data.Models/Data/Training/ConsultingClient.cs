using Data.Models.Data.Candidate;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Получател на услугата(консултиран) по дейност от ЦИПО
    /// </summary>
    [Table("Training_ConsultingClient")]
    [Comment("Получател на услугата(консултиран) по дейност от ЦИПО")]
    public class ConsultingClient : IEntity, IModifiable
    {
        public ConsultingClient()
        {
            this.ConsultingDocumentUploadedFiles = new HashSet<ConsultingDocumentUploadedFile>();
            this.Consultings = new HashSet<Consulting>();
            this.ConsultingTrainers = new HashSet<ConsultingTrainer>();
            this.ConsultingPremises = new HashSet<ConsultingPremises>();
            this.ConsultingClientRequiredDocuments = new HashSet<ConsultingClientRequiredDocument>();
        }

        [Key]
        public int IdConsultingClient { get; set; }
        public int IdEntity => IdConsultingClient;

        [Comment("Връзка с  Получател на услугата(обучаем)")]
        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; }

        public Client Client { get; set; }

        [Comment("Връзка с CandidateProvider")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProvider CandidateProvider { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Име")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Презиме")]
        public string? SecondName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Фамилия")]
        public string FamilyName { get; set; }

        [Comment("Пол")]
        public int? IdSex { get; set; }

        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength10)]
        [Comment("ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Comment("Гражданство")]
        public int? IdNationality { get; set; }

        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)

        [Comment("Приключване на курс")]
        public int? IdFinishedType { get; set; }//Таблица 'code_education' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат

        [Comment("Дата на стартиране на консултацията")]
        public DateTime? StartDate { get; set; }

        [Comment("Дата на приключване на консултацията")]
        public DateTime? EndDate { get; set; }

        [Comment("Съгласие за използване на информацията за контакт от НАПОО")]
        public bool IsContactAllowed { get; set; }

        [Comment("Лице с увреждания")]
        public bool IsDisabledPerson { get; set; }

        [Comment("Лице в неравностойно положение")]
        public bool IsDisadvantagedPerson { get; set; }

        [Comment("Адрес")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Address { get; set; }

        [Comment("E-mail адрес")]
        [StringLength(DBStringLength.StringLength100)]
        public string? EmailAddress { get; set; }

        [Comment("Телефон")]
        [StringLength(DBStringLength.StringLength20)]
        public string? Phone { get; set; }

        [Comment("Учащ")]
        public bool IsStudent { get; set; }

        [Comment("Заето лице")]
        public bool IsEmployedPerson { get; set; }

        [Comment("Вид на регистрация в бюрото по труда")]
        public int? IdRegistrationAtLabourOfficeType { get; set; } // Номенклатура: KeyTypeIntCode - "RegistrationAtLabourOfficeType"

        [Comment("Вид на насочен към услугите на ЦИПО")]
        public int IdAimAtCIPOServicesType { get; set; } // Номенклатура: KeyTypeIntCode - "AimAtCIPOServicesType"

        [Comment("Дали консултираното лице е архивирано")]
        public bool IsArchived { get; set; }

        public virtual ICollection<ConsultingDocumentUploadedFile> ConsultingDocumentUploadedFiles { get; set; }

        public virtual ICollection<Consulting> Consultings { get; set; }

        public virtual ICollection<ConsultingPremises> ConsultingPremises { get; set; }

        public virtual ICollection<ConsultingTrainer> ConsultingTrainers { get; set; }

        public virtual ICollection<ConsultingClientRequiredDocument> ConsultingClientRequiredDocuments { get; set; }

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
