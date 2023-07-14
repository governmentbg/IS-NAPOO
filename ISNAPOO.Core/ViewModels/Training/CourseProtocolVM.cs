using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using Data.Models.Common;
using System.IO;
using System.Collections.Generic;
using ISNAPOO.Core.ViewModels.Candidate;
using Data.Models.Data.Training;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class CourseProtocolVM
    {
        public CourseProtocolVM()
        {
            this.CourseProtocolGrades = new HashSet<CourseProtocolGradeVM>();
            this.ClientCourseDocuments = new HashSet<ClientCourseDocumentVM>();
        }

        [Key]
        public int IdCourseProtocol { get; set; }

        [Required(ErrorMessage = "Полето 'Курс' е задължително!")]
        [Comment("Връзка с курс за обучение, предлагани от ЦПО")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Курс' е задължително!")]
        public int IdCourse { get; set; }

        public virtual CourseVM Course { get; set; }

        [Required]
        [Comment("Връзка с ЦПО")]
        public int IdCandidateProvider { get; set; }

        public virtual CandidateProviderVM CandidateProvider { get; set; }

        [Comment("Връзка с член на изпитна комисия към курс за обучение")]
        public int? IdCourseCommissionMember { get; set; }

        public virtual CourseCommissionMemberVM CourseCommissionMember { get; set; }

        [Required(ErrorMessage = "Полето 'Номер на протокола' е задължително!")]
        [StringLength(DBStringLength.StringLength50, ErrorMessage = "Полето 'Номер на протокола' може да съдържа до 50 символа!")]
        [Comment("Номер на протокол")]
        public string CourseProtocolNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на протокола' е задължително!")]
        [Comment("Дата на протокол")]
        public DateTime? CourseProtocolDate { get; set; }

        [Required(ErrorMessage = "Полето 'Вид на протокола' е задължително!")]
        [Comment("Вид на протокол")]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Вид на протокола' е задължително!")]
        public int IdCourseProtocolType { get; set; } // Номенклатура 'CourseProtocolType'

        public string CourseProtocolTypeName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

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

        public bool IsClientWithDocumentPresent { get; set; }

        public string NameAndDate => $"№{this.CourseProtocolNumber}/{this.CourseProtocolDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";

        public int Order { get; set; }

        public string CoursePeriod { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }

        public virtual ICollection<CourseProtocolGradeVM> CourseProtocolGrades { get; set; }

        public virtual ICollection<ClientCourseDocumentVM> ClientCourseDocuments { get; set; }

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
