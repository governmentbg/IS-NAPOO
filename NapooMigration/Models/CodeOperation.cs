using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeOperation
    {
        public long Id { get; set; }
        public string? VcName { get; set; }
        public int? IntOrder { get; set; }
    }
}
