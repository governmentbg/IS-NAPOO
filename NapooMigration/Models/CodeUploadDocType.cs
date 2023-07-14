using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeUploadDocType
    {
        public int Id { get; set; }
        public string? VcDocTypeName { get; set; }
        public bool BoolYearDependent { get; set; }
        public string? VcDocTypeNameShort { get; set; }
        public bool? BoolForCpo { get; set; }
        public bool? BoolValid { get; set; }
    }
}
