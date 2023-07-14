using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCourseGroupRequiredDocumentsType
    {
        public long Id { get; set; }
        public string VcDocumentType { get; set; } = null!;
        public bool? BoolForClient { get; set; }
        public bool? BoolMandatory { get; set; }
        public bool? IsValid { get; set; }
        public long? IntCourseType { get; set; }
        public string? VcCheckboxName { get; set; }
        public int? CheckboxOrder { get; set; }
    }
}
