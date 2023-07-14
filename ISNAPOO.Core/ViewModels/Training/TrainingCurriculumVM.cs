using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.DOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class TrainingCurriculumVM
    {
        public TrainingCurriculumVM()
        {
            this.TrainingCurriculumERUs = new HashSet<TrainingCurriculumERUVM>();
            this.CourseSchedules = new HashSet<CourseScheduleVM>();
            this.SelectedERUs = new List<ERUVM>();
        }

        public int IdTrainingCurriculum { get; set; }

        [Display(Name = "Връзка със учебен план")]
        public int? IdCandidateCurriculum { get; set; }

        public virtual CandidateCurriculumVM CandidateCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка със Специалност")]
        public int IdCandidateProviderSpeciality { get; set; }

        public virtual CandidateProviderSpecialityVM CandidateProviderSpeciality { get; set; }

        [Required]
        [Display(Name = "Връзка с Програмa за обучение")]
        public int IdProgram { get; set; }

        public virtual ProgramVM Program { get; set; }

        [Display(Name = "Връзка с Курс за обучение")]
        public int? IdCourse { get; set; }

        public virtual CourseVM Course { get; set; }

        [Required(ErrorMessage = "Полето 'Вид професионална подготовка' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид професионална подготовка' е задължително!")]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }

        public string ProfessionalTraining { get; set; }

        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 1000 символа.")]
        [Display(Name = "Предмет")]
        [Required(ErrorMessage = "Полето 'Предмет' е задължително!")]
        public string Subject { get; set; }// Предмет

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Тема' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Тема")]
        [Required(ErrorMessage = "Полето 'Тема' е задължително!")]
        public string Topic { get; set; }// Тема

        [Display(Name = "Tеория")]
        public double? Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        public double? Practice { get; set; }//Практика

        public double Hours { get; set; }

        public List<ERUVM> SelectedERUs { get; set; }

        public string ERUsJoined => this.SelectedERUs.Any() ? string.Join("", this.SelectedERUs.Select(x => x.Code)) : string.Empty;

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до учебна програма' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Път до учебна програма")]
        public string UploadedFileName { get; set; }

        [Display(Name = "Подредба")]
        public int? Order { get; set; }//OldIS => int_curric_order
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

        public virtual ICollection<TrainingCurriculumERUVM> TrainingCurriculumERUs { get; set; }

        public virtual ICollection<CourseScheduleVM> CourseSchedules { get; set; }



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
