using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefRequestDocType
    {
        public long Id { get; set; }
        public int? IntProviderId { get; set; }
        public int? IntRequestDocTypeId { get; set; }
        public int? IntRequestId { get; set; }
        public int? IntDocCount { get; set; }
        public int? IntNapooRequestId { get; set; }
    }
}
