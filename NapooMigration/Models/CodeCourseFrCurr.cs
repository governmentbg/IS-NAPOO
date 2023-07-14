using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCourseFrCurr
    {
        public long Id { get; set; }
        public string? VcCourseFrCurrName { get; set; }
        public int? IntOrder { get; set; }
        public string? VcShortDesc { get; set; }
        public long? IntCourseType { get; set; }
        public long? IntCourseValidationType { get; set; }
        public string? VcDescription { get; set; }
        public string? VcDescInEdu { get; set; }
        public string? VcDescInQual { get; set; }
        public string? VcEdForms { get; set; }
        public long? IntDurationMonths { get; set; }
        public long? IntMandatoryHours { get; set; }
        public long? IntSelectableHours { get; set; }
        public long? IntTotalHours { get; set; }
        public long? IntMinPercPractice { get; set; }
        public bool? BoolValid { get; set; }
        public int? IntVqs { get; set; }
        public long? IntMinPercCommon { get; set; }
        public string? VcDescPerc { get; set; }
    }
}
