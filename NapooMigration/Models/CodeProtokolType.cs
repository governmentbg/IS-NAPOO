using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeProtokolType
    {
        public int Id { get; set; }
        public string? VcCodeName { get; set; }
        public string? VcName { get; set; }
        public bool? BoolValid { get; set; }
    }
}
