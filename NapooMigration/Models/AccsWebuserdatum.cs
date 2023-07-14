using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class AccsWebuserdatum
    {
        public string Sessiondata { get; set; } = null!;
        public long? Lastlogin { get; set; }
        public long? Firstlogin { get; set; }
        public long? Nsec { get; set; }
        public string? Udat { get; set; }
        public string? Adat { get; set; }
    }
}
