using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefVisit
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntVisitNo { get; set; }
        public long? IntVisitResultId { get; set; }
        public DateTime? DtVisitDate { get; set; }
        public string? VcVisitTheme { get; set; }
        public string? VcVisitNotes { get; set; }
        public string? VcVisitProtNo { get; set; }
        public DateTime? VcVisitProtDate { get; set; }
    }
}
