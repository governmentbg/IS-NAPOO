using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Common;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.Candidate;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationProtocolVM
    {
        public ValidationProtocolVM()
        {
            this.ValidationClientDocuments = new HashSet<ValidationClientDocumentVM>();

        }

        [Key]
        public int IdValidationProtocol { get; set; }

        [Comment("Връзка с клиент по процедура за валидиране")]
        [ForeignKey(nameof(ValidationClient))]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Comment("Връзка с ЦПО")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider { get; set; }
       
        [Comment("Връзка с член на изпитна комисия към курс за валидиране")]
        [ForeignKey(nameof(ValidationCommissionMember))]
        public int? IdValidationCommissionMember { get; set; }

        public ValidationCommissionMemberVM ValidationCommissionMember { get; set; }
       
        public CandidateProviderVM CandidateProvider { get; set; }

        [Required(ErrorMessage = "Полето 'Номер на протокола' е задължително!")]
        [StringLength(DBStringLength.StringLength50)]
        [Comment("Номер на протокол")]
        public string ValidationProtocolNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на протокола' е задължително!")]
        [Comment("Дата на протокол")]
        public DateTime? ValidationProtocolDate { get; set; }

        [Required]
        [Comment("Вид на протокол")]
        public int IdValidationProtocolType { get; set; } // Номенклатура 'CourseProtocolType'

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

        public string ValidationProtocolTypeName { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Оценка по теория")]
        public decimal? TheoryResult { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Оценка по практика")]
        public decimal? PracticeResult { get; set; }

        [Column(TypeName = "decimal(3, 2)")]
        [Comment("Обща оценка от теория и практика")]
        public decimal? FinalResult { get; set; }

        public string CreatePersonName { get; set; }

        public string ModifyPersonName { get; set; }
        public string NameAndDate => $"№{this.ValidationProtocolNumber}/{this.ValidationProtocolDate.Value.ToString(GlobalConstants.DATE_FORMAT)}г.";

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
        public ICollection<ValidationClientDocumentVM> ValidationClientDocuments { get; set; }
        public ICollection<ValidationProtocolGradeVM> ValidationProtocolGrades { get; set; }

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
