using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderTrainerVM: IEmpty, IEquatable<CandidateProviderTrainerVM?>
    {
        public CandidateProviderTrainerVM()
        {
            this.CandidateProviderTrainerProfiles = new HashSet<CandidateProviderTrainerProfileVM>();
            this.CandidateProviderTrainerQualifications = new HashSet<CandidateProviderTrainerQualificationVM>();
            this.CandidateProviderTrainerDocuments = new HashSet<CandidateProviderTrainerDocumentVM>();
            this.CandidateProviderTrainerSpecialities = new HashSet<CandidateProviderTrainerSpecialityVM>();
            this.CandidateProviderTrainerCheckings = new HashSet<CandidateProviderTrainerCheckingVM>();
            this.SelectedSpecialities = new List<SpecialityVM>();
        }

        public int IdCandidateProviderTrainer { get; set; }

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        public int IdCandidate_Provider { get; set; }
        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Име' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }

        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Презиме' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Презиме")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Фамилия' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }

        [Display(Name = "Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

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
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Образователно-квалификационна степен")]
        public int IdEducation { get; set; }//Таблица 'code_education' от страта база. Примерни стойности: висше - магистър, висше - бакалавър, висше - професионален бакалавър, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование

        public string EducationValue { get; set; }

        [Display(Name = "Свидетелство за професионална квалификация")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Свидетелство за професионална квалификация' не може да съдържа повече от 50 символа!")]
        public string? ProfessionalQualificationCertificate { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Специалност по диплома' не може да съдържа повече от 255 символа.")]
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
        public int? IdContractType { get; set; }//Таблица code_ccontract_type от страта база. Примерни стойности:договор за работа, договор за индивидуална дейност, без осигуренoосигурено работно място, с договор по програма “ОСПОЗ”, с договор по програма “АХУ”

        [Display(Name = "Дата на договора")]
        public DateTime? ContractDate { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Номер на диплома' не може да съдържа повече от 20 символа.")]
        [Comment("Номер на диплома")]
        public string? DiplomaNumber { get; set; }

        [Display(Name = "Статус")]
        [Required(ErrorMessage = "Полето 'Статус' е задължително!")]
        public int? IdStatus { get; set; } //Активен/Неактивен 

        [Comment("Дата на деактивиране на преподавателя/консултанта")]
        [Display(Name = "Дата на деактивиране на преподавателя/консултанта")]
        public DateTime? InactiveDate { get; set; }

        public string StatusName { get; set; }

        public string ModifyPersonName { get; set; }

        public string CreatePersonName { get; set; }

        public string FullName => $"{this.FirstName} {this.SecondName} {this.FamilyName}";

        public List<SpecialityVM> SelectedSpecialities { get; set; }

        public virtual ICollection<CandidateProviderTrainerProfileVM> CandidateProviderTrainerProfiles { get; set; }

        public virtual ICollection<CandidateProviderTrainerQualificationVM> CandidateProviderTrainerQualifications { get; set; }

        public virtual ICollection<CandidateProviderTrainerDocumentVM> CandidateProviderTrainerDocuments { get; set; }

        public virtual ICollection<CandidateProviderTrainerSpecialityVM> CandidateProviderTrainerSpecialities { get; set; }

        public virtual ICollection<CandidateProviderTrainerCheckingVM> CandidateProviderTrainerCheckings { get; set; }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(this.FirstName) && string.IsNullOrEmpty(this.SecondName) && string.IsNullOrEmpty(this.FamilyName);
        }

        public int GetEmptyHashCode()
        {
            return new CandidateProviderTrainerVM().GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as CandidateProviderTrainerVM);
        }

        public bool Equals(CandidateProviderTrainerVM? other)
        {
            return other != null &&
                   IdCandidateProviderTrainer == other.IdCandidateProviderTrainer &&
                   IdCandidate_Provider == other.IdCandidate_Provider &&
                   EqualityComparer<CandidateProviderVM>.Default.Equals(CandidateProvider, other.CandidateProvider) &&
                   FirstName == other.FirstName &&
                   SecondName == other.SecondName &&
                   FamilyName == other.FamilyName &&
                   IdIndentType == other.IdIndentType &&
                   Indent == other.Indent &&
                   BirthDate == other.BirthDate &&
                   IdSex == other.IdSex &&
                   IdNationality == other.IdNationality &&
                   Email == other.Email &&
                   IdEducation == other.IdEducation &&
                   EducationSpecialityNotes == other.EducationSpecialityNotes &&
                   EducationCertificateNotes == other.EducationCertificateNotes &&
                   EducationAcademicNotes == other.EducationAcademicNotes &&
                   IsAndragog == other.IsAndragog &&
                   IdContractType == other.IdContractType &&
                   ContractDate == other.ContractDate &&
                   IdStatus == other.IdStatus &&
                   FullName == other.FullName &&
                   EqualityComparer<ICollection<CandidateProviderTrainerProfileVM>>.Default.Equals(CandidateProviderTrainerProfiles, other.CandidateProviderTrainerProfiles) &&
                   EqualityComparer<ICollection<CandidateProviderTrainerQualificationVM>>.Default.Equals(CandidateProviderTrainerQualifications, other.CandidateProviderTrainerQualifications) &&
                   EqualityComparer<ICollection<CandidateProviderTrainerDocumentVM>>.Default.Equals(CandidateProviderTrainerDocuments, other.CandidateProviderTrainerDocuments) &&
                   IdCreateUser == other.IdCreateUser &&
                   CreationDate == other.CreationDate &&
                   IdModifyUser == other.IdModifyUser &&
                   ModifyDate == other.ModifyDate;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(IdCandidateProviderTrainer);
            hash.Add(IdCandidate_Provider);
            hash.Add(CandidateProvider);
            hash.Add(FirstName);
            hash.Add(SecondName);
            hash.Add(FamilyName);
            hash.Add(IdIndentType);
            hash.Add(Indent);
            hash.Add(BirthDate);
            hash.Add(IdSex);
            hash.Add(IdNationality);
            hash.Add(Email);
            hash.Add(IdEducation);
            hash.Add(EducationSpecialityNotes);
            hash.Add(EducationCertificateNotes);
            hash.Add(EducationAcademicNotes);
            hash.Add(IsAndragog);
            hash.Add(IdContractType);
            hash.Add(ContractDate);
            hash.Add(IdStatus);
            hash.Add(FullName);
            hash.Add(CandidateProviderTrainerProfiles);
            hash.Add(CandidateProviderTrainerQualifications);
            hash.Add(CandidateProviderTrainerDocuments);
            hash.Add(IdCreateUser);
            hash.Add(CreationDate);
            hash.Add(IdModifyUser);
            hash.Add(ModifyDate);
            return hash.ToHashCode();
        }

        public string SexValue { get; set; }

        public string IndentTypeValue { get; set; }

        public string NationalityValue { get; set; }

        public string ContractTypeValue { get; set; }

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
