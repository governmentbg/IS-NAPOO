using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class IscsIisclientdatum
    {
        public string? IisServerName { get; set; }
        public string? IisServerUrl { get; set; }
        public string? IisServerKey { get; set; }
        public int? IisCdata { get; set; }
        public string? IisLastRequest { get; set; }
        public string? IisData { get; set; }
    }
}
