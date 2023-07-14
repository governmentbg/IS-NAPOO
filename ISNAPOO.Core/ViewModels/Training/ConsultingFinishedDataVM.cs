using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ConsultingFinishedDataVM
    {
        public int IdConsultingClient { get; set; }

        public int IdConsultingDocumentUploadedFile { get; set; }

        [Required(ErrorMessage = "Полето 'Статус на завършване' е задължително!")]
        public int? IdFinishedType { get; set; }

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
    }
}
