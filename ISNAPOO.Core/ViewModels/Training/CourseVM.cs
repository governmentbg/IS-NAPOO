using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using ISNAPOO.Core.ViewModels.EKATTE;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Training
{
    /// <summary>
    /// Курс за обучение, предлагани от ЦПО, От стара таблица tb_course_groups
    /// </summary>
    [Table("Training_Course")]
    [Comment("Курс за обучение, предлагани от ЦПО")]
    public class CourseVM : IMapFrom<Course>
    {
        public CourseVM()
        {
            this.ClientCourses = new HashSet<ClientCourseVM>();
            this.CourseCommissionMembers = new HashSet<CourseCommissionMemberVM>();
            this.CourseSubjects = new HashSet<CourseSubjectVM>();
            this.CourseProtocols = new HashSet<CourseProtocolVM>();
            this.ClientRequiredDocuments = new HashSet<ClientRequiredDocumentVM>();
        }

        public int IdCourse { get; set; }

        [Comment("Връзка с Програмa за обучение, предлагани от ЦПО")]
        public int IdProgram { get; set; }

        public virtual ProgramVM Program { get; set; }

        public string ProgramName { get; set; }

        [Comment("Връзка с CandidateProvider")]
        public int? IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Крайна дата за записване")]
        public DateTime? SubscribeDate { get; set; }

        public string SubscribeDateAsStr => this.SubscribeDate.HasValue ? $"{this.SubscribeDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [Required(ErrorMessage = "Полето 'Наименование на курса' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на курса' не може да съдържа повече от 255 символа.")]
        [Comment("Наименование на курса")]
        public string CourseName { get; set; }

        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Полето 'Наименование на курса на латиница' не може да съдържа повече от 255 символа.")]
        [Comment("Наименование на курса на латиница")]
        public string? CourseNameEN { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Други пояснения' не може да съдържа повече от 4000 символа.")]
        [Comment("Други пояснения")]
        public string? AdditionalNotes { get; set; }

        [Comment("Статус на курса")]
        public int? IdStatus { get; set; }//предстоящ, текущ, приключил

        public virtual KeyValueVM Status { get; set; }

        public string StatusName { get; set; }

        [Required(ErrorMessage = "Полето 'Вид' е задължително!")]
        [Comment("Вид")]
        public int? IdMeasureType { get; set; }//по програми и мерки за заетост, по реда на Глава 7 от ЗНЗ

        [Comment("Вид на курса за обучение")]
        public int? IdTrainingCourseType { get; set; } // номенклатура TypeFrameworkProgram

        public string TrainingCourseTypeName { get; set; }

        public virtual KeyValueVM MeasureType { get; set; }

        [Required(ErrorMessage = "Полето 'Основен източник на финансиране' е задължително!")]
        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)

        public virtual KeyValueVM AssignType { get; set; }

        public string AssignTypeName { get; set; }

        [Required(ErrorMessage = "Полето 'Форма на обучение' е задължително!")]
        [Comment("Форма на обучение")]
        public int? IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална

        public virtual KeyValueVM FormEducation { get; set; }

        public string FormEducationName { get; set; }

        [Comment("Населено място")]
        public int? IdLocation { get; set; }

        public virtual LocationVM Location { get; set; }

        [Comment("Задължителни учебни ч.(бр.)")]
        public int? MandatoryHours { get; set; } = 0;

        [Comment("Избираеми учебни ч.(бр.)")]
        public int? SelectableHours { get; set; } = 0;

        [Comment("Продължителност")]
        public int? DurationHours { get; set; } = 0;

        [Required(ErrorMessage = "Полето 'Цена (в лева за един курсист)' е задължително!")]
        [Range(0, double.MaxValue, ErrorMessage = "Полето 'Цена (в лева за един курсист)' може да има само положителна стойност!")]
        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Цена (в лева за един курсист)")]
        public decimal? Cost { get; set; }

        [Required(ErrorMessage = "Полето 'Очаквана дата за започване на курса' е задължително!")]
        [Comment("Очаквана дата за започване на курса")]
        public DateTime? StartDate { get; set; }

        public string StartDateAsStr => this.StartDate.HasValue ? $"{this.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [Required(ErrorMessage = "Полето 'Очаквана дата за завършване на курса' е задължително!")]
        [Comment("Очаквана дата за завършване на курса")]
        public DateTime? EndDate { get; set; }

        public string EndDateAsStr => this.EndDate.HasValue ? $"{this.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [Comment("Очаквана дата за изпит по теория")]
        public DateTime? ExamTheoryDate { get; set; }

        public string timeSpan
        {
            get
            {
                return $"{StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} - {EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г.";
            }
        }

        [Comment("Очаквана дата за изпит по практика")]
        public DateTime? ExamPracticeDate { get; set; }

        [Required(ErrorMessage = "Полето 'Основна учебна база' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Основна учебна база' е задължително!")]
        [Comment("Метериална техническа база")]
        public int? IdCandidateProviderPremises { get; set; }

        public virtual CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Comment("Брой обучаеми в неравностойно положение (параграф 1 – т. 4а от ЗНЗ)")]
        public int? DisabilityCount { get; set; }

        [Comment("Дали курсът е архивиран")]
        public bool IsArchived { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }

        public string ArchiveCourseValue { get; set; }

        public string CourseNameAndPeriod => $"{this.CourseName} {this.Period}";

        public virtual ICollection<ClientCourseVM> ClientCourses { get; set; }

        public virtual ICollection<CourseCommissionMemberVM> CourseCommissionMembers { get; set; }

        public virtual ICollection<CourseSubjectVM> CourseSubjects { get; set; }

        public virtual ICollection<CourseProtocolVM> CourseProtocols { get; set; }

        public virtual ICollection<ClientRequiredDocumentVM> ClientRequiredDocuments { get; set; }

        public DateTime? startCourseFrom { get; set; }

        public DateTime? startCourseTo { get; set; }

        public DateTime? endCourseFrom { get; set; }

        public DateTime? endtCourseTo { get; set; }

        public DateTime? subscribeDateFrom { get; set; }

        public DateTime? subscribeDateTo { get; set; }
        public DateTime? examTheoryDateFrom { get; set; }

        public DateTime? examTheoryDateTo { get; set; }
        public DateTime? examPracticeDateFrom { get; set; }

        public DateTime? examPracticeDateTo { get; set; }

        public string? clientCourseFirstName { get; set; }
        public string? clientCourseLastName { get; set; }

        public string? clientCourseIndent { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }
        public DateTime? combined_date { get; set; }
        public string combined_date_string => this.combined_date.Value.ToString(GlobalConstants.DATE_FORMAT);
        public string CandidateProviderName => this.CandidateProvider.CPONameOwnerGrid;
        public string CandidateProviderLicenseNumber => this.CandidateProvider.LicenceNumber;
        public string CourseTypeByDate => this.combined_date.Value == this.ExamTheoryDate.Value ? "Теория" : "Практика" ;
        public string CourseNameWithStartAndEndDate => $"{this.CourseName}/ {this.StartDate.Value.ToString("dd.MM.yyyy")} - {this.EndDate.Value.ToString("dd.MM.yyyy")}";
        public string Period => $"{this.StartDate.Value.ToString("dd.MM.yyyy")} г. - {this.EndDate.Value.ToString("dd.MM.yyyy")} г.";

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до учебна програма' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Път до учебна програма")]
        public string UploadedFileName { get; set; }

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

        public int? RIDPKCountSubmitted { get; set; }

        public int? RIDPKCountReturned { get; set; }

        public int? RIDPKCountEnteredInRegister { get; set; }

        public int? RIDPKCountDeclined { get; set; }

        public int? RIDPKCountNotSubmitted { get; set; }

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

