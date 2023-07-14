using Data.Models.Data.Common;
using ISNAPOO.Core.Mapping;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ISNAPOO.Core.ViewModels.Common
{
    public class KeyTypeVM : IMapFrom<KeyType>
    {
        public KeyTypeVM()
        {
            this.KeyValues = new HashSet<KeyValue>();
        }
        public int IdKeyType { get; set; }

        [Required(ErrorMessage = "Полето 'Наименование' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Наименование' не може да съдържа повече от 255 символа!")]
        public string KeyTypeName { get; set; }

        [Required(ErrorMessage = "Полето 'Код' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Код' не може да съдържа повече от 255 символа!")]
        public string KeyTypeIntCode { get; set; }

        [Required(ErrorMessage = "Полето 'Описание' е задължително!")]
        [StringLength(255, ErrorMessage = "Полето 'Описание' не може да съдържа повече от 255 символа!")]
        public string Description { get; set; }

        public bool IsSystem { get; set; }

        public int IdEntity => this.IdKeyType;

        public int IdCreateUser { get; set; }

        public DateTime CreationDate { get; set; }

        public int IdModifyUser { get; set; }

        public DateTime ModifyDate { get; set; }

        public string ModifyPersonName { get; set; }
        public string CreatePersonName { get; set; }

        public virtual ICollection<KeyValue> KeyValues { get; set; }
    }
}
