using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{

    /// <summary>
    /// Sequence
    /// </summary>
    [Table("Policy")]
    [Display(Name = "Policy")]
    [Index(nameof(PolicyCode), IsUnique = true)]
    public class Policy : IEntity
    {
        [Key]
        public int idPolicy { get; set; }
        public int IdEntity => idPolicy;

        [Required]
        [Display(Name = "Код на Policy")]
        [Comment("Код на Policy")]
        [StringLength(DBStringLength.StringLength100)]        
        public string PolicyCode { get; set; }


        [Display(Name = "Описание на Policy")]
        [Comment("Описание на Policy")]
        [StringLength(DBStringLength.StringLength255)]
        public string PolicyDescription { get; set; }

    }
}
