using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefProcedureStepStage
    {
        public long Id { get; set; }
        public long? ProcedureId { get; set; }
        public long? StepId { get; set; }
        public long? StageId { get; set; }
        public string? Label { get; set; }
        public long? Iorder { get; set; }
        public bool? IsValid { get; set; }
    }
}
