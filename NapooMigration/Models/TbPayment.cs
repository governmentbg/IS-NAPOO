using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbPayment
    {
        public long Id { get; set; }
        public long IntProviderId { get; set; }
        public long IntStartedProceduresId { get; set; }
        public int IntProcedurePriceId { get; set; }
        public int? IntSpecialitiesCount { get; set; }
        public int IntSume { get; set; }
        public string? VcText { get; set; }
        public int? IntStatus { get; set; }
        public DateTime TsGen { get; set; }
        public long? IntTransactionId { get; set; }
        public DateTime? TsPayed { get; set; }
    }
}
