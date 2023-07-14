using Data.Models.Data.Archive;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Archive
{
    public class AnnualInfoStatusVM
    { 
        public int IdAnnualInfoStatus { get; set; }

        [Comment("Връзка с отчета за годишна информация")]
        public int IdAnnualInfo { get; set; }

        public AnnualInfo AnnualInfo { get; set; }

        [Comment("Статус на отчета за годишна информация")]
        public int IdStatus { get; set; } // Номенклатура - KeyTypeIntCode: "AnnualInfoStatusType"

        /// <summary>        
        /// Работен,Подаден,Одобрен,Върнат
        /// </summary>
        public string StatusValue { get; set; }

        /// <summary>
        /// Working,Submitted,Approved,Returned        
        /// </summary>
        public string StatusValueIntCode { get; set; }

        //Потребител
        public string PersonName { get; set; }

        //Длъжност
        public string Title { get; set; }

        public DateTime? StatusDate { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Comment("Коментар при операция с отчета за годишна информация")]
        public string? Comment { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Comment("Прикачен файл")]
        public string UploadedFileName { get; set; }

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
