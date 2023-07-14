using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbArchProcedureProviderSpecialitiesCurriculum
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProcedureProviderSpecialityId { get; set; }
        public bool? BoolIsUpdated { get; set; }
        public DateTime? DtUpdateDate { get; set; }
        public long? IntSpecialityCurriculumUpdateReasonId { get; set; }
        public string? VcFileName { get; set; }
        public string? OidFile { get; set; }
    }
}
