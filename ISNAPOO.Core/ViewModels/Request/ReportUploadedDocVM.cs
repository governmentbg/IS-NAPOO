using ISNAPOO.Common.Constants;
using ISNAPOO.Core.ViewModels.Candidate;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class ReportUploadedDocVM
    {
        public int IdReportUploadedDoc { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка с CPO,CIPO - Обучаваща институция")]
        public int IdRequestReport { get; set; }

        public virtual RequestReportVM RequestReport { get; set; }

        /// <summary>
        /// Отчет на документи с фабрична номерация
        /// Протокол за унищожени документи с фабрична номерация        
        /// Приемно-предавателен протокол за документи с фабрична номерация           
        /// </summary>
        [Comment("Тип документ")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на документ' е задължително!")]
        public int IdTypeReportUploadedDocument { get; set; }//Нова номенклатура TypeReportUploadedDocument: 

        public string TypeReportUploadedDocumentName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Прикачен файл' може да съдържа до 512 символа!")]
        [Comment("Прикачен файл")]
        public string? UploadedFileName { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Описание' може да съдържа до 512 символа!")]
        [Comment("Описание")]
        public string Description { get; set; }

        public string UploadedByName { get; set; }

        public MemoryStream UploadedFileStream { get; set; }

        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts[arrNameParts.Length - 1] : string.Empty);
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
