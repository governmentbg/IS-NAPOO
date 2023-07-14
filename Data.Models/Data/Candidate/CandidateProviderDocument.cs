using Data.Models.Common;
using Data.Models.Data.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Документи връзка с CPO,CIPO - Обучаваща институция"
    /// </summary>
    [Table("Candidate_ProviderDocument")]
    [Display(Name = "Документи връзка с CPO,CIPO - Обучаваща институция")]
    public class CandidateProviderDocument : AbstractUploadFile, IEntity, IModifiable
    {
        public CandidateProviderDocument()
        {

        }

        [Key]
        public int IdCandidateProviderDocument { get; set; }
        public int IdEntity => IdCandidateProviderDocument;

        [Comment( "Връзка с CPO,CIPO - Обучаваща институция")]
        [ForeignKey(nameof(CandidateProvider))]
        public int IdCandidateProvider  { get; set; }
        public CandidateProvider CandidateProvider  { get; set; }

        /// <summary>   
        /// Сертификат за съответствие с правилата и нормите за пожарна безопасност
        /// Протокол за съответствие с правилата и нормите за пожарна безопасност
        /// </summary>

        [Comment("Вид на документа")]
        public int IdDocumentType { get; set; }//Нова номенклатура:  CandidateProviderDocumentType

        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Полето 'Описание на документа' не може да съдържа повече от 1000 символа.")]
        [Display(Name = "Описание на документа")]
        [Comment("Описание на документа")]
        public string? DocumentTitle { get; set; }

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'UploadedFileName' не може да съдържа повече от 512 символа.")]
        [Display(Name = "UploadedFileName")]
        [Comment("UploadedFileName")]
        public override string? UploadedFileName { get; set; }


        [Comment("Допълнителни документи")]
        public bool IsAdditionalDocument { get; set; }//Допълнителни документи

        /// TODO: Да се махната тези полета
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

        /// TODO: Да се махната тези полета
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
