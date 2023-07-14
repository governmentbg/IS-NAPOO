using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Data.Models.Data.Framework;
using Data.Models.Common;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;

namespace Data.Models.Data.SPPOO
{
    /// <summary>
    /// Прикачен файл за наредба за правоспособност
    /// </summary>
    [Table("SPPOO_LegalCapacityOrdinanceUploadedFile")]
    [Display(Name = "Област")]
    public class LegalCapacityOrdinanceUploadedFile : AbstractUploadFile, IEntity, IModifiable
    {
        [Key]
        public int IdLegalCapacityOrdinanceUploadedFile { get; set; }
        public int IdEntity => IdLegalCapacityOrdinanceUploadedFile;

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Comment("Вид на наредбата за правоспособност")]
        public int IdLegalCapacityOrdinanceType { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
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
