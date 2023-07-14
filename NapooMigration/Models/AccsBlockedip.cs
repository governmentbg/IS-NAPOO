using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class AccsBlockedip
    {
        public string Ip { get; set; } = null!;
        public long? Nt { get; set; }
        public long? Lt { get; set; }
        public long? Tf { get; set; }
        public long? Ts { get; set; }
        public long? Sf { get; set; }
        public string? Un { get; set; }
    }
}
