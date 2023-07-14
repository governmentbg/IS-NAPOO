using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbStartedProcedure
    {
        public long Id { get; set; }
        public long? ProviderId { get; set; }
        public long? ProcedureId { get; set; }
        public DateTime? Ts { get; set; }
        public long? IntCandidateTypeId { get; set; }
        public string? VcProviderOwner { get; set; }
        public string? IntProviderBulstat { get; set; }
        public long? IntEkatteId { get; set; }
        public string? VcProviderPhone1 { get; set; }
        public string? VcProviderPhone2 { get; set; }
        public string? VcProviderEmail { get; set; }
        public string? VcProviderFax { get; set; }
        public string? VcProviderWeb { get; set; }
        public string? VcProviderManager { get; set; }
        public string? VcLegalBookNumber { get; set; }
        public DateTime? DtLegalBookDate { get; set; }
        public string? VcLicenseExpertiseOrderNumber { get; set; }
        public DateTime? DtLicenseExpertiseOrderDate { get; set; }
        public string? VcNegativeOpinionNumber { get; set; }
        public DateTime? DtNegativeOpinionDate { get; set; }
        public string? VcDeniedLicenseOrderNumber { get; set; }
        public DateTime? DtDeniedLicenseOrderDate { get; set; }
        public string? VcIssuedLicenseOrderNumber { get; set; }
        public DateTime? DtIssuedLicenseOrderDate { get; set; }
        public string? VcSummarizedReportNumber { get; set; }
        public DateTime? DtSummarizedReportDate { get; set; }
        public string? VcMeetingProtocolNumber { get; set; }
        public DateTime? DtMeetingProtocolDate { get; set; }
        public string? VcChairmanReportNumber { get; set; }
        public DateTime? DtChairmanReportDate { get; set; }
        public string? VcLicenseExpertiseMailNumber { get; set; }
        public DateTime? DtLicenseExpertiseMailDate { get; set; }
        public string? VcIssuesMailNumber { get; set; }
        public DateTime? DtIssuesMailNumber { get; set; }
        public string? VcLicenseNumber { get; set; }
        public DateTime? DtLicenseDate { get; set; }
        public DateTime? DtNapooReportDeadline { get; set; }
        public DateTime? DtSummarizedReportDeadline { get; set; }
        public DateTime? DtReportReviewDeadline { get; set; }
        public string? VcLicensingMailOutgoingNumber { get; set; }
        public string? VcNegativeIssues { get; set; }
        public DateTime? DtNegativeDeadline { get; set; }
        public string? VcDeniedMailNumber { get; set; }
        public string? VcMeetingHour { get; set; }
        public DateTime? DtMeetingDate { get; set; }
        public string? VcNegativeNeededDocuments { get; set; }
        public string? VcNegativeReasons { get; set; }
        public long? IntReceiveDocumentsId { get; set; }
        public DateTime? DtExpertReportDeadline { get; set; }
    }
}
