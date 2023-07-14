using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CourseScheduleVM
    {
        public int IdCourseSchedule { get; set; }

        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Тема' е задължително!")]
        [Display(Name = "Връзка с тема от учебна програма към курс")]
        public int IdTrainingCurriculum { get; set; }

        public virtual TrainingCurriculumVM TrainingCurriculum { get; set; }

        [Display(Name = "Връзка с МТБ")]
        public int? IdCandidateProviderPremises { get; set; }

        public virtual CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        [Display(Name = "Връзка с преподавател")]
        public int? IdCandidateProviderTrainer { get; set; }

        public virtual CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        //Нова номенклатура: Вид на обучение - "TrainingScheduleType" (Теория, Практика)
        [Required(ErrorMessage = "Полето 'Вид на провежданото обучение' е задължително!")]
        [Range(1, int.MaxValue, ErrorMessage = "Полето 'Вид на провежданото обучение' е задължително!")]
        [Display(Name = "Вид на провежданото обучение")]
        public int IdTrainingScheduleType { get; set; }

        public string TrainingScheduleType { get; set; }

        [Required(ErrorMessage = "Полето 'Часове' е задължително!")]
        [Range(0.1, double.MaxValue, ErrorMessage = "Полето 'Часове' може да има стойност по-голяма 0!")]
        [Display(Name = "Брой часове на провеждано обучение")]
        public double? Hours { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на провеждано обучение' е задължително!")]
        [Display(Name = "Дата на провеждано обучение")]
        public DateTime? ScheduleDate { get; set; }

        [Display(Name = "Продължителност от")]
        public DateTime? TimeFrom { get; set; }

        [Display(Name = "Продължителност до")]
        public DateTime? TimeTo { get; set; }

        public string Period => this.TimeFrom.HasValue && this.TimeTo.HasValue ? $"{this.TimeFrom.Value.ToString("HH:mm")} - {this.TimeTo.Value.ToString("HH:mm")}" : string.Empty;

        public string ScheduleDateAsStr => $"{this.ScheduleDate.Value.ToString("dd.MM.yyyy")} г.";

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

        public MemoryStream UploadedFileStream { get; set; }

        public string ModifyPersonName { get; set; }

        public string CreatePersonName { get; set; }

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
