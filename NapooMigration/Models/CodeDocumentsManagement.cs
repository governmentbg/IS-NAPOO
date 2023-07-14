using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeDocumentsManagement
    {
        public long Id { get; set; }
        public string? VcDocumentsManagementName { get; set; }
        public long? IntCandidateTypeId { get; set; }
        public bool? BoolIsBrra { get; set; }
        public bool? BoolIsMandatory { get; set; }
        public bool? BoolIsValid { get; set; }
        public int? OrderId { get; set; }
        public bool? BoolIsNotBrra { get; set; }
        public bool? BoolIsPrnOnly { get; set; }
        public bool? BoolIsConditional { get; set; }
        public string? VcCondition { get; set; }
        public bool? Seenbyexpert { get; set; }
    }
}
