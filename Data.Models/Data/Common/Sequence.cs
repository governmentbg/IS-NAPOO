using Data.Models.Data.Framework;
using ISNAPOO.Common.Constants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Common
{

    /// <summary>
    /// Sequence
    /// </summary>
    [Table("Sequence")]
    [Display(Name = "Sequence")]
    public class Sequence : IEntity
    {
        [Key]
        public int idSequence { get; set; }
        public int IdEntity => idSequence;

        [Required]
        [Display(Name = "Ресурс")]
        [StringLength(DBStringLength.StringLength100)]
        public string Resource { get; set; }

        [Display(Name = "ИД На ресурс")]
        public int? IdResource { get; set; }

        [Display(Name = "Година")]
        public int? Year { get; set; }

        [Display(Name = "Следваща стойност")]
        public long NextVal { get; set; }

    }
}
