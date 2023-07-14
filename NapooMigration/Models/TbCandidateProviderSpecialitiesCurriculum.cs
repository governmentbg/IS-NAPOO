using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbCandidateProviderSpecialitiesCurriculum
    {
        public long Id { get; set; }
        public long? IntCandidateProviderSpecialityId { get; set; }
        public bool? BoolIsUpdated { get; set; }
        public DateTime? DtUpdateDate { get; set; }
        public long? IntSpecialityCurriculumUpdateReasonId { get; set; }
        public string? VcFileName { get; set; }
        [Column(TypeName = "oid")]
        public uint? OidFile { get; set; }
    }
}
