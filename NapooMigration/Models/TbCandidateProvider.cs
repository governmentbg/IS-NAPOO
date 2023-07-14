using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCandidateProvider
    {
        public long Id { get; set; }
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
        public bool? BoolIsBrra { get; set; }
        public string? VcProviderProfile { get; set; }
        public string? VcFilingSystemNumber { get; set; }
        public DateTime? DtFilingSystemDate { get; set; }
        public DateTime? DtLicenceData { get; set; }
        public int? IntLicenceProtNo { get; set; }
        public string? VcProviderName { get; set; }
        public long? IntStartedProcedures { get; set; }
        public long? IntStartedProcedureProgress { get; set; }
        public long? ProcedureId { get; set; }
        public long? StepId { get; set; }
        public long? StageId { get; set; }
        public bool? IsReturned { get; set; }
        public long? IntReceiveTypeId { get; set; }
        public long? IntProviderRegistrationId { get; set; }
        public long? IntCodeCpoManagementVersion { get; set; }
    }
}
