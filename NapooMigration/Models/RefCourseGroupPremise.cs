using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefCourseGroupPremise
    {
        public long Id { get; set; }
        public long IntCourseGroupId { get; set; }
        public long IntProviderPremiseId { get; set; }
        public long? IntTrainingTypeId { get; set; }
        public bool? IsValid { get; set; }
    }
}
