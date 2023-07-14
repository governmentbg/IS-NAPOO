using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCommissionInstitutionType
    {
        public long Id { get; set; }
        public string? VcCommissionInstitutionTypeName { get; set; }
        public string? VcShortName { get; set; }
    }
}
