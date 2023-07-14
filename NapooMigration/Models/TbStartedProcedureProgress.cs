using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbStartedProcedureProgress
    {
        public long Id { get; set; }
        public long? StartedProcedureId { get; set; }
        public long? UserId { get; set; }
        public long? StepId { get; set; }
        public long? StageId { get; set; }
        public DateTime? Ts { get; set; }
    }
}
