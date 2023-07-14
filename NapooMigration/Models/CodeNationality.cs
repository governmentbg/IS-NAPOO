using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeNationality
    {
        public long Id { get; set; }
        public string? VcCountryName { get; set; }
        public bool? BoolIsEuMember { get; set; }
    }
}
