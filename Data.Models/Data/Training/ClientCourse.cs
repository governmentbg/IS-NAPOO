using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{


    /// <summary>
    /// Получател на услугата(обучаем) връзка с курс От стара таблица tb_course_groups
    /// </summary>
    [Table("Training_ClientCourse")]
    [Comment("Получател на услугата(обучаем) връзка с курс")]
    public class ClientCourse : IEntity, IModifiable, IDataMigration
    {
        public ClientCourse()
        {

            this.ClientCourseDocuments = new HashSet<ClientCourseDocument>();
            this.ClientRequiredDocuments = new HashSet<ClientRequiredDocument>();
            this.CourseSubjectGrades = new HashSet<CourseSubjectGrade>();
            this.CourseProtocolGrades = new HashSet<CourseProtocolGrade>();
            this.ClientCourseStatuses = new HashSet<ClientCourseStatus>();
            this.ClientRequiredDocuments = new HashSet<ClientRequiredDocument>();
        }

        [Key]
        public int IdClientCourse { get; set; }
        public int IdEntity => IdClientCourse;

        [Comment("Връзка с  Получател на услугата(обучаем)")]
        [ForeignKey(nameof(Client))]
        public int IdClient { get; set; }
        public Client Client { get; set; }

        [Comment("Връзка с Курс за обучение, предлагани от ЦПО")]
        [ForeignKey(nameof(Course))]
        public int IdCourse { get; set; }
        public Course Course { get; set; }


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Име")]
        public string FirstName { get; set; }


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Презиме")]
        public string? SecondName { get; set; }


        [StringLength(DBStringLength.StringLength100)]
        [Comment("Фамилия")]
        public string FamilyName { get; set; }

    
        [Comment("Пол")]
        public int? IdSex { get; set; }


        [Comment("Вид на идентификатора")]//ЕГН/ЛНЧ/ИДН
        public int? IdIndentType { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Comment("ЕГН/ЛНЧ/ИДН")]
        public string? Indent { get; set; }

        [Comment("Дата на включване в курса")]
        public DateTime? CourseJoinDate { get; set; }


        [Comment("Дата на раждане")]
        public DateTime? BirthDate { get; set; }

        [Comment("Гражданство")]
        public int? IdNationality { get; set; }
        
        [Display(Name = "Професионално направление")]
        [ForeignKey(nameof(ProfessionalDirection))]
        public int? IdProfessionalDirection { get; set; }
        public ProfessionalDirection ProfessionalDirection { get; set; }


        
        [Display(Name = "Специалност")]
        [ForeignKey(nameof(Speciality))]
        public int? IdSpeciality { get; set; }
        public Speciality Speciality { get; set; }


        [Comment("Образование")]
        public int? IdEducation { get; set; }//Таблица 'code_education', висше - бакалавър, висше - магистър, висше - професионален бакалавър, основно, придобито право за явяване на държавни зрелостни изпити за завършване на средно образование


        [Comment("Основен източник на финансиране")]
        public int? IdAssignType { get; set; }//Заплащане на такса от обучаемите,По заявка от работодател,ОП Развитие на човешките ресурси(по проекти),ОП Развитие на човешките ресурси(с ваучери),Активни мерки на пазара на труда(от държавния бюд,Други,по заявка на АЗ - ДБТ (не се прилага сред 1.1.2016,други програми - фондове от ЕС (не се прилага сред,други програми - национални фондове (не се прилагапрограма "Аз мога" (не се прилага сред 1.1.2016)



        [Comment("Приключване на курс")]
        public int? IdFinishedType { get; set; }//Таблица 'code_education' завършил с документ, прекъснал по уважителни причини, прекъснал по неуважителни причини, завършил курса, но не положил успешно изпита, придобил СПК по реда на чл.40 от ЗПОО, издаване на дубликат


        [Comment("Дата на приключване на курса")]
        public DateTime? FinishedDate { get; set; }



        [Comment("Придобита квалификация")]
        public int? IdQualificationLevel { get; set; }//Таблица 'code_qual_level': Придобита квалификация по професия от същата област на образование, Придобита квалификация по част от същата професия, Придобита първа СПК по професия от същата област на образование, Придобита втора СПК по професия от същата област на образование, Придобита квалификация по част от професия с III СПК, Придобита I СПК, Придобита II СПК, Придобита III СПК, Придобита квалификация по част от професия с II СПК

        [Comment("Месторождение (държава)")]
        public int? IdCountryOfBirth { get; set; }

        [Comment("Месторождение (населено място)")]
        public int? IdCityOfBirth { get; set; }

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

        public virtual ICollection<ClientCourseDocument> ClientCourseDocuments { get; set; }
        public virtual ICollection<ClientRequiredDocument> ClientRequiredDocuments { get; set; }
        public virtual ICollection<CourseSubjectGrade> CourseSubjectGrades { get; set; }
        public virtual ICollection<CourseProtocolGrade> CourseProtocolGrades { get; set; }
        public virtual ICollection<ClientCourseStatus> ClientCourseStatuses { get; set; }

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


