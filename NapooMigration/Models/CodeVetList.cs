using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeVetList
    {
        public long Id { get; set; }
        public string? VcVetListName { get; set; }
        public bool? BoolIsValid { get; set; }
    }
}
