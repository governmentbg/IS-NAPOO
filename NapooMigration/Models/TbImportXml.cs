using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbImportXml
    {
        public long Id { get; set; }
        public string? FileName { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCourseId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public DateTime? FileDate { get; set; }
    }
}
