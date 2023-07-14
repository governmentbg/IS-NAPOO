using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ISNAPOO.Core.ViewModels.Training
{
    public class ConsultingVM
    {
        [Key]
        public int IdConsulting { get; set; }

        [Comment("Връзка с консултирано лице")]
        public int IdConsultingClient { get; set; }

        public virtual ConsultingClientVM ConsultingClient { get; set; }

        [Comment("Вид на дейността по консултиране")]
        public int? IdConsultingType { get; set; } // Номенклатура - KeyTypeIntCode: "ConsultingType"

        public string? ConsultingTypeValue { get; set; }

        [Column(TypeName = "decimal(10, 2)")]
        [Comment("Цена (в лева за консултирано лице)")]
        public decimal? Cost { get; set; }

        [Comment("Начин на предоставяне на услугата")]
        public int? IdConsultingReceiveType { get; set; } // Номенклатура: KeyTypeIntCode - "ConsultingReceiveType"

        public string ConsultingReceiveTypeValue { get; set; }

        public string CostAsStr => this.Cost.HasValue ? $"{this.Cost.Value.ToString("f2")} лв" : string.Empty;

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
