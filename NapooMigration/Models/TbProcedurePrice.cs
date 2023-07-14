using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProcedurePrice
    {
        public int Id { get; set; }
        public string VcName { get; set; } = null!;
        public int? IntPrice { get; set; }
        public long? IntPocedureId { get; set; }
        public bool? BoolCountDependant { get; set; }
        public int? IntCountMin { get; set; }
        public int? IntCountMax { get; set; }
        public bool? BoolCpo { get; set; }
        public bool? BoolCipo { get; set; }
        public DateTime DtFrom { get; set; }
        public DateTime? DtTo { get; set; }
        public int? IntPaymentPeriod { get; set; }
        public long? IntProcedureStepsId { get; set; }
    }
}
