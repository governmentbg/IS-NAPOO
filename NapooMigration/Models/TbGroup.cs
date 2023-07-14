using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbGroup
    {
        public long Id { get; set; }
        public string? VcShortName { get; set; }
        public string? VcGroupName { get; set; }
        public int? IntGroupType { get; set; }
        public long? Pid { get; set; }
    }
}
