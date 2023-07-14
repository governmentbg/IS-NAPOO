using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefCandidatesExpertCommission
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntExpertCommissionId { get; set; }
    }
}
