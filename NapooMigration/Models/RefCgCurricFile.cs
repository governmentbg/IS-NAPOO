using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefCgCurricFile
    {
        public long Id { get; set; }
        public long IntCourseId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public long IntProviderSpecialitiesCurriculumId { get; set; }
        public bool? IsValid { get; set; }
    }
}
