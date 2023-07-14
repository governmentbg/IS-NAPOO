using Data.Models.Data.Candidate;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ISNAPOO.Core.ViewModels.Candidate;
using ISNAPOO.Core.ViewModels.DOC;
using ISNAPOO.Core.ViewModels.Common.ValidationModels;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationCurriculumVM : IMapFrom<ValidationCurriculum>
    {
        public ValidationCurriculumVM()
        {
            this.ValidationCurriculumERUs = new HashSet<ValidationCurriculumERUVM>();
            this.SelectedERUs = new List<ERUVM>();
        }

        public int IdValidationCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка със Специалност")]
        [ForeignKey(nameof(CandidateProviderSpeciality))]
        public int IdCandidateProviderSpeciality { get; set; }

        public CandidateProviderSpecialityVM CandidateProviderSpeciality { get; set; }

        [Display(Name = "Връзка с Курс за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Required]
        [Display(Name = "Вид професионална подготовка")] //А1 Обща професионална подготовка, А2 Отраслова професионална подготовка,  А3 Специфична професионална подготовка
        public int IdProfessionalTraining { get; set; }
        public string ProfessionalTraining { get; set; }

        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Предмет' не може да съдържа повече от 1000 символа.")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }// Предмет

        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Тема' не може да съдържа повече от 4000 символа.")]
        [Display(Name = "Тема")]
        public string Topic { get; set; }// Тема

        [Display(Name = "Tеория")]
        public double? Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        public double? Practice { get; set; }//Практика

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        public List<ERUVM> SelectedERUs { get; set; }

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

        public virtual ICollection<ValidationCurriculumERUVM> ValidationCurriculumERUs { get; set; }

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
