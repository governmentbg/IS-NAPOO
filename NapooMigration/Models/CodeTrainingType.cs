using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeTrainingType
    {
        public long Id { get; set; }
        public string VcTrainingTypeName { get; set; } = null!;
        public bool? IsValid { get; set; }
        public bool? BoolGroupMtb { get; set; }
        public bool? BoolGroupTrainer { get; set; }
        public string? VcSection { get; set; }
    }
}
