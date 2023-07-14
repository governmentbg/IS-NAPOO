using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbArchProvider
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public DateTime? DtDecisionDate { get; set; }
        public long? IntProviderNo { get; set; }
        public long? IntCandidateTypeId { get; set; }
        public long? IntProviderStatusId { get; set; }
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
        public long? IntOperationId { get; set; }
        public long? IntLicenceNumber { get; set; }
    }
}
