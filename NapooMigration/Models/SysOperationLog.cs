using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class SysOperationLog
    {
        public long Id { get; set; }
        public DateTime? DtDateTime { get; set; }
        public long? IntUserId { get; set; }
        public long? IntOperationId { get; set; }
        public string? VcAdditionalInfo { get; set; }
        public long? IntProviderId { get; set; }
    }
}
