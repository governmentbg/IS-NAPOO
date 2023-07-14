using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbDoi
    {
        public long Id { get; set; }
        public long? IntDoiId { get; set; }
        public string? VcDoiRegualtion { get; set; }
        public string? VcDoiGeneralDescr { get; set; }
        public string? VcDoiJobProfile { get; set; }
        public string? VcDoiEducObjectives { get; set; }
        public string? VcDoiLrngOutcomes { get; set; }
        public string? VcDoiMtbUpdates { get; set; }
        public string? VcDoiPdfPath { get; set; }
        public bool? BSubmitted { get; set; }
        public DateTime? DtSubmittedDate { get; set; }
        public string? VcComment { get; set; }
        public string? VcDoiCommission { get; set; }
        public string? VcDoiCertificateSupplement { get; set; }
        public string? VcDoiJobCareer { get; set; }
        public string? VcSearch { get; set; }
    }
}
