using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefArchProcedureExpert
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntExpertId { get; set; }
        public long? IntVetArea { get; set; }
    }
}
