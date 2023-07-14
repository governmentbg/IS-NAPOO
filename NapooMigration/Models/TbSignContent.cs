using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbSignContent
    {
        public long Id { get; set; }
        public long IntProviderId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public long? IntClientId { get; set; }
        public long IntUserId { get; set; }
        public string VcContent { get; set; } = null!;
        public string VcSignedContent { get; set; } = null!;
        public long IntDocumentStatus { get; set; }
        public DateTime Dttimestamp { get; set; }
        public string? SignerEgn { get; set; }
        public string? SignerBulstat { get; set; }
        public string? SubjectEmailAddress { get; set; }
        public string? SubjectSerialNumber { get; set; }
        public string? SubjectCn { get; set; }
        public string? SubjectC { get; set; }
        public string? SubjectE { get; set; }
        public string? SubjectOu { get; set; }
        public string? IssuerO { get; set; }
        public string? SerialNumber { get; set; }
        public string? ValidFrom { get; set; }
        public string? ValidTo { get; set; }
        public string? ExtensionsSubjectAltName { get; set; }
    }
}
