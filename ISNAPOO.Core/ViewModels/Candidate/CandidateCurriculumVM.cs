using Data.Models.Data.Candidate;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.DOC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateCurriculumVM
    {
        public CandidateCurriculumVM()
        {
            this.SelectedERUs = new List<ERUVM>();
            this.CandidateCurriculumERUs = new HashSet<CandidateCurriculumERUVM>();
        }

        [Key]
        public int IdCandidateCurriculum { get; set; }

        [Required]
        [Display(Name = "Връзка със Специалност")]
        public int IdCandidateProviderSpeciality { get; set; }

        public CandidateProviderSpecialityVM CandidateProviderSpeciality { get; set; }

        [Display(Name = "Връзка с промяна на учебна програма за специалност")]
        public int? IdCandidateCurriculumModification { get; set; }

        public virtual CandidateCurriculumModificationVM CandidateCurriculumModification { get; set; }

        [Required(ErrorMessage = "Полето 'Вид професионална подготовка' е задължително!")]
        [Range(1, int.MaxValue, ErrorMessage = "Полето 'Вид професионална подготовка' е задължително!")]
        [Display(Name = "Вид професионална подготовка")]//А1 Обща професионална подготовка, А2 Отраслова професионална подготовка, А3 Специфична професионална подготовка, Б Разширена професионална подготовка
        public int IdProfessionalTraining { get; set; }

        public string ProfessionalTraining { get; set; }

        [Required(ErrorMessage = "Полето 'Предмет' е задължително!")]
        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Предмет' може да съдържа до 1000 символа!")]
        [Display(Name = "Предмет")]
        public string Subject { get; set; }// Предмет

        [Required(ErrorMessage = "Полето 'Тема' е задължително!")]
        [StringLength(DBStringLength.StringLength4000, ErrorMessage = "Полето 'Тема' може да съдържа до 4000 символа!")]
        [Display(Name = "Тема")]
        public string Topic { get; set; }// Тема

        [Display(Name = "Tеория")]
        [Range(0, double.MaxValue, ErrorMessage = "Полето 'Теория' трябва да има стойност по-голяма от 0!")]
        public double? Theory { get; set; }//Tеория

        [Display(Name = "Практика")]
        [Range(0, double.MaxValue, ErrorMessage = "Полето 'Практика' трябва да има стойност по-голяма от 0!")]
        public double? Practice { get; set; }//Практика

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

        public bool IsEdit { get; set; }

        public int IdForGrid { get; set; }

        public int IdForCurriculumModal { get; set; }

        public List<ERUVM> SelectedERUs { get; set; }

        public string ERUsJoined => this.SelectedERUs.Any() ? string.Join("", this.SelectedERUs.Select(x => x.Code)) : string.Empty;

        public virtual ICollection<CandidateCurriculumERUVM> CandidateCurriculumERUs { get; set; }

        public string ERUsForExport => string.Join(Environment.NewLine, this.SelectedERUs.Select(x => x.Code));

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
