using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class SysAcl
    {
        public long Id { get; set; }
        public string? VcObjectManager { get; set; }
        public string? VcItemName { get; set; }
        public string? VcAcl { get; set; }
    }
}
