using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefVisitsExpert
    {
        public long Id { get; set; }
        public long? IntVisitId { get; set; }
        public long? IntExpertId { get; set; }
    }
}
