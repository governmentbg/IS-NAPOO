using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbVersion
    {
        public long Id { get; set; }
        public string? VcComment { get; set; }
        public DateTime? DtTimestamp { get; set; }
    }
}
