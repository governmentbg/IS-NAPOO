using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Contracts;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.SPPOO;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Register
{
    public class RegisterTrainerModalVM
    {
        public RegisterTrainerModalVM()
        {
            this.CandidateProviderTrainerProfiles = new HashSet<CandidateProviderTrainerProfileVM>();
            this.CandidateProviderTrainerQualifications = new HashSet<CandidateProviderTrainerQualificationVM>();
            this.CandidateProviderTrainerDocuments = new HashSet<CandidateProviderTrainerDocumentVM>();
            this.CandidateProviderTrainerSpecialities = new HashSet<CandidateProviderTrainerSpecialityVM>();
            this.SelectedSpecialities = new List<SpecialityVM>();
        }

        public int IdCandidateProviderTrainer { get; set; }

        [Display(Name = "Връзка с  CPO,CIPO - Кандидат Обучаваща институция")]
        public string? IdCandidate_Provider { get; set; }
        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Име")]
        public string? FirstName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Презиме")]
        public string? SecondName { get; set; }

        //[Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Фамилия")]
        public string? FamilyName { get; set; }

        [Display(Name = "Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public string? IdIndentType { get; set; }

        [Display(Name = "ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Display(Name = "Дата на раждане")]
        public string? BirthDate { get; set; }

        [Display(Name = "Пол")]
        public string? IdSex { get; set; }

        [Display(Name = "Гражданство")]
        public string? IdNationality { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail' не може да съдържа повече от 255 символа.")]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string? Email { get; set; }

        [Required]
        [Display(Name = "Образователно-квалификационна степен")]
        public string? IdEducation { get; set; }//Таблица 'code_education' от страта база. Примерни стойности: висше - магистър, висше - бакалавър, висше - професионален бакалавър, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование

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
        public string? IdContractType { get; set; }//Таблица code_ccontract_type от страта база. Примерни стойности:договор за работа, договор за индивидуална дейност, без осигуренoосигурено работно място, с договор по програма “ОСПОЗ”, с договор по програма “АХУ”

        [Display(Name = "Дата на договора")]
        public string? ContractDate { get; set; }

        [Display(Name = "Статус")]
        public string? IdStatus { get; set; }//Активен/Неактивен

        public string? FullName => $"{this.FirstName} {this.SecondName} {this.FamilyName}";

        public List<SpecialityVM>? SelectedSpecialities { get; set; }

        public virtual ICollection<CandidateProviderTrainerProfileVM>? CandidateProviderTrainerProfiles { get; set; }

        public virtual ICollection<CandidateProviderTrainerQualificationVM>? CandidateProviderTrainerQualifications { get; set; }

        public virtual ICollection<CandidateProviderTrainerDocumentVM>? CandidateProviderTrainerDocuments { get; set; }

        public virtual ICollection<CandidateProviderTrainerSpecialityVM>? CandidateProviderTrainerSpecialities { get; set; }

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


