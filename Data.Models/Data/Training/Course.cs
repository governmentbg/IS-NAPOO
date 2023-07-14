 


using Data.Models.Common;
using Data.Models.Data.Candidate;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Курс за обучение, предлагани от ЦПО, От стара таблица tb_course_groups
    /// </summary>
    [Table("Training_Course")]
    [Comment("Курс за обучение, предлагани от ЦПО")]
    public class Course : AbstractUploadFile, IEntity, IModifiable, IDataMigration
    {
        public Course()
        {
            this.ClientCourses = new HashSet<ClientCourse>();
            this.CourseCommissionMembers = new HashSet<CourseCommissionMember>();
            this.CourseSubjects = new HashSet<CourseSubject>();
            this.CourseProtocols = new HashSet<CourseProtocol>();
            this.ClientRequiredDocuments = new HashSet<ClientRequiredDocument>();
            this.CourseOrders = new HashSet<CourseOrder>();
        }

        [Key]
        public int IdCourse { get; set; }
        public int IdEntity => IdCourse;

        [Comment("Връзка с Програмa за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Program))]
        public int? IdProgram { get; set; }
        public Program Program { get; set; }

        [Comment("Връзка с CandidateProvider")]
        [ForeignKey(nameof(CandidateProvider))]
        public int? IdCandidateProvider { get; set; }
        public CandidateProvider CandidateProvider { get; set; }



        [Comment("Крайна дата за записване")]        
        public DateTime? SubscribeDate { get; set; }


        [StringLength(DBStringLength.StringLength255)]
        [Comment("Наименование на курса")]
        public string CourseName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Наименование на курса на латиница' не може да съдържа повече от 255 символа.")]
        [Comment("Наименование на курса на латиница")]
        public string? CourseNameEN { get; set; }

        [Column(TypeName = "varchar(MAX)")]
        [Comment("Други пояснения")]
        public string? AdditionalNotes { get; set; }

        [Comment("Статус на курса")]
        public int? IdStatus { get; set; }//предстоящ, текущ, приключил


        [Comment("Вид")]
        public int? IdMeasureType { get; set; }//по програми и мерки за заетост, по реда на Глава 7 от ЗНЗ

        [Comment("Вид на курса за обучение")]
        public int? IdTrainingCourseType { get; set; } // номенклатура TypeFrameworkProgram

        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)

        [Comment("Форма на обучение")]
        public int? IdFormEducation { get; set; }//Дневна, Вечерна, Дистанционна, Индивидуална, самостоятелна, Задочна, Дуална

        [Comment("Населено място")]
        [ForeignKey(nameof(Location))]
        public int? IdLocation { get; set; }
        public Location Location { get; set; }

        
        [Comment("Задължителни учебни ч.(бр.)")]
        public int? MandatoryHours { get; set; } = 0;

        
        [Comment("Избираеми учебни ч.(бр.)")]
        public int? SelectableHours { get; set; } = 0;

        
        [Comment("Продължителност")]
        public int? DurationHours { get; set; } = 0;


        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Цена (в лева за един курсист)")]
        public decimal? Cost { get; set; }


        [Comment("Очаквана дата за започване на курса")]
        public DateTime? StartDate { get; set; }


        [Comment("Очаквана дата за завършване на курса")]
        public DateTime? EndDate { get; set; }


        [Comment("Очаквана дата за изпит по теория")]
        public DateTime? ExamTheoryDate { get; set; }


        [Comment("Очаквана дата за изпит по практика")]
        public DateTime? ExamPracticeDate { get; set; }

        [Comment("Метериална техническа база")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int? IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл с учебна програма")]
        public override string? UploadedFileName { get; set; }

        public virtual ICollection<ClientCourse> ClientCourses { get; set; }

        public virtual ICollection<CourseCommissionMember> CourseCommissionMembers { get; set; }

        public virtual ICollection<CourseSubject> CourseSubjects { get; set; }

        public virtual ICollection<CourseProtocol> CourseProtocols { get; set; }

        public virtual ICollection<ClientRequiredDocument> ClientRequiredDocuments { get; set; }

        public virtual ICollection<CourseOrder> CourseOrders { get; set; }


        [Comment("Брой обучаеми в неравностойно положение (параграф 1 – т. 4а от ЗНЗ)")]
        public int? DisabilityCount { get; set; }

        [Comment("Дали курсът е архивиран")]
        public bool IsArchived { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }

        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion

        #region IModifiable
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public override int IdModifyUser { get; set; }

        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion
    }
}

