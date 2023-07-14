namespace ISNAPOO.Core.ViewModels.SPPOO
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Data.Models;
    using ISNAPOO.Core.Mapping;

    public class OrderVM : IMapFrom<SPPOOOrder>, IMapTo<SPPOOOrder>
    {
        
        public OrderVM()
        {
            this.OrderNumber = string.Empty;
            this.UploadedFileName = string.Empty;
            this.OrderType = string.Empty;
        }

        //[Key]
        public int IdOrder { get; set; }


        [Required(ErrorMessage = "Полето 'Номер на Заповед' е задължително!")]
        [StringLength(100, ErrorMessage = "Максималната позволена дължина е 100 за полето 'Номер на Заповед'")]
        public string OrderNumber { get; set; }

        public string OrderNumberWithOrderDate { get; set; }

        [Required(ErrorMessage = "Полето 'Дата на Заповед' е задължително!")]
        public DateTime? OrderDate { get; set; }

        public DateTime? OrderDateFrom { get; set; }
        public Boolean IsExactOrderDate { get; set; }
        public DateTime? OrderDateTo { get; set; }

        public string UploadedFileName { get; set; }

        public string OrderType { get; set; }


        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }


        public int IdTypeChange { get; set; }

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
