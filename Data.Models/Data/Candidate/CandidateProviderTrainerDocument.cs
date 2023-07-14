using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Документи връзка с Преподавател"
    /// </summary>
    [Table("Candidate_ProviderTrainerDocument")]
    [Display(Name = "Документи връзка с Преподавател")]
    public class CandidateProviderTrainerDocument : AbstractUploadFile, IEntity, IModifiable , IDataMigration
    {
        public CandidateProviderTrainerDocument()
        {

        }

        [Key]
        public int IdCandidateProviderTrainerDocument { get; set; }
        public int IdEntity => IdCandidateProviderTrainerDocument;

        [Display(Name = "Връзка с Преподавател")]
        [ForeignKey(nameof(CandidateProviderTrainer))]
        public int IdCandidateProviderTrainer { get; set; }
        public CandidateProviderTrainer CandidateProviderTrainer { get; set; }

        /// <summary>   
        /// Свидетелство
        /// Автобиография
        /// Декларация за съгласие
        /// Диплома за преквалификация
        /// </summary>

        [Display(Name = "Вид на документа")]
        public int IdDocumentType { get; set; }//Нова номенклатура:  TrainerDocumentType

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Описание на документа' не може да съдържа повече от 512 символа.")]
        [Display(Name = "Описание на документа")]
        public string? DocumentTitle { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'UploadedFileName' не може да съдържа повече от 512 символа.")]
        [Display(Name = "UploadedFileName")]
        public override string? UploadedFileName { get; set; }

        [NotMapped]
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

        [NotMapped]
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
        public override int IdModifyUser { get; set; }
        [Required]
        public override DateTime ModifyDate { get; set; }
        #endregion

        #region IDataMigration
        public int? OldId { get; set; }

        public override string? MigrationNote { get; set; }
        #endregion
    }
}
