using Data.Models.Common;
using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Издадени документи на консултирано лице
    /// </summary>
    [Table("Training_ConsultingClientRequiredDocument")]
    [Comment("Издадени документи на консултирано лице")]
    public class ConsultingClientRequiredDocument : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdConsultingClientRequiredDocument { get; set; }
        public int IdEntity => IdConsultingClientRequiredDocument;

        [Comment("Връзка с консултирано лице")]
        [ForeignKey(nameof(ConsultingClient))]
        public int IdConsultingClient { get; set; }

        public ConsultingClient ConsultingClient { get; set; }

        // Номенклатура: KeyTypeIntCode - "ClientCourseDocumentType"
        [Comment("Тип задължителни документи за курс,курсист")]
        public int IdConsultingRequiredDocumentType { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Comment("Описание")]
        public string? Description { get; set; }//vc_desciption

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public override string UploadedFileName { get; set; }
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

        public override string? MigrationNote { get; set; }

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
    }
}
