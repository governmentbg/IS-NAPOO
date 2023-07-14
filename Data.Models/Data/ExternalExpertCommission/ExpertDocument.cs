using Data.Models.Common;
using Data.Models.Data.Framework;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.ExternalExpertCommission
{
     
    /// <summary>
    /// Документи за експерта 
    /// </summary>
    [Table("ExpComm_ExpertDocument")]
    [Display(Name = "Документи за експерта ")]
    public class ExpertDocument : AbstractUploadFile, IEntity, IModifiable

    {
        public ExpertDocument()
        {
            this.UploadedFileName = String.Empty;
        }

        [Key]
        public int IdExpertDocument { get; set; }
        public int IdEntity => IdExpertDocument;

        [Required]
        [Display(Name = "Експерт")]
        [ForeignKey(nameof(Expert))]
        public int IdExpert { get; set; }
        public virtual Expert Expert { get; set; }

        [Required]
        [Display(Name = "Вид на документа")]//Диплома, CV
        public int IdDocumentType { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Прикачен файл")]
        public override string UploadedFileName { get; set; }

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
