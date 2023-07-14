using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefFrCurrEducLevel
    {
        public long Id { get; set; }
        public long IntCodeCourseFrCurrId { get; set; }
        public long IntCodeEducation { get; set; }
        public bool? IsValid { get; set; }
    }
}
