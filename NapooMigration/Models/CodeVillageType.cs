using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeVillageType
    {
        public long Id { get; set; }
        public string? VcVillageTypeName { get; set; }
        public string? VcVillageTypeShortName { get; set; }
    }
}
