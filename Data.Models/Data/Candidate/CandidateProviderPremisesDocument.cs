using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Migrations;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Candidate
{


    /// <summary>
    /// Документи връзка с Метериална техническа"
    /// </summary>
    [Table("Candidate_ProviderPremisesDocument")]
    [Display(Name = "Документи връзка с Метериална техническа")]
    public class CandidateProviderPremisesDocument : AbstractUploadFile, IEntity, IModifiable
    {
        public CandidateProviderPremisesDocument()
        {

        }

        [Key]
        public int IdCandidateProviderPremisesDocument { get; set; }
        public int IdEntity => IdCandidateProviderPremisesDocument;

        [Required]
        [Comment("Метериална техническа база")]
        [ForeignKey(nameof(CandidateProviderPremises))]
        public int IdCandidateProviderPremises { get; set; }
        public CandidateProviderPremises CandidateProviderPremises { get; set; }

        /// <summary>   
        /// Документ (сертификат, протокол, становище) за съответствие с правилата и нормите за пожарна безопасност
        /// Документи, издадени от компетентните органи за съответствие на МТБ със здравните изксвания
        /// Документи за наличие на МТБ съобразно ДОС за придобиване на квалификация по професия - учебна дейност на центъра
        /// Документи за наличие на МТБ съобразно ДОС за придобиване на квалификация по професия - административния офис на центъра
        /// </summary>

        [Comment("Вид на документа")]
        public int IdDocumentType { get; set; }//Нова номенклатура:  PremisesDocumentType

        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Описание на документа' не може да съдържа повече от 512 символа.")]
        [Comment("Описание на документа")]
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

