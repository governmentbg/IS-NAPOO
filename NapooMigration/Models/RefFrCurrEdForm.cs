﻿using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefFrCurrEdForm
    {
        public long Id { get; set; }
        public long IntCodeCourseFrCurrId { get; set; }
        public long IntCodeCourseEdForm { get; set; }
        public bool? IsValid { get; set; }
    }
}
