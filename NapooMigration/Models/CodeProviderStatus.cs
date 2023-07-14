using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeProviderStatus
    {
        public long Id { get; set; }
        public string? VcProviderStatusName { get; set; }
        public bool? IsCpo { get; set; }
        public bool? IsBrra { get; set; }
    }
}
