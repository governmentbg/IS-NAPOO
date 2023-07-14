using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefArchExpertCommission
    {
        public long Id { get; set; }
        public long? IntArchProviderId { get; set; }
        public long? IntExpertCommissionId { get; set; }
    }
}
