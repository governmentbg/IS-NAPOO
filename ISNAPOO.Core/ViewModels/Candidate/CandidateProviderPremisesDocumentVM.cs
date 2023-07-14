using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Candidate
{
    public class CandidateProviderPremisesDocumentVM
    {
        public int IdCandidateProviderPremisesDocument { get; set; }

        [Required]
        [Comment("Метериална техническа база")]
        public int IdCandidateProviderPremises { get; set; }

        public CandidateProviderPremisesVM CandidateProviderPremises { get; set; }

        /// <summary>   
        /// Документ (сертификат, протокол, становище) за съответствие с правилата и нормите за пожарна безопасност
        /// Документи, издадени от компетентните органи за съответствие на МТБ със здравните изксвания
        /// Документи за наличие на МТБ съобразно ДОС за придобиване на квалификация по професия - учебна дейност на центъра
        /// Документи за наличие на МТБ съобразно ДОС за придобиване на квалификация по професия - административния офис на центъра
        /// </summary>
        [Comment("Вид на документа")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        public int IdDocumentType { get; set; }//Нова номенклатура:  PremisesDocumentType

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Описание на документа' може да съдържа 512 символа!")]
        [Comment("Описание на документа")]
        public string? DocumentTitle { get; set; }

        public string DocumentTypeName { get; set; }

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

        public int MyProperty { get; set; }

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
