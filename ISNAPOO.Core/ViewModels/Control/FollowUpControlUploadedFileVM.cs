using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Control
{
    public class FollowUpControlUploadedFileVM
    {
        public int IdFollowUpControlUploadedFile { get; set; }

        [Display(Name = "Връзка с последващ контрол")]
        public int IdFollowUpControl { get; set; }

        public FollowUpControlVM FollowUpControl { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Описание на прикачения файл")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Полето 'Прикачен файл' е задължително!")]
        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Прикачен файл")]
        public  string UploadedFileName { get; set; }
        public string CreatePersonName { get; set; }
        public string ModifyPersonName { get; set; }

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
