using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefProcedureStep
    {
        public long Id { get; set; }
        public long? ProcedureId { get; set; }
        public long? StepId { get; set; }
        public string? Label { get; set; }
        public long? Iorder { get; set; }
        public bool? IsValid { get; set; }
        public string? LabelEn { get; set; }
        public string? LabelReg { get; set; }
        public string? LabelRegEn { get; set; }
    }
}
