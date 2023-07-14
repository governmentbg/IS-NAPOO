using Data.Models.Data.Framework;
using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Identity
{
    public class ApplicationUserVM : IMapTo<ApplicationUser>, IMapFrom<ApplicationUser>, IModifiable
    {
        public string Id { get; set; }
        public int IdPerson { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Име' не може да съдържа повече от 255 символа!")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string Email { get; set; }

        public string UserName { get; set; }
        public string EIK { get; set; }

        [Required(ErrorMessage = "Полето \'Име\' е задължително!")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Име' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа!")]
        public string FirstName { get; set; }
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Презиме' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Презиме")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Фамилия' може да съдържа само текст на български език!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Display(Name = "Фамилия")]
        public string FamilyName { get; set; }
        public string Password { get; set; }
        public int IdCandidateProvider { get; set; }
        public PersonVM Person { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Име' не може да съдържа повече от 20 символа!")]
        public string? Phone { get; set; }
        public List<ApplicationRoleVM> Roles { get; set; } = new List<ApplicationRoleVM>();
        public string RolesInfo { get; set; }

        public int IdUser { get; set; }

        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'Име' не може да съдържа повече от 10 символа!")]
        public string IndentTypeName { get; set; }

        [Display(Name = "Статус на потребителя")]
        public int IdUserStatus { get; set; }
        public string UserStatusName { get; set; }

        public bool IsCommissionExpert { get; set; }
        public bool IsExternalExpert { get; set; }
        public bool IsNapooExpert { get; set; }
        public bool IsDOCExpert { get; set; }

        public bool IsFirstReistration { get; set; } = false;
        
        public string FullName
        {
            get { return this.FirstName + " " + this.FamilyName; }
        }

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }
        public string CreateUserName { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }
        public string ModifyUserName { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }
        #endregion


    }
}
