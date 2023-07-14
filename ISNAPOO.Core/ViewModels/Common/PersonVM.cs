using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.DOC.NKPD;
using ISNAPOO.Core.ViewModels.EKATTE;
using ISNAPOO.Core.ViewModels.Identity;
using ISNAPOO.Core.ViewModels.SPPOO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class PersonVM : IMapFrom<Person>, IMapTo<Person>
    {
        public PersonVM()
        {
            this.TaxOffice = string.Empty;
            this.CandidateProviderPersons = new HashSet<CandidateProviderPersonVM>();
        }

        [Key]
        public int IdPerson { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [RegularExpression(@"^[\p{IsCyrillic}]+[\s-]*[\p{IsCyrillic}]+\s*$", ErrorMessage = "Полето 'Име' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа!")]
        [Display(Name = "Име")]
        public string FirstName { get; set; }


        [RegularExpression(@"^[\p{IsCyrillic}]+[\s-]*[\p{IsCyrillic}]+\s*$", ErrorMessage = "Полето 'Презиме' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Презиме")]
        public string SecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [RegularExpression(@"^[\p{IsCyrillic}]+[\s-]*[\p{IsCyrillic}]+\s*$", ErrorMessage = "Полето 'Фамилия' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }

        [Display(Name = "Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }
        public string IndentTypeName { get; set; }

        [Display(Name = "ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Номер на лична карта' не може да съдържа повече от 10 символа.")]
        [Display(Name = "Номер на лична карта")]
        public string? PersonalID { get; set; }

        public DateTime? PersonalIDDateFrom { get; set; }

        public string PersonalIDDateFromFormated { get { return this.PersonalIDDateFrom.HasValue ? this.PersonalIDDateFrom.Value.ToString(GlobalConstants.DATE_FORMAT) : string.Empty; } }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Издадена от' не може да съдържа повече от 50 символа.")]
        [Display(Name = "Издадена от")]
        public string? PersonalIDIssueBy { get; set; }

        public string TaxOffice { get; set; }

        [Display(Name = "Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Пол")]
        public int? IdSex { get; set; }

        [Display(Name = "Населено място")]
        public int? IdLocation { get; set; }
        public LocationVM? Location { get; set; }

    

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Адрес")]
        public string? Address { get; set; }

        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 10 символа.")]
        [Display(Name = "Пощенски код")]
        public string? PostCode { get; set; }

        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Телефон' не може да съдържа повече от 50 символа.")]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail' не може да съдържа повече от 255 символа.")]
        [Display(Name = "E-mail")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string? Email { get; set; }


        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Титла' не може да съдържа повече от 20 символа.")]
        [Display(Name = "Титла")]
        public string? Title { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Длъжност' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Длъжност")]
        //[Required(ErrorMessage = "Полето \"Длъжност\" е задължително!")]
        public string? Position { get; set; }

        public DateTime? PasswordResetDate { get; set; }
        public DateTime? PasswordResetDateOnly => PasswordResetDate.HasValue ? PasswordResetDate.Value.Date : null;

        public string? IdApplicationUser { get; set; }

        public string TokenExpertType { get; set; }
        public int IdExpert { get; set; }

        public virtual ICollection<CandidateProviderPersonVM> CandidateProviderPersons { get; set; }

        public string FullName
        {
            get { return this.FirstName + " " + this.FamilyName; }
            set { FullName = value; }
        }

        public bool IsSignContract { get; set; }
        public bool IsContractRegisterDocu { get; set; }

        #region Import users
        // иползва се за импорта на потребители
        public string ProviderOwner { get; set; }

        // иползва се за импорта на потребители
        public string LicenseNumber { get; set; }

        // иползва се за импорта на потребители
        public string Bulstat { get; set; }
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
