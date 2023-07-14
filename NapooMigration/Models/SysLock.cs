using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class SysLock
    {
        public string? SessionId { get; set; }
        public string? LockId { get; set; }
        public DateTime? Ts { get; set; }
    }
}
