using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderTrainerDocumentVM
    {
        public int IdCandidateProviderTrainerDocument { get; set; }

        [Display(Name = "Връзка с Преподавател")]
        public int IdCandidateProviderTrainer { get; set; }

        public CandidateProviderTrainerVM CandidateProviderTrainer { get; set; }

        /// <summary>   
        /// Свидетелство
        /// Автобиография
        /// Декларация за съгласие
        /// Диплома за преквалификация
        /// </summary>
        [Display(Name = "Вид на документа")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        public int IdDocumentType { get; set; }//Нова номенклатура:  TrainerDocumentType

        public string DocumentTypeName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Описание на документа' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Описание на документа")]
        public string? DocumentTitle { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'UploadedFileName' не може да съдържа повече от 512 символа.")]
        [Display(Name = "UploadedFileName")]
        public string? UploadedFileName { get; set; }

        public string UploadedByName { get; set; }

        public MemoryStream UploadedFileStream { get; set; }

        public string FileName { get; set; }

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

        public string CreationDateAsStr => $"{this.CreationDate.ToString(GlobalConstants.DATE_FORMAT)} г.";

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
