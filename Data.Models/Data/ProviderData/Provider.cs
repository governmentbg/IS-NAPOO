using Data.Models.Data.Common;
using Data.Models.Data.Request;
using Data.Models.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ProviderData
{

    /// <summary>
    /// Данни за ЦПО,ЦИПО
    /// </summary>
    [Table("Provider")]
    [Display(Name = "Данни за ЦПО,ЦИПО")]
    public class Provider
    {
        public Provider()
        {
        }

        
        [Key]
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

        [Required]
        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Наименование на юридическото лице")]
        public string ProviderOwner { get; set; }

        [Required]
        [StringLength(DBStringLength.StringLength20)]
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
        [ForeignKey(nameof(Location))]
        public int? IdLocation { get; set; }
        public Location Location { get; set; }


        [Display(Name = "Адрес по регистрация")]
        [StringLength(DBStringLength.StringLength255)]
        public string ProviderAddress { get; set; }

        [Display(Name = "Пощенски код")]
        [StringLength(DBStringLength.StringLength4)]
        public string ZipCode { get; set; }

        [Display(Name = "Телефон")]
        [StringLength(DBStringLength.StringLength255)]
        public string ProviderPhone { get; set; } = string.Empty;



        #region Данни за контакт с ЦПО


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Лице за контакт/кореспондениця")]
        public string? PersonNameCorrespondence { get; set; }

        [Comment("Населено място за кореспондениця на ЦПО,ЦИПО")]
        [ForeignKey(nameof(LocationCorrespondence))]
        public int? IdLocationCorrespondence { get; set; }
        public Location LocationCorrespondence { get; set; }

        [Comment("Адрес за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255)]
        public string? ProviderAddressCorrespondence { get; set; }

        [Comment("Пощенски код за кореспонденция   на ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength4)]
        public string? ZipCodeCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Факс за кореспонденция с ЦПО,ЦИПО")]
        public string? ProviderFaxCorrespondence { get; set; }

        [Comment("E-mail за кореспонденция с ЦПО,ЦИПО")]
        [StringLength(DBStringLength.StringLength255)]
        public string? ProviderEmailCorrespondence { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        public string ProviderPhoneCorrespondence { get; set; } = string.Empty;
        #endregion


        [StringLength(DBStringLength.StringLength512)]
        [Comment("Име на ЦПО,ЦИПО")]
        public string ProviderName { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        public string ProviderFax { get; set; } = string.Empty;

        [StringLength(DBStringLength.StringLength255)]
        public string ProviderWeb { get; set; } = string.Empty;

        [StringLength(DBStringLength.StringLength255)]
        public string ProviderEmail { get; set; } = string.Empty;



    }
}
