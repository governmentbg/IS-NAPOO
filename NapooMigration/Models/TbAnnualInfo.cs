using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbAnnualInfo
    {
        public long Id { get; set; }
        public long IntYear { get; set; }
        public long? IntProviderId { get; set; }
        public string? VcName { get; set; }
        public string? VcPosition { get; set; }
        public string? VcPhone { get; set; }
        public string? VcEmail { get; set; }
        public DateTime? DtTimestamp { get; set; }
        public int? IntStatus { get; set; }
    }
}
