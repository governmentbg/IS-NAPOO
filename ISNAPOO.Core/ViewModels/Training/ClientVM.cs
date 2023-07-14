using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    /// <summary>
    /// Получател на услугата(обучаем) От стара таблица tb_clients
    /// </summary>
    [Comment("Получател на услугата(обучаем)")]
    public class ClientVM : IMapFrom<Client>
    {
        public int IdClient { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Comment("Име")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Име на Латиница")]
        public string FirstNameEN { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Comment("Презиме")]
        public string? SecondName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Презиме на Латиница")]
        public string? SecondNameEN { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Comment("Фамилия")]
        public string FamilyName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Фамилия на Латиница")]
        public string FamilyNameEN { get; set; }

        public string? FullName => !string.IsNullOrEmpty(this.SecondName) ? $"{this.FirstName} {this.SecondName} {this.FamilyName}" : $"{this.FirstName} {this.FamilyName}";

        public string FullNameEN => !string.IsNullOrEmpty(this.SecondNameEN) ? $"{this.FirstNameEN} {this.SecondNameEN} {this.FamilyNameEN}" : $"{this.FirstNameEN} {this.FamilyNameEN}";

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Пол' е задължително!")]
        [Comment("Пол")]
        public int? IdSex { get; set; }

        public virtual KeyValueVM Sex { get; set; }

        [Required(ErrorMessage = "Полето 'Вид на идентификатора' е задължително!")]
        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength10, ErrorMessage = "Полето 'ЕГН/ЛНЧ/ИДН' не може да съдържа повече от 10 символа.")]
        [Comment( "ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на раждане' е задължително!")]
        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Полето 'Гражданство' е задължително!")]
        [Comment("Гражданство")]
        public int? IdNationality { get; set; }

        public virtual KeyValueVM Nationality { get; set; }

        [Display(Name = "Професионално направление")]
        public int? IdProfessionalDirection { get; set; }

        public virtual ProfessionalDirectionVM ProfessionalDirection { get; set; }

        [Comment("Образование")]
        public int? IdEducation { get; set; }//Таблица 'code_education', висше - бакалавър, висше - магистър, висше - професионален бакалавър, основно, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование

        [Required(ErrorMessage = "Полето 'Месторождение (държава)' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Месторождение (държава)' е задължително!")]
        [Comment("Месторождение (държава)")]
        public int? IdCountryOfBirth { get; set; }

        [Comment("Месторождение (населено място)")]
        public int? IdCityOfBirth { get; set; }

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


