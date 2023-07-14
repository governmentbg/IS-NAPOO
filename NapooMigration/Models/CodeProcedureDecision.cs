using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeProcedureDecision
    {
        public long Id { get; set; }
        public string? VcProcedureDecision { get; set; }
        public string? VcProcedureDecisionEn { get; set; }
    }
}
