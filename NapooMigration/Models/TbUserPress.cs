using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbUserPress
    {
        public int IntId { get; set; }
        public string? VcUser { get; set; }
        public string? VcPass { get; set; }
        public string? VcObl { get; set; }
        public string? VcName { get; set; }
    }
}
