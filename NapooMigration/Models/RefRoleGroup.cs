using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefRoleGroup
    {
        public long Id { get; set; }
        public long? IntRoleId { get; set; }
        public long? IntGroupId { get; set; }
    }
}
