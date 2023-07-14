using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeValidityCheckTarget
    {
        public int Id { get; set; }
        public string VcName { get; set; } = null!;
    }
}
