using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCourseType
    {
        public long Id { get; set; }
        public string? VcCourseTypeName { get; set; }
        public string? VcCourseTypeNameEn { get; set; }
        public bool? BoolValid { get; set; }
        public bool? BoolGroup { get; set; }
        public string? VcCourseTypeShort { get; set; }
        public bool? BoolHasFrCurr { get; set; }
        public bool? BoolRdpkCheck { get; set; }
        public bool? BoolHasSpeciality { get; set; }
        public long? IntCfinishedTypeId { get; set; }
    }
}
