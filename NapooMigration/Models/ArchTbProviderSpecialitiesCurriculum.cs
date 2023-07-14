using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ArchTbProviderSpecialitiesCurriculum
    {
        public long Id { get; set; }
        public long? IntProviderSpecialityId { get; set; }
        public bool? BoolIsUpdated { get; set; }
        public DateTime? DtUpdateDate { get; set; }
        public long? IntSpecialityCurriculumUpdateReasonId { get; set; }
        public string? VcFileName { get; set; }
        public long IntYear { get; set; }
    }
}
