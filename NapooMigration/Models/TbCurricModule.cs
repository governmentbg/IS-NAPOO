using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCurricModule
    {
        public long Id { get; set; }
        public long IntCourseId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public string VcModuleName { get; set; } = null!;
        public long IntHours { get; set; }
        public long IntCurricHoursType { get; set; }
        public long? IntTrainingType { get; set; }
        public bool? IsValid { get; set; }
        public int? IntCurricOrder { get; set; }
    }
}
