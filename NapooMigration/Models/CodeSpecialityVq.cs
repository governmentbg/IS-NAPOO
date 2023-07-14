using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeSpecialityVq
    {
        public int Id { get; set; }
        public string VcVqsName { get; set; } = null!;
        public string? VcVqsShortName { get; set; }
    }
}
