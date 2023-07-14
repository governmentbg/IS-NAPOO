using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeAssignType
    {
        public long Id { get; set; }
        public string? VcAssignTypeName { get; set; }
        public long? IntOrderId { get; set; }
    }
}
