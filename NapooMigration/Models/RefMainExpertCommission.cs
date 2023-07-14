using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefMainExpertCommission
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntExpertCommissionId { get; set; }
    }
}
