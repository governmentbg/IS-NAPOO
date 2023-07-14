using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeQualLevel
    {
        public long Id { get; set; }
        public string VcQualLevelName { get; set; } = null!;
        public bool? IsValid { get; set; }
        public int? IntGradeSpk { get; set; }
        public bool? BoolSpkPart { get; set; }
        public bool? BoolSameArea { get; set; }
        public bool? BoolSameProf { get; set; }
    }
}
