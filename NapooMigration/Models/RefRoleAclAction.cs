using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefRoleAclAction
    {
        public long Id { get; set; }
        public long? IntRoleId { get; set; }
        public long? IntAclActionId { get; set; }
    }
}
