using Data.Models.Data.ExternalExpertCommission;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.ExternalExpertCommission
{
    public class ExpertDocumentVM : IMapFrom<ExpertDocument>, IMapTo<ExpertDocument>
    {
        public ExpertDocumentVM()
        {
            this.UploadedFileName = String.Empty;
        }

        //[Key]
        public int IdExpertDocument { get; set; }

        //[Display(Name = "Експерт")]        
        public int IdExpert { get; set; }
        public ExpertVM Expert { get; set; }

        //[Display(Name = "Вид на документа")] //Диплома, CV
        [Range(1, int.MaxValue, ErrorMessage = "Изборът на 'Вид на документа' е задължителен")]
        public int IdDocumentType { get; set; }
        public string DocumentTypeName { get; set; }

        //[Required(ErrorMessage = "Изборът на файл в 'Прикачен файл' е задължителен")]
        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Максималната позволена дължина е 512 за полето 'Прикачен файл'")]
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

        public string FileNameSelected { get; set; }

        [Required(ErrorMessage = "Изборът на файл в 'Прикачен файл' е задължителен")]
        public string FileNameToValidate
        {
            get { return (string.IsNullOrWhiteSpace(this.FileNameSelected) ? this.FileName : this.FileNameSelected); }
        }
    }
}
