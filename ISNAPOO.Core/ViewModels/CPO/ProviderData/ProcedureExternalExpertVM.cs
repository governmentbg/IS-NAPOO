using Data.Models.Data.ProviderData;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using ISNAPOO.Core.ViewModels.ExternalExpertCommission;
using ISNAPOO.Core.ViewModels.SPPOO;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace ISNAPOO.Core.ViewModels.CPO.ProviderData
{
    public class ProcedureExternalExpertVM : IMapFrom<ProcedureExternalExpert>, IMapTo<ProcedureExternalExpert>
    {

        [Key]
        public int IdProcedureExternalExpert { get; set; }

        [Display(Name = "Връзка с  Данни за Процедура за лицензиране")]
        public int IdStartedProcedure { get; set; }
        public StartedProcedureVM StartedProcedure { get; set; }


        [Required]
        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Водещ експерт' е задължително!")]
        [Display(Name = "Връзка с  Експерт")]
        public int IdExpert { get; set; }
        public ExpertVM Expert { get; set; }

        public bool? IsLeadingExpert { get; set; }

        [Display(Name = "Професионално направление")]
        public int? IdProfessionalDirection { get; set; }
        public ProfessionalDirectionVM ProfessionalDirection { get; set; }

        [Display(Name = "Данни за Процедура за лицензиране - прогрес")]
        [ForeignKey(nameof(ProcedureDocument))]
        public int? IdProcedureDocument { get; set; }
        public ProcedureDocumentVM ProcedureDocument { get; set; }

        [Display(Name = "Активен/Неактивен")]
        [Comment("Показва статуса на външния експерт спрямо процедурата")]
        public bool IsActive { get; set; }

        [Comment("Дата на прикачване на доклада")]
        public DateTime? UploadDate { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string? UploadedFileName { get; set; }

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
