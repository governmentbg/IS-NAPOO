using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCourse40Competence
    {
        public long Id { get; set; }
        public long IntClientId { get; set; }
        public string VcCompetence { get; set; } = null!;
        public bool? IsValid { get; set; }
    }
}
