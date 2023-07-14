using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbUser
    {
        public long Id { get; set; }
        public string? VcFullname { get; set; }
        public long? IntGlobalGroupId { get; set; }
        public long? IntLocalGroupId { get; set; }
        public string? Upwd { get; set; }
        public string? Unhs { get; set; }
        public string? Udat { get; set; }
        public string? Adat { get; set; }
    }
}
