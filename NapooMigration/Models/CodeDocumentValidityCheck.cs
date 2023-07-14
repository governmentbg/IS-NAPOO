using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeDocumentValidityCheck
    {
        public int Id { get; set; }
        public string? VcDescription { get; set; }
        public string? VcCondition { get; set; }
        public long? IntDocumentTypeId { get; set; }
        public long? IntCourseType { get; set; }
        public bool? BoolMandatory { get; set; }
        public int? IntCodeValidityCheckTarget { get; set; }
        public bool? BoolIfRows0 { get; set; }
        public string? VcInFile { get; set; }
        public string? VcFunctionName { get; set; }
        public bool? IsValid { get; set; }
        public bool? BoolDuplicate { get; set; }
    }
}
