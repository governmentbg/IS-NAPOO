using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefProviderPremisesSpeciality
    {
        public long Id { get; set; }
        public long? IntProviderPremiseId { get; set; }
        public int? IntProviderSpecialityId { get; set; }
        public long? IntProviderPremiseSpecialityUsage { get; set; }
        public long? IntProviderPremiseSpecialityCorrespondence { get; set; }
    }
}
