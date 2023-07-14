using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefExpertsType
    {
        public long Id { get; set; }
        public long? IntExpertId { get; set; }
        public long? IntExpertTypeId { get; set; }
        public string? VcPosition { get; set; }
    }
}
