using ISNAPOO.Core.ViewModels.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class ImportUsersVM
    {
        public ImportUsersVM()
        {
            this.Roles = new List<ApplicationRoleVM>();
        }

        [Required(ErrorMessage = "Полето 'Вид на импорта' е задължително!")]
        public int? IdImportType { get; set; }

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

        public List<ApplicationRoleVM> Roles { get; set; }

        public bool AllowSentEmails { get; set; }
    }
}
