using Data.Models.Common;
using Data.Models.Data.SPPOO;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.SPPOO
{
    public class LegalCapacityOrdinanceUploadedFileVM
    {
        public int IdLegalCapacityOrdinanceUploadedFile { get; set; }

        // Номенклатура: KeyTypeIntCode - "LegalCapacityOrdinanceType"
        [Required(ErrorMessage = "Полето 'Вид на наредбата за правоспособност' е задължително!")]
        [Display(Name = "Вид на наредбата за правоспособност")]
        public int? IdLegalCapacityOrdinanceType { get; set; }
        public string LegalCapacityOrdinanceTypeName { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Прикачен файл")]
        public string UploadedFileName { get; set; }


        public string FileName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.UploadedFileName) && this.UploadedFileName != "#")
                {
                    var arrNameParts = this.UploadedFileName.Split(new string[2] { "\\", "/" }, StringSplitOptions.RemoveEmptyEntries);

                    return (arrNameParts.Length > 0 ? arrNameParts.Last() : string.Empty);
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

        public string CreatePersonName { get; set; }
        public string ModifyPersonName { get; set; }

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
