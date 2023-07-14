using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class SysSignLog
    {
        public long Id { get; set; }
        public long? IntUserId { get; set; }
        public string? VcIdNumber { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCourseGroup { get; set; }
        public long? IntClientId { get; set; }
        public string? VcCertValidFrom { get; set; }
        public string? VcCertValidTo { get; set; }
        public string? VcCertEmail { get; set; }
        public string? VcError { get; set; }
        public DateTime DtEvent { get; set; }
        public string? VcCert { get; set; }
    }
}
