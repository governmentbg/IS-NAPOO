using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefRoleUser
    {
        public long Id { get; set; }
        public long? IntRoleId { get; set; }
        public long? IntUserId { get; set; }
    }
}
