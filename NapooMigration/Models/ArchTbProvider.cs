using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ArchTbProvider
    {
        public long Id { get; set; }
        public long? IntProviderStatusId { get; set; }
        public long? IntLicenceStatusId { get; set; }
        public long? IntLicenceNumber { get; set; }
        public string? VcProviderOwner { get; set; }
        public string? IntProviderBulstat { get; set; }
        public long? IntLocalGroupId { get; set; }
        public long? IntEkatteId { get; set; }
        public string? VcZipCode { get; set; }
        public string? VcProviderAddress { get; set; }
        public string? VcProviderPhone1 { get; set; }
        public string? VcProviderPhone2 { get; set; }
        public string? VcProviderFax { get; set; }
        public string? VcProviderWeb { get; set; }
        public string? VcProviderEmail { get; set; }
        public string? VcProviderManager { get; set; }
        public string? VcProviderContactPers { get; set; }
        public long? IntProviderContactPersEkatteId { get; set; }
        public string? VcProviderContactPersZipcode { get; set; }
        public string? VcProviderContactPersAddress { get; set; }
        public string? VcProviderContactPersPhone1 { get; set; }
        public string? VcProviderContactPersPhone2 { get; set; }
        public string? VcProviderContactPersFax { get; set; }
        public string? VcProviderContactPersEmail { get; set; }
        public long? IntProviderOwnershipId { get; set; }
        public bool? BoolIsBrra { get; set; }
        public string? VcProviderProfile { get; set; }
        public long IntYear { get; set; }
        public string? VcProviderName { get; set; }
        public DateTime? DtLicenceData { get; set; }
        public long? IntProviderRegistrationId { get; set; }
    }
}
