using ISNAPOO.Common.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Control
{
    public class FollowUpControlDocumentVM
    {
        public int IdFollowUpControlDocument { get; set; }

        [Required]
        [Display(Name = "Връзка с последващ контрол")]
        public int IdFollowUpControl { get; set; }

        public FollowUpControlVM FollowUpControl { get; set; }

        // Нова номенклатура: Тип на документа при последващ контрол "FollowUpControlDocumentType" - Заповед за осъществяване на последващ контрол, Констативен протокол от извършен последващ контрол, Писмо до ЦПО, Доклад за проверка за осъществяване на последващ контрол
        [Display(Name = "Тип на документа при последващ контрол")]
        public int IdDocumentType { get; set; }
        public string DocumentTypeName { get; set; }

        [Display(Name = "ID на работен документ в деловодната система на НАПОО")]
        public int? DS_ID { get; set; }

        [Display(Name = "Дата на работен документ в деловодната система на НАПОО")]
        public DateTime? DS_DATE { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на работен документ в деловодната система на НАПОО")]
        public string? DS_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на работен документ в деловодната система на НАПОО")]
        public string? DS_DocNumber { get; set; }

        [Display(Name = "Официален номер на документ в деловодната система на НАПОО")]
        public int? DS_OFFICIAL_ID { get; set; }

        [Display(Name = "Дата на официален документ в деловодната система на НАПОО")]
        public DateTime? DS_OFFICIAL_DATE { get; set; }

        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "GUID на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_GUID { get; set; }

        [StringLength(DBStringLength.StringLength20)]
        [Display(Name = "Номер на официален документ в деловодната система на НАПОО")]
        public string? DS_OFFICIAL_DocNumber { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Преписка в деловодната система на НАПОО")]
        public string? DS_PREP { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Връзка към документа  в деловодната система на НАПОО")]
        public string? DS_LINK { get; set; }

        [Required(ErrorMessage = "Полето '№ на документ' е задължително!")]
        public string? ApplicationNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Дата' е задължително!")]
        public DateTime? ApplicationDate { get; set; }
        [DefaultValue(false)]
        [Display(Name = "Стойност дали документа е взет от деловодната система на НАПОО")]
        public bool IsFromDS { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Прикачен файл от център")]
        public string? UploadedFileName { get; set; }

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
