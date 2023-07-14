using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISNAPOO.Core.ViewModels.Training
{
    [Table("Training_CourseDocumentUploadedFile")]
    [Comment("Прикачени файлове за документи на курсисти")]
    public class CourseDocumentUploadedFileVM
    {
        public int IdCourseDocumentUploadedFile { get; set; }

        [Required]
        [Comment("Връзка с издадени документи на курсисти")]
        [ForeignKey(nameof(ClientCourseDocument))]
        public int IdClientCourseDocument { get; set; }

        public virtual ClientCourseDocumentVM ClientCourseDocument { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Прикачен файл' не може да съдържа повече от 512 символа.")]
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

        #region IDataMigration
        public int? OldId { get; set; }

        public string? MigrationNote { get; set; }
        #endregion
    }
}
