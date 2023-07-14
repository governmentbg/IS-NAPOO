using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderDocumentVM 
    {
        public int IdCandidateProviderDocument { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public CandidateProviderVM CandidateProvider { get; set; }

        /// <summary>   
        /// Сертификат за съответствие с правилата и нормите за пожарна безопасност
        /// Протокол за съответствие с правилата и нормите за пожарна безопасност
        /// </summary>
        [Comment("Вид на документа")]
        [Required(ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        public int IdDocumentType { get; set; }//Нова номенклатура:  CandidateProviderDocumentType

        public string DocumentTypeName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Описание на документа' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Описание на документа")]
        [Comment("Описание на документа")]
        public string? DocumentTitle { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'UploadedFileName' не може да съдържа повече от 512 символа.")]
        [Display(Name = "UploadedFileName")]
        [Comment("UploadedFileName")]
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
