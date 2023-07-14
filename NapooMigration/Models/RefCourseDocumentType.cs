using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefCourseDocumentType
    {
        public long Id { get; set; }
        public long? IntCodeCourseType { get; set; }
        public long? IntCodeDocumentType { get; set; }
    }
}
