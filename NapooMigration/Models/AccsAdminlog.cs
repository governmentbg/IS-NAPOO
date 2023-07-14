using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class AccsAdminlog
    {
        public long AlId { get; set; }
        public long? T { get; set; }
        public long? Sessionid { get; set; }
        public long? TUserid { get; set; }
        public long? Action { get; set; }
        public string? Adata { get; set; }
    }
}
