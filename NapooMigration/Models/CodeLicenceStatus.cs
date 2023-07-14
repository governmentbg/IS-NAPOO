using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeLicenceStatus
    {
        public long Id { get; set; }
        public string? VcLicenceStatusName { get; set; }
        public string? VcLicenceStatusNameEn { get; set; }
        public string? VcLicStatusShortName { get; set; }
        public string? VcLicStatusShortNameEn { get; set; }
    }
}
