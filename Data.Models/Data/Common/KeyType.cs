using Data.Models.Data.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// Тип номенклатура
    /// </summary>
    [Table("KeyType")]
    [Display(Name = "Тип номенклатура")]
    public class KeyType : IEntity, IModifiable
    {
        [Key]
        public int IdKeyType { get; set; }
        public int IdEntity => IdKeyType;

        [StringLength(255)]
        public string KeyTypeName { get; set; }

        [StringLength(255)]
        public string KeyTypeIntCode { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public bool IsSystem { get; set; }

        
        [Required]
        public int IdCreateUser { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        [Required]
        public int IdModifyUser { get; set; }

        [Required]
        public DateTime ModifyDate { get; set; }

        public IEnumerable<KeyValue> KeyValues { get; set; }
    }
}
