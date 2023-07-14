using Data.Models.Data.Common;
using ISNAPOO.Common.Constants;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class TemplateDocumentVM : IMapTo<TemplateDocument>
    {
        public int idTemplateDocument { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование на шаблона' е задължително!")]
        [StringLength(DBStringLength.StringLength255, ErrorMessage = "Наименованието на шаблона е твърде дълго!")]
        [Display(Name = "Наименование на шаблона")]
        public string TemplateName { get; set; }//Доклад длъжностно лице при лицензиране на ЦПО


        [StringLength(DBStringLength.StringLength1000, ErrorMessage = "Описание на шаблона е твърде дълго!")]
        [Display(Name = "Описание на шаблона")]
        public string TemplateDescription { get; set; }


        [StringLength(DBStringLength.StringLength512, ErrorMessage = "Полето 'Път до шаблона' не може да съдържа повече от 512 символа!")]
        [Display(Name = "Път до шаблона")]
        public string TemplatePath { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Статус на валидност' е задължително!")]
        [Display(Name = "Статус на валидност")]
        public int IdStatus { get; set; }//Активен, Неактивен, Изтрит

        public string StatusName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Модул' е задължително!")]
        [Display(Name = "Модул")]
        public int IdModule { get; set; }//ЦПО - Лицензиране, ЦПО - Изменение на лиценз, ЦПО - Последващ контрол

        public string ModuleName { get; set; }

        [Range(1, GlobalConstants.MAX_VALUE_FOR_REQUIRED_ID, ErrorMessage = "Полето 'Тип на приложението' е задължително!")]
        [Display(Name = "Тип на приложението")]
        public int IdApplicationType { get; set; }//Prilojenie 1 - doklad dlajnostno lize, Prilojenie 2_zapoved, Prilojenie 3_uvedomitelno pismo_taksa

        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public string ApplicationTypeName { get; set; }
        public string ApplicationTypeIntCode { get; set; }

        public string UploadedFileName { get; set; }

        public DateTime TemplateDocumentDate { get; set; }
        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

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
