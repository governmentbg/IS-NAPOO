using Data.Models.Data.Common;
using ISNAPOO.Core.Mapping;
using System;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Common.ValidationModels
{
    public class KeyValueVM : IMapTo<KeyValue>, IMapFrom<KeyValue>
    {
        public int IdKeyValue { get; set; }

        public int IdKeyType { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Наименование' не може да съдържа повече от 255 символа!")]
        public string Name { get; set; }

        public string NameEN { get; set; }
        
        [Required(ErrorMessage = "Полето 'Код' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Код' не може да съдържа повече от 255 символа!")]
        public string KeyValueIntCode { get; set; }

        [Required(ErrorMessage = "Полето 'Описание' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Описание' не може да съдържа повече от 255 символа!")]
        public string Description { get; set; }

        public string DescriptionEN { get; set; }

        [Required(ErrorMessage = "Полето 'Поредност' е задължително!")]
        [Range(0, 10000, ErrorMessage = "Редът трябва да е число между 0 и 1000")]
        public int Order { get; set; }

        public string? DefaultValue1 { get; set; }

        public string? DefaultValue2 { get; set; }

        public string? DefaultValue3 { get; set; }

        public string? DefaultValue4 { get; set; }
        
        public string? DefaultValue5 { get; set; } 
        public string? DefaultValue6 { get; set; }

        public string? FormattedText { get; set; }

        public string? FormattedTextEN { get; set; }

        public int IdCreateUser { get; set; }

        public DateTime CreationDate { get; set; }

        public int IdModifyUser { get; set; }

        public DateTime ModifyDate { get; set; }
        public bool IsActive { get; set; }
        public string IsActiveStr 
        {
            get
            {
                return IsActive ? "Активен" : "Неактивен";
            }
        }
        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }
    }
}
