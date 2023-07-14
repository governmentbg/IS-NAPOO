using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeMunicipality
    {
        public long Id { get; set; }
        public long? IntOblId { get; set; }
        public string? VcMunicipalityName { get; set; }
        public string? VcMunicipalityCodeName { get; set; }
    }
}
