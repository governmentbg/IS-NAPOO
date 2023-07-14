using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Assessment
{
    public class SurveyVM
    {
        public SurveyVM()
        {
            this.Questions = new HashSet<QuestionVM>();
            this.SurveyResults = new HashSet<SurveyResultVM>();
        }

        public int IdSurvey { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на анкетата' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Наименование на анкетата' може да съдържа до 100 символа!")]
        [Comment("Име на анкета")]
        public string Name { get; set; }

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Описание' може да съдържа до 4000 символа!")]
        [Comment("Допълнителен текст")]
        public string? AdditionalText { get; set; }

        [Required(ErrorMessage = "Полето 'Тип на анкетата' е задължително!")]
        [Comment("Вид анкета")]
        public int? IdSurveyТype { get; set; } // KeyTypeIntCode: "SurveyType" - Проследяване реализацията на завършилите ПО в ЦПО/Измерване на степента на удовлетвореност на обучените

        public string SurveyTypeValue { get; set; }

        [Comment("Тип анкета")]
        public int? IdSurveyTarget { get; set; } // KeyTypeIntCode: "SurveyTarget" - за студенти, за ЦПО, за работодатели

        [Required(ErrorMessage = "Полето 'Период на обучение от' е задължително!")]
        [Comment("Период на обучение от")]
        public DateTime? TrainingPeriodFrom { get; set; }

        public string TrainingPeriodFromAsStr => this.TrainingPeriodFrom.HasValue ? $"{this.TrainingPeriodFrom.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [Required(ErrorMessage = "Полето 'Период на обучение до' е задължително!")]
        [Comment("Период на обучение до")]
        public DateTime? TrainingPeriodTo { get; set; }

        public string TrainingPeriodToAsStr => this.TrainingPeriodTo.HasValue ? $"{this.TrainingPeriodTo.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        //[Required(ErrorMessage = "Полето 'Вид на курса за обучение' е задължително!")]
        [Comment("Вид на курса за обучение")]
        public int? IdTrainingCourseType { get; set; } // KeyTypeIntCode: "TypeFrameworkProgram"

        public string TrainingCourseTypeValue { get; set; }

        [Required(ErrorMessage = "Полето 'Вътрешен код на анкетата' е задължително!")]
        [StringLength(DBStringLength.StringLength100, ErrorMessage = "Полето 'Вътрешен код на анкетата' може да съдържа до 100 символа!")]
        [Comment("Вътрешен код на анкетата")]
        public string? InternalCode { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на активност от' е задължително!")]
        [Comment("Дата на активност от")]
        public DateTime? StartDate { get; set; }

        public string StartDateAsStr => this.StartDate.HasValue ? $"{this.StartDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        [Required(ErrorMessage = "Полето 'Дата на активност до' е задължително!")]
        [Comment("Дата на активност до")]
        public DateTime? EndDate { get; set; }

        public string SurveyExpirationValue => this.StartDate.HasValue && this.EndDate.HasValue
            ? DateTime.Now.Date >= this.StartDate.Value && DateTime.Now.Date <= this.EndDate.Value
                ? "Активна"
                : "Неактивна"
            : string.Empty;

        [Comment("Статус на анкетата")]
        public int IdSurveyStatus { get; set; } // Номенклатура - KeyTypeIntCode: "SurveyStatusType"

        public string EndDateAsStr => this.EndDate.HasValue ? $"{this.EndDate.Value.ToString(GlobalConstants.DATE_FORMAT)} г." : string.Empty;

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        public string EmailTemplateHeader { get; set; }

        public string EmailTemplateText { get; set; }

        public int FiledOutCount { get; set; }

        public int SurveysSentCount { get; set; }

        public MemoryStream UploadedFileStream { get; set; }

        public string UploadedFileName { get; set; }

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

        public virtual ICollection<QuestionVM> Questions { get; set; }

        public virtual ICollection<SurveyResultVM> SurveyResults { get; set; }


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
