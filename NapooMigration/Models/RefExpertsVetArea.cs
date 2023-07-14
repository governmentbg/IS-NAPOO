using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefExpertsVetArea
    {
        public long Id { get; set; }
        public long? IntExpertId { get; set; }
        public long? IntVetAreaId { get; set; }
    }
}
