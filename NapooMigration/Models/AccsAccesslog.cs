using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class AccsAccesslog
    {
        public long? Timestart { get; set; }
        public long? Timeend { get; set; }
        public long? Timeused { get; set; }
        public long? Userid { get; set; }
        public long? Ggid { get; set; }
        public long? Lgid { get; set; }
        public long Sessionid { get; set; }
        public string? Ip { get; set; }
    }
}
