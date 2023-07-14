using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class StartRegistrationModel
    {
        public StartRegistrationModel()
        {
        }



        public string EAuthStatusName { get; set; }
        public string EAuthStatusFormatted
        {
            get
            {
                string name = "Неуспешно получаване на данни";

                if (EAuthStatusName == "Success") 
                {
                    name = "Успешно получаване на данни";
                }
                else if (EAuthStatusName == "AuthenticationFailed")
                {
                    name = "Неуспешно получаване на данни";
                }

                return name;
            }

        }
        public string EAuthPersonName { get; set; }
        public string EAuthEmail { get; set; }
        public string EAuthEGN { get; set; }

        public Guid guid = Guid.NewGuid();

        public string FileName { get; set; }
        public bool HasUploadedFile { get; set; }

        public MemoryStream file { get; set; }


        [Required(ErrorMessage = "Полето 'ЕИК (БУЛСТАТ)' е задължително")]
        public string EIK { get; set; }

        //[RegularExpression(@"\p{IsCyrillic}+", ErrorMessage = "Моля, въведете текста на български език в полето 'Юридическо лице'")]
        //[RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+", ErrorMessage = "Полето 'Юридическо лице' може да съдържа само текст на български език!")]
        [Required(ErrorMessage = "Полето 'Юридическо лице' е задължително")]
        public string CompanyName { get; set; }

        [Display(Name = "Email адрес")]
        [Required(ErrorMessage = "Полето 'E-mail' е задължително")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string Email { get; set; }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на лицензията' е задължително!")]
        public int IdLicensingType { get; set; }



        //[RegularExpression(@"\p{IsCyrillic}+", ErrorMessage = "Моля, въведете текста на български език в полето 'Представлявано от'")]
        //[RegularExpression(GlobalConstants.RegexExpressionThreeNamesCyrilic, ErrorMessage = "Полето 'Представлявано от' може да съдържа само текст на български език!")]
        [StringLength(100, ErrorMessage = "Максималната позволена дължина е 100 символа за полето 'Представлявано от'")]
        public string ManagerName { get; set; }

        //[RegularExpression(@"\p{IsCyrillic}+", ErrorMessage = "Моля, въведете текста на български език в полето 'Пълномощник'")]
        //[RegularExpression(GlobalConstants.RegexExpressionThreeNamesCyrilic, ErrorMessage = "Полето 'Пълномощник' може да съдържа само текст на български език!")]
        [StringLength(100, ErrorMessage = "Максималната позволена дължина е 100 символа за полето 'Пълномощник'")]
        public string AttorneyName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'ЕГН, ИТН, ИДН' не може да съдържа повече от 100 символа.")]
        [Display(Name = "ЕГН, ИТН, ИДН")]
        public string? Indent { get; set; }



        //[RegularExpression(@"\p{IsCyrillic}+", ErrorMessage = "Моля, въведете текста на български език в полето 'Длъжност'")]
        [Required(ErrorMessage = "Полето 'Длъжност' е задължително")]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Длъжност' не може да съдържа повече от 20 символа.")]
        [Display(Name = "Длъжност")]
        public string? Title { get; set; }
    }
}
