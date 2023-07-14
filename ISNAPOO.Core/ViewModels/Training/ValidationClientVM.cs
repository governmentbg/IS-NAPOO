using Data.Models.Data.Candidate;
using Data.Models.Data.SPPOO;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.SPPOO;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Request;
using Data.Models.Common;
using Data.Models.Data.Common;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationClientVM
    {
        public ValidationClientVM()
        {
            this.ValidationCommissionMembers = new HashSet<ValidationCommissionMemberVM>();
            this.ValidationProtocols = new HashSet<ValidationProtocolVM>();
            this.ValidationClientDocuments = new HashSet<ValidationClientDocumentVM>();
            this.ValidationCompetencies = new HashSet<ValidationCompetencyVM>();
            this.ValidationClientRequiredDocuments = new HashSet<ValidationClientRequiredDocumentVM>();
            this.ValidationProtocolGrades = new HashSet<ValidationProtocolGradeVM>();
            this.ValidationCurriculums = new HashSet<ValidationCurriculum>();
            this.ValidationPremises = new HashSet<ValidationPremises>();
        }

        [Key]
        public int IdValidationClient { get; set; }

        [Comment("Връзка с  Получател на услугата(обучаем)")]
        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; }

        public ClientVM Client { get; set; }

        [Comment("Връзка с CandidateProvider")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }

        public CandidateProviderVM CandidateProvider { get; set; }

        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int? IdSpeciality { get; set; }

        public SpecialityVM Speciality { get; set; }

        [Comment("Рамкова програма")]
        [ForeignKey(nameof(FrameworkProgram))]
        public int? IdFrameworkProgram { get; set; }

        public FrameworkProgramVM FrameworkProgram { get; set; }

        [Comment("Придобита квалификация")]
        public int? IdQualificationLevel { get; set; }//Таблица 'code_qual_level': Придобита квалификация по професия от същата област на образование, Придобита квалификация по част от същата професия, Придобита първа СПК по професия от същата област на образование, Придобита втора СПК по професия от същата област на образование, Придобита квалификация по част от професия с III СПК, Придобита I СПК, Придобита II СПК, Придобита III СПК, Придобита квалификация по част от професия с II СПК

        // Номенклатура "TypeFrameworkProgram"
        [Comment("Вид на курса за обучение")]
        public int IdCourseType { get; set; }
        public KeyValueVM? CourseType { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Име")]
        public string FirstName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Презиме")]
        public string? SecondName { get; set; }

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Фамилия")]
        public string FamilyName { get; set; }

        public string FullName => !string.IsNullOrEmpty(this.SecondName) ? $"{this.FirstName} {this.SecondName} {this.FamilyName}" : $"{this.FirstName} {this.FamilyName}";

        public string timeSpan
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                    return $"{StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} - {EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
                else
                    return "";
            }
        }

        public string WholeName => FirstName + " " + SecondName + " " + FamilyName;

        [Comment("Пол")]
        public int? IdSex { get; set; }

        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength10)]
        [Comment("ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Comment("Гражданство")]
        public int? IdNationality { get; set; }

        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)

        [Comment("Приключване на курс")]
        public int? IdFinishedType { get; set; }//Таблица 'code_education' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат
        public KeyValueVM FinishType { get; set; }
        [Comment("Месторождение (държава)")]
        public int? IdCountryOfBirth { get; set; }

        [Comment("Месторождение (населено място)")]
        public int? IdCityOfBirth { get; set; }

        public virtual Location CityOfBirth { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Цена (в лева за консултирано лице)")]
        public decimal? Cost { get; set; }

        [Comment("Дата на стартиране на консултацията")]
        public DateTime? StartDate { get; set; }

        [Comment("Дата на приключване на консултацията")]
        public DateTime? EndDate { get; set; }

        [Comment("Очаквана дата за изпит по теория")]
        public DateTime? ExamTheoryDate { get; set; }

        [Comment("Очаквана дата за изпит по практика")]
        public DateTime? ExamPracticeDate { get; set; }

        [Comment("Адрес")]
        [StringLength(DBStringLength.StringLength255)]
        public string? Address { get; set; }

        [Comment("E-mail адрес")]
        [StringLength(DBStringLength.StringLength100)]
        public string? EmailAddress { get; set; }

        [Comment("Телефон")]
        [StringLength(DBStringLength.StringLength20)]
        public string? Phone { get; set; }
        [Comment("Съгласие за използване на информацията за контакт от НАПОО")]
        public bool IsContactAllowed { get; set; }

        [Comment("Лице с увреждания")]
        public bool IsDisabledPerson { get; set; }

        [Comment("Лице в неравностойно положение")]
        public bool IsDisadvantagedPerson { get; set; }

        [Comment("Статус на валидирането")]
        public int? IdStatus { get; set; } // Номенклатура - KeyTypeIntCode: "CourseStatus"

        [Comment("Дали валидирането е архивирано")]
        public bool IsArchived { get; set; }
        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }
        [Display(Name = "ID на работен документ в деловодната система на НАПОО")]
        public int? DS_ID { get; set; }

        [Display(Name = "Дата на работен документ в деловодната система на НАПОО")]
        public DateTime? DS_DATE { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на работен документ в деловодната система на НАПОО")]
        public string? DS_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на на работен документ в деловодната система на НАПОО")]
        public string? DS_DocNumber { get; set; }


        [Display(Name = "Официален номер на документ в деловодната система на НАПОО")]

        public int? DS_OFFICIAL_ID { get; set; }

        [Display(Name = "Дата на официален документ в деловодната система на НАПОО")]
        public DateTime? DS_OFFICIAL_DATE { get; set; }


        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_GUID { get; set; }


        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_DocNumber { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Преписка в деловодната система на НАПОО")]
        public string? DS_PREP { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Връзка към документа  в деловодната система на НАПОО")]
        public string? DS_LINK { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл с учебна програма")]
        public string? UploadedCurriculumFileName { get; set; }

        public string Period => this.StartDate.HasValue && this.EndDate.HasValue ? $"{this.StartDate.Value.ToString("dd.MM.yyyy")} г. - {this.EndDate.Value.ToString("dd.MM.yyyy")} г." : string.Empty;

        public virtual ICollection<ValidationCommissionMemberVM> ValidationCommissionMembers { get; set; }

        public virtual ICollection<ValidationProtocolVM> ValidationProtocols { get; set; }

        public virtual ICollection<ValidationClientDocumentVM> ValidationClientDocuments { get; set; }
        public virtual ICollection<ValidationCompetencyVM> ValidationCompetencies { get; set; }

        public virtual ICollection<ValidationClientRequiredDocumentVM> ValidationClientRequiredDocuments { get; set; }

        public virtual ICollection<ValidationProtocolGradeVM> ValidationProtocolGrades { get; set; }
        public virtual ICollection<ValidationCurriculum> ValidationCurriculums { get; set; }
        public virtual ICollection<ValidationPremises> ValidationPremises { get; set; }

        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts[arrNameParts.Length - 1] : string.Empty);
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
