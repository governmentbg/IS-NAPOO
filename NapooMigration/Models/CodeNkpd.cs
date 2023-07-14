using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeNkpd
    {
        public long Id { get; set; }
        public string? IntNkpdId1 { get; set; }
        public string? IntNkpdId2 { get; set; }
        public string? VcNkpdName { get; set; }
    }
}
