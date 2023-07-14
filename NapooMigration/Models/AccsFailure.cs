using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class AccsFailure
    {
        public long? T { get; set; }
        public string? Ip { get; set; }
        public long? UserId { get; set; }
        public string? Uname { get; set; }
        public long? Errorcode { get; set; }
    }
}
