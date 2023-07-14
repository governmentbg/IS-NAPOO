using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCourseEdForm
    {
        public long Id { get; set; }
        public string? VcCourseEdFormName { get; set; }
        public bool? BoolIsValid { get; set; }
        public long? IntOrder { get; set; }
    }
}
