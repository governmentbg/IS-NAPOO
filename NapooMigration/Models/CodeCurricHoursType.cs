using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCurricHoursType
    {
        public long Id { get; set; }
        public string VcCurricHoursType { get; set; } = null!;
        public bool? IsValid { get; set; }
        public long? IntCodeTrainingTypeId { get; set; }
        public string? VcSectionCode { get; set; }
    }
}
