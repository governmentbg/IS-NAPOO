namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    using global::Data.Models.Data.ProviderData;
    using ISNAPOO.Common.Constants;
    using ISNAPOO.Core.Mapping;
    using ISNAPOO.Core.ViewModels.EKATTE;
    using Microsoft.EntityFrameworkCore;
    using System.ComponentModel.DataAnnotations;

    public class ProviderVM : IMapFrom<Provider>, IMapTo<Provider>
    {
        public int Id { get; set; }

        /// <summary>
        /// "активна"
        ///"окончателно отнета"
        ///"временно отнета за срок от 6 месеца"
        ///"заличена"
        ///"временно отнета за срок от 3 месеца"
        ///" прекратена"
        /// </summary>
        [Display(Name = "Статус на лиценза")]
        public int LicenceStatusId { get; set; }

        public int LicenceNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на юридическото лице' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на юридическото лице' не може да съдържа повече от 255 символа.")]
        [Display(Name = "Наименование на юридическото лице")]
        public string ProviderOwner { get; set; }

        [Required(ErrorMessage = "Полето 'ЕИК/Булстат' е задължително!")]
        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'ЕИК/Булстат' не може да съдържа повече от 20 символа.")]
        [Display(Name = "ЕИК/Булстат")]
        public string PoviderBulstat { get; set; }

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

        public string? Country { get; set; }

        [Display(Name = "Населено място")]
        public int? IdLocation { get; set; }

        public LocationVM Location { get; set; }

        [Display(Name = "Адрес по регистрация")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес по регистрация' не може да съдържа повече от 255 символа.")]
        public string ProviderAddress { get; set; }

        [Display(Name = "Пощенски код")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код' не може да съдържа повече от 4 символа.")]
        public string ZipCode { get; set; }

        [Display(Name = "Телефон")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Телефон' не може да съдържа повече от 255 символа.")]
        public string ProviderPhone { get; set; } = string.Empty;

        #region Данни за контакт с ЦПО
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Име на ЦПО,ЦИПО' не може да съдържа повече от 512 символа.")]
        [Comment("Име на ЦПО,ЦИПО")]
        public string ProviderName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Лице за контакт/кореспондениця' не може да съдържа повече от 100 символа.")]
        [Comment("Лице за контакт/кореспондениця")]
        public string? PersonNameCorrespondence { get; set; }

        [Comment("Населено място за кореспондениця на ЦПО,ЦИПО")]
        public int? IdLocationCorrespondence { get; set; }

        public LocationVM LocationCorrespondence { get; set; }

        [Comment("Адрес за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес за кореспонденция   на ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderAddressCorrespondence { get; set; }

        [Comment("Пощенски код за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength4, ErrorMessage = "Полето 'Пощенски код за кореспонденция   на ЦПО,ЦИПО' не може да съдържа повече от 4 символа.")]
        public string? ZipCodeCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Факс за кореспонденция с ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        [Comment("Факс за кореспонденция с ЦПО,ЦИПО")]
        public string? ProviderFaxCorrespondence { get; set; }

        [Comment("E-mail за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'E-mail за кореспонденция с ЦПО,ЦИПО' не може да съдържа повече от 255 символа.")]
        public string? ProviderEmailCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'ProviderPhoneCorrespondence' не може да съдържа повече от 255 символа.")]
        public string ProviderPhoneCorrespondence { get; set; } = string.Empty;
        #endregion

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'ProviderFax' не може да съдържа повече от 255 символа.")]
        public string ProviderFax { get; set; } = string.Empty;

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'ProviderWeb' не може да съдържа повече от 255 символа.")]
        public string ProviderWeb { get; set; } = string.Empty;

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'ProviderEmail' не може да съдържа повече от 255 символа.")]
        [EmailAddress(ErrorMessage = "Невалиден E-mail адрес!")]
        public string ProviderEmail { get; set; } = string.Empty;

        public string CPONameAndOwnerName => $"{this.ProviderName} {this.ProviderOwner}";

        public string CPONameOwnerAndBulstat => $"{this.ProviderName} {this.ProviderOwner} {this.PoviderBulstat}";
    }
}
