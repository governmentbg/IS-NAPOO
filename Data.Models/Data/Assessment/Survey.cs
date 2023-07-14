using Data.Models.Data.Control;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Assessment
{
    /// <summary>
    /// Анкета
    /// </summary>
    [Table("Assess_Survey")]
    [Display(Name = "Анкета")]
    public class Survey : IEntity, IModifiable
    {
        public Survey()
        {
            this.Questions = new List<Question>();
            this.SurveyResults = new List<SurveyResult>();
        }
        [Key]
        public int IdSurvey { get; set; }
        public int IdEntity => IdSurvey;

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Име на анкета")]
        public string Name { get; set; }

        [StringLength(DBStringLength.StringLength4000)]
        [Comment("Допълнителен текст")]
        public string? AdditionalText { get; set; }

        [Comment("Вид анкета")]
        public int? IdSurveyТype { get; set; } // KeyTypeIntCode: "SurveyType" - Проследяване реализацията на завършилите ПО в ЦПО/Измерване на степента на удовлетвореност на обучените

        [Comment("Тип анкета")]
        public int? IdSurveyTarget { get; set; } // KeyTypeIntCode: "SurveyTarget" - за студенти, за ЦПО, за работодатели

        [Comment("Период на обучение от")]
        public DateTime? TrainingPeriodFrom { get; set; }

        [Comment("Период на обучение до")]
        public DateTime? TrainingPeriodTo { get; set; }

        [Comment("Вид на курса за обучение")]
        public int? IdTrainingCourseType { get; set; } // KeyTypeIntCode: "TypeFrameworkProgram"

        [StringLength(DBStringLength.StringLength100)]
        [Comment("Вътрешен код на анкетата")]
        public string? InternalCode { get; set; }

        [Comment("Дата на активност от")]
        public DateTime? StartDate { get; set; }

        [Comment("Дата на активност до")]
        public DateTime? EndDate { get; set; }

        [Comment("Статус на анкетата")]
        public int IdSurveyStatus { get; set; } // Номенклатура - KeyTypeIntCode: "SurveyStatusType"

        public virtual ICollection<Question> Questions { get; set; }

        public virtual ICollection<SurveyResult> SurveyResults { get; set; }


        #region САМООЦЕНЯВАНЕ

        [Comment("Година")]
        public int? Year { get; set; }

        [Comment("Отлично")]
        public int? Excellent { get; set; }

        [Comment("Добро")]
        public int? Good { get; set; }

        [Comment("Задоволително")]
        public int? Satisfactory { get; set; }

        #endregion

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
    }
}
