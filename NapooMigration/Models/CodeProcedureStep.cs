using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeProcedureStep
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public bool? IsValid { get; set; }
    }
}
