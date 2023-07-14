using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ReportPremise
    {
        public long? IntYear { get; set; }
        public long? IntProviderId { get; set; }
        public string? VcProviderOwner { get; set; }
        public long? IntProviderOwnershipId { get; set; }
        public int? IntProviderPremiseNo { get; set; }
        public long? IntEkatteId { get; set; }
        public long? IntMunicipalityId { get; set; }
        public long? IntOblId { get; set; }
        public long? IntNutsId { get; set; }
        public long? IntProviderPremiseStatus { get; set; }
        public int? IntSpecialityId { get; set; }
        public long? IntProviderPremiseSpecialityCorrespondence { get; set; }
    }
}
