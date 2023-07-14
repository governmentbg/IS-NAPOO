using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Data.Models.Common;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ConsultingDocumentUploadedFileVM
    {
        public int IdConsultingDocumentUploadedFile { get; set; }

        [Comment("Връзка с консултирано лице")]
        public int IdConsultingClient { get; set; }

        public virtual ConsultingClientVM ConsultingClient { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

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
