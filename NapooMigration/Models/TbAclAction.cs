using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbAclAction
    {
        public long Id { get; set; }
        public string? VcActionName { get; set; }
    }
}
