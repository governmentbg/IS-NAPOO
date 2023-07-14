using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class DuplicateIssueVM
    {
        public int IdClientCourseDocument { get; set; }

        public int IdValidationClientDocument { get; set; }

        [Required(ErrorMessage = "Полето 'Курс' е задължително!")]
        public int? IdCourse { get; set; }

        [Required(ErrorMessage = "Полето 'Курсист' е задължително!")]
        public int? IdClientCourse { get; set; }

        [Required(ErrorMessage = "Полето 'Валидирано лице' е задължително!")]
        public int? IdValidationClient { get; set; }

        public CourseVM Course { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        public int IdCourseDocumentUploadedFile { get; set; }

        public int IdValidationDocumentUploadedFile { get; set; }

        [Display(Name = "UploadedFileName")]
        public string? UploadedFileName { get; set; }

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

        public bool IsRIDPKDocumentSubmitted { get; set; }

        public int IdCourseProtocol { get; set; }

        public int IdValidationProtocol { get; set; }

        public string FinalResult { get; set; }

        public int? FinishedYear { get; set; } = DateTime.Now.Year;

        public bool HasDocumentFabricNumber { get; set; }

        public int? IdDocumentSerialNumber { get; set; }

        public string DocumentRegNo { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на издаване' е задължително!")]
        public DateTime? DocumentDate { get; set; }

        public string DocumentProtocol { get; set; }

        public string CourseTypeFromToken { get; set; }

        public string DocumentTypeName { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

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
