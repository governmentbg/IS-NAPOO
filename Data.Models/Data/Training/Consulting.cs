using Data.Models.Data.Framework;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models.Data.Training
{
    /// <summary>
    /// Консултация по дейности, предлагани от ЦИПО
    /// </summary>
    [Table("Training_Consulting")]
    [Comment("Консултация по дейности, предлагани от ЦИПО")]
    public class Consulting : IEntity, IModifiable
    {
        [Key]
        public int IdConsulting { get; set; }
        public int IdEntity => IdConsulting;

        [Comment("Връзка с консултирано лице")]
        [ForeignKey(nameof(ConsultingClient))]
        public int IdConsultingClient { get; set; }

        public ConsultingClient ConsultingClient { get; set; }

        [Comment("Вид на дейността по консултиране")]
        public int? IdConsultingType { get; set; } // Номенклатура: KeyTypeIntCode - "ConsultingType"

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Цена (в лева за консултирано лице)")]
        public decimal? Cost { get; set; }

        [Comment("Начин на предоставяне на услугата")]
        public int? IdConsultingReceiveType { get; set; } // Номенклатура: KeyTypeIntCode - "ConsultingReceiveType"

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
