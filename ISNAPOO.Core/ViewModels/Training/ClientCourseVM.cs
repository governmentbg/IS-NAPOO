using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Training
{
    /// <summary>
    /// Получател на услугата(обучаем) връзка с курс От стара таблица tb_course_groups
    /// </summary>
    [Comment("Получател на услугата(обучаем) връзка с курс")]
    public class ClientCourseVM : IMapFrom<ClientCourse>
    {
        public ClientCourseVM()
        {
            this.ClientCourseDocuments = new HashSet<ClientCourseDocumentVM>();
            this.ClientCourseSubjectGrades = new HashSet<CourseSubjectGradeVM>();
            this.CourseProtocolGrades = new HashSet<CourseProtocolGradeVM>();
            this.ClientCourseStatuses = new HashSet<ClientCourseStatusVM>();
            this.ClientRequiredDocuments = new HashSet<ClientRequiredDocumentVM>();
        }

        public int IdClientCourse { get; set; }

        [Comment("Връзка с  Получател на услугата(обучаем)")]
        public int IdClient { get; set; }

        public virtual ClientVM Client { get; set; }

        [Comment("Връзка с Курс за обучение, предлагани от ЦПО")]
        public int IdCourse { get; set; }

        public virtual CourseVM Course { get; set; }

        [Required(ErrorMessage = "Полето 'Име' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Име' не може да съдържа повече от 100 символа.")]
        [Comment("Име")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Име' може да съдържа само текст на български език!")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Презиме' не може да съдържа повече от 100 символа.")]
        [Comment("Презиме")]
        [RegularExpression(@"\p{IsCyrillic}*\s*-*", ErrorMessage = "Полето 'Презиме' може да съдържа само текст на български език!")]
        public string? SecondName { get; set; }

        [Required(ErrorMessage = "Полето 'Фамилия' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Фамилия' не може да съдържа повече от 100 символа.")]
        [Comment("Фамилия")]
        [RegularExpression(@"\p{IsCyrillic}+\s*-*\p{IsCyrillic}+\s*", ErrorMessage = "Полето 'Фамилия' може да съдържа само текст на български език!")]
        public string FamilyName { get; set; }

        public string FullName => !string.IsNullOrEmpty(this.SecondName) ? $"{this.FirstName} {this.SecondName} {this.FamilyName}" : $"{this.FirstName} {this.FamilyName}";

        [Required(ErrorMessage = "Полето 'Пол' е задължително!")]
        [Comment("Пол")]
        public int? IdSex { get; set; }

        [Required(ErrorMessage = "Полето 'Вид на идентификатора' е задължително!")]
        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [Comment("ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на раждане' е задължително!")]
        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Required(ErrorMessage = "Полето 'Гражданство' е задължително!")]
        [Comment("Гражданство")]
        public int? IdNationality { get; set; }

        [Display(Name = "Професионално направление")]
        public int IdProfessionalDirection { get; set; }

        public virtual ProfessionalDirectionVM ProfessionalDirection { get; set; }

        [Display(Name = "Специалност")]
        public int? IdSpeciality { get; set; }

        public virtual SpecialityVM Speciality { get; set; }

        [Comment("Образование")]
        public int? IdEducation { get; set; }//Таблица 'code_education', висше - бакалавър, висше - магистър, висше - професионален бакалавър, основно, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование
        public virtual KeyValueVM? Education { get; set; }


        [Required(ErrorMessage = "Полето 'Финансиране' е задължително!")]
        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)

        [Comment("Приключване на курс")]
        public int? IdFinishedType { get; set; }//Таблица 'code_cfinished_type' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат

        public virtual KeyValueVM? FinishedType { get; set; }

        public string FinishedTypeName { get; set; }

        [Comment("Дата на приключване на курса")]
        public DateTime? FinishedDate { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на включване' е задължително!")]
        [Comment("Дата на включване в курса")]
        public DateTime? CourseJoinDate { get; set; }

        [Comment("Придобита квалификация")]
        public int? IdQualificationLevel { get; set; }//Таблица 'code_qual_level': Придобита квалификация по професия от същата област на образование, Придобита квалификация по част от същата професия, Придобита първа СПК по професия от същата област на образование, Придобита втора СПК по професия от същата област на образование, Придобита квалификация по част от професия с III СПК, Придобита I СПК, Придобита II СПК, Придобита III СПК, Придобита квалификация по част от професия с II СПК
        
        public virtual KeyValueVM QualificationLevel { get; set; }

        public string CoursePeriod { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        [Required(ErrorMessage = "Полето 'Месторождение (държава)' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Месторождение (държава)' е задължително!")]
        [Comment("Месторождение (държава)")]
        public int? IdCountryOfBirth { get; set; }

        [Comment("Месторождение (населено място)")]
        public int? IdCityOfBirth { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Адрес' може да съдържа до 255 символа!")]
        public string? Address { get; set; }

        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'E-mail адрес' може да съдържа до 100 символа!")]
    
        public string? EmailAddress { get; set; }

        [StringLength(DBStringLength.StringLength20, ErrorMessage = "Полето 'Телефон' може да съдържа до 20 символа!")]
        public string? Phone { get; set; }

        [Comment("Съгласие за използване на информацията за контакт от НАПОО")]
        public bool IsContactAllowed { get; set; }

        [Comment("Лице с увреждания")]
        public bool IsDisabledPerson { get; set; }

        [Comment("Лице в неравностойно положение")]
        public bool IsDisadvantagedPerson { get; set; }

        [Display(Name = "Път до курсисти")]
        public string UploadedFileName { get; set; }

        public List<KeyValuePair<int, List<string>>> ClientDocuments { get; set; }

        public List<KeyValuePair<int, string>> CourseProtocolsWithGrades { get; set; }

        public MemoryStream UploadedFileStream { get; set; }

        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts.Last() : string.Empty);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool HasUploadedFile
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public int male { get; set; } = 0;

        public int female { get; set; } = 0;

        public int bulgarian { get; set; } = 0;

        public int otherNation { get; set; } = 0;

        public int to16 { get; set; } = 0;

        public int between16and29 { get; set; } = 0;
        public int between30and45 { get; set; } = 0;
        public int between46and60 { get; set; } = 0;
        public int between61and80 { get; set; } = 0;
        public int moreThan80 { get; set; } = 0;

        public int disabled { get; set; } = 0;

        // IdCandidateProvider, не е foreign key
        public int? IdCandidateProvider { get; set; }

        public virtual ICollection<ClientCourseDocumentVM> ClientCourseDocuments { get; set; }

        public virtual ICollection<CourseSubjectGradeVM> ClientCourseSubjectGrades { get; set; }

        public virtual ICollection<CourseProtocolGradeVM> CourseProtocolGrades { get; set; }

        public virtual ICollection<ClientCourseStatusVM> ClientCourseStatuses { get; set; }
        public virtual ICollection<ClientRequiredDocumentVM> ClientRequiredDocuments { get; set; }

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


