using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefArchProcedureExpertCommission
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntExpertCommissionId { get; set; }
    }
}
