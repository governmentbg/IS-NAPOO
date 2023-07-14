using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{
    /// <summary>
    /// Стойности на номенклатура
    /// </summary>
    [Table("KeyValue")]
    [Display(Name = "Стойности на номенклатура")]
    public class KeyValue : IEntity, IModifiable
    {
        [Key]
        public int IdKeyValue { get; set; }

        public int IdEntity => IdKeyValue;

        [Required]
        [ForeignKey(nameof(KeyType))]
        public int IdKeyType { get; set; }
        public KeyType KeyType { get; set; }

        [Required]
        [StringLength(1000)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string? NameEN { get; set; }

        [StringLength(255)]
        public string KeyValueIntCode { get; set; }
        

        [StringLength(1024)]
        public string? Description { get; set; }

        [StringLength(1024)]
        public string? DescriptionEN { get; set; }

        public int Order { get; set; }

        [StringLength(255)]
        public string? DefaultValue1 { get; set; }

        [StringLength(255)]
        public string? DefaultValue2 { get; set; }

        [StringLength(255)]
        public string? DefaultValue3 { get; set; }

        [StringLength(255)]
        public string? DefaultValue4 { get; set; }

        [StringLength(255)]
        public string? DefaultValue5 { get; set; }
        
        [StringLength(255)]
        public string? DefaultValue6 { get; set; }

        [StringLength(4000)]
        public string? FormattedText { get; set; }

        [StringLength(4000)]
        public string? FormattedTextEN { get; set; }


        [Comment("Определя дали стойността е активна")]
        public bool IsActive { get; set; }

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
