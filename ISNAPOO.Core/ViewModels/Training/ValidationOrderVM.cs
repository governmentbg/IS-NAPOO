using Data.Models.Common;
using Data.Models.Data.Training;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ValidationOrderVM
    {
        public int IdValidationOrder { get; set; }

        [Display(Name = "Връзка с курс за валидиране")]
        public int IdValidationClient { get; set; }

        public ValidationClientVM ValidationClient { get; set; }

        [Required(ErrorMessage = "Полето 'Номер на заповед' е задължително!")]
        [StringLength(DBStringLength.StringLength50)]
        [Display(Name = "Номер на заповед")]
        public string OrderNumber { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на заповед' е задължително!")]
        [Display(Name = "Дата на заповед")]
        public DateTime? OrderDate { get; set; }

        [StringLength(DBStringLength.StringLength1000)]
        [Display(Name = "Описание на заповед")]
        public string? Description { get; set; }

        [StringLength(DBStringLength.StringLength512)]
        [Display(Name = "Прикачен файл")]
        public string UploadedFileName { get; set; }
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
