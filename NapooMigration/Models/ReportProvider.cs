using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ReportProvider
    {
        public long? IntYear { get; set; }
        public long? IntProviderId { get; set; }
        public string? VcProviderOwner { get; set; }
        public long? IntLicenceNumber { get; set; }
        public string? IntProviderBulstat { get; set; }
        public long? IntEkatteId { get; set; }
        public long? IntMunicipalityId { get; set; }
        public long? IntOblId { get; set; }
        public long? IntNutsId { get; set; }
        public long? IntProviderOwnershipId { get; set; }
        public long? IntNumCoursesA { get; set; }
        public long? IntNumCoursesB { get; set; }
        public long? IntNumClientsA { get; set; }
        public long? IntNumClientsB { get; set; }
        public long? IntNumClientsC { get; set; }
    }
}
