using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeProviderRegistration
    {
        public long Id { get; set; }
        public string? VcProviderRegistrationTypeName { get; set; }
    }
}
