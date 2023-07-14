using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeDocumentType
    {
        public long Id { get; set; }
        public string? VcDocumentTypeName { get; set; }
        public string? VcDocumentTypeNameEn { get; set; }
        public bool? BoolMoreThanOne { get; set; }
        public long? IntParentId { get; set; }
        public bool? BoolHasFabNumber { get; set; }
        public bool? BoolHasMarks { get; set; }
        public bool? BoolHasFile { get; set; }
        public bool? BoolHasQual { get; set; }
    }
}
