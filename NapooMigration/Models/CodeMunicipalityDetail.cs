using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeMunicipalityDetail
    {
        public long Id { get; set; }
        public long? IntMunicipalityId { get; set; }
        public long? IntOblId { get; set; }
        public string? VcMunicipalityDetailsName { get; set; }
    }
}
