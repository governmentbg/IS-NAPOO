using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProcedureProgress
    {
        public long Id { get; set; }
        public long? IntProcedureDecisionId { get; set; }
        public long? IntProviderId { get; set; }
        public string? VcProviderOwner { get; set; }
        public long? IntProcedureNumber { get; set; }
        public DateTime? DtDecisionDate { get; set; }
        public string? IntDecisionProtocolNumber { get; set; }
        public DateTime? DtSystemDate { get; set; }
    }
}
