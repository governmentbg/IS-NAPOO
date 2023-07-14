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
    public class ConsultingClientRequiredDocumentVM : IMapFrom<ConsultingClientRequiredDocument>
    {

        public int IdConsultingClientRequiredDocument { get; set; }

        [Display(Name = "Връзка с консултирано лице")]
        public int IdConsultingClient { get; set; }

        public ConsultingClientVM ConsultingClient { get; set; }

        // Номенклатура: KeyTypeIntCode - "ClientCourseDocumentType"
        [Required(ErrorMessage = "Полето 'Вид на документа' е задължително!")]
        [Display(Name = "Тип задължителни документи за курс,курсист")]
        public int? IdConsultingRequiredDocumentType { get; set; }
        public string ConsultingRequiredDocumentNameType { get; set; }

        [StringLength(DBStringLength.StringLength255)]
        [Display(Name = "Описание")]
        public string? Description { get; set; }//vc_desciption

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

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

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
