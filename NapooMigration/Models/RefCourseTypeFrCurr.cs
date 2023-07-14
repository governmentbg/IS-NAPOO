using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefCourseTypeFrCurr
    {
        public long IntCodeCourseTypeId { get; set; }
        public long IntCodeCourseFrCurrId { get; set; }
        public bool? BoolValid { get; set; }
    }
}
