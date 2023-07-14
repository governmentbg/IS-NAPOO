using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{
     

    /// <summary>
    /// Лице
    /// </summary>
    [Table("Candidate_ProviderTrainer")]
    [Display(Name = "Преподавател")]
    public class CandidateProviderTrainer : IEntity, IModifiable, IDataMigration    
    {
        public CandidateProviderTrainer()
        {
            this.FirstName = string.Empty;
            this.SecondName = string.Empty;
            this.FamilyName = string.Empty;

            this.CandidateProviderTrainerProfiles = new HashSet<CandidateProviderTrainerProfile>();
            this.CandidateProviderTrainerQualifications = new HashSet<CandidateProviderTrainerQualification>();
            this.CandidateProviderTrainerDocuments = new HashSet<CandidateProviderTrainerDocument>();
            this.CandidateProviderTrainerSpecialities = new HashSet<CandidateProviderTrainerSpeciality>();
            this.CandidateProviderTrainerCheckings = new HashSet<CandidateProviderTrainerChecking>();
        }

        [Key]
        public int IdCandidateProviderTrainer { get; set; }
        public int IdEntity => IdCandidateProviderTrainer;

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidate_Provider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

      
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Презиме")]
        public string? SecondName { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }

        [Display(Name = "Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето ЕГН/ЛНЧ/ИДН трябва да съдържа 10 символа!")]
        [Display(Name = "ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Display(Name = "Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Пол")]
        public int? IdSex { get; set; }

        [Display(Name = "Гражданство")]        
        public int? IdNationality { get; set; }


        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail' не може да съдържа повече от 255 символа.")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Образователно-квалификационна степен")]
        public int IdEducation { get; set; }//Таблица 'code_education' от страта база. Примерни стойности: висше - магистър, висше - бакалавър, висше - професионален бакалавър, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование

        [Display(Name = "Свидетелство за професионална квалификация")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Свидетелство за професионална квалификация' не може да съдържа повече от 50 символа!")]
        public string? ProfessionalQualificationCertificate { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Специалност по диплома' не може да съдържа повече от 255 символа!")]
        [Display(Name = "Специалност по диплома")]
        public string? EducationSpecialityNotes { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Свидетелство за правоспособност' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Свидетелство за правоспособност")]
        public string? EducationCertificateNotes { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Специална научна подготовка' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Специална научна подготовка")]
        public string? EducationAcademicNotes { get; set; }

        [Display(Name = "Андрагогическа квалификация")]
        public bool IsAndragog { get; set; }//(Да/Не)
       
        [Display(Name = "Вид на договора")]
        public int? IdContractType { get; set; }//Таблица code_ccontract_type от страта база. Примерни стойности:договор за работа, договор за индивидуална дейност, без осигуренoосигурено работно място new keytype TrainerContractType


        [Display(Name = "Дата на договора")]
        public DateTime? ContractDate { get; set; }


        [Display(Name = "Статус")]
        public int? IdStatus { get; set; }//Активен/Неактивен

        [Comment("Дата на деактивиране на преподавателя/консултанта")]
        [Display(Name = "Дата на деактивиране на преподавателя/консултанта")]
        public DateTime? InactiveDate { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на диплома' не може да съдържа повече от 20 символа.")]
        [Comment("Номер на диплома")]
        public string? DiplomaNumber { get; set; }

        public virtual ICollection<CandidateProviderTrainerProfile> CandidateProviderTrainerProfiles { get; set; }

        public virtual ICollection<CandidateProviderTrainerQualification> CandidateProviderTrainerQualifications { get; set; }

        public virtual ICollection<CandidateProviderTrainerDocument> CandidateProviderTrainerDocuments { get; set; }
        public virtual ICollection<CandidateProviderTrainerSpeciality> CandidateProviderTrainerSpecialities { get; set; }
        public virtual ICollection<CandidateProviderTrainerChecking> CandidateProviderTrainerCheckings { get; set; }


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
