using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbESigner
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntUserId { get; set; }
        public string? VcIdNumber { get; set; }
        public string? VcNames { get; set; }
        public string? VcDescription { get; set; }
        public DateTime DtFrom { get; set; }
        public DateTime? DtTo { get; set; }
    }
}
