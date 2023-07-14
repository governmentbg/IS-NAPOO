using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefClientsCoursesDocumentsStatus
    {
        public long Id { get; set; }
        public long IntProviderId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public long? IntCourseGroup40Id { get; set; }
        public long IntClientCoursesDocumentsId { get; set; }
        public long IntDocumentStatus { get; set; }
        public string? VcNote { get; set; }
        public long IntUserId { get; set; }
        public DateTime? Dt { get; set; }
        public long? IntSignContentId { get; set; }
        public bool? IsValid { get; set; }
        public long? IntClientId { get; set; }
    }
}
