using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class SysMailLog
    {
        public long Id { get; set; }
        public int? IntMailType { get; set; }
        public int? IntUserId { get; set; }
        public int? IntProviderId { get; set; }
        public string? VcMailText { get; set; }
        public DateTime? DtMailDate { get; set; }
    }
}
