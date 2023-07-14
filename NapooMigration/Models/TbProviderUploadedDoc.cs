using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProviderUploadedDoc
    {
        public long Id { get; set; }
        public int? IntUploadDocTypeId { get; set; }
        public long? IntYear { get; set; }
        public string? TxtDocDescription { get; set; }
        public long? IntDocStatusId { get; set; }
        public DateTime? DtDocUploadDate { get; set; }
        public string? TxtFileName { get; set; }
        public string? TxtFileType { get; set; }
        public byte[]? BinFile { get; set; }
        public long? IntProviderId { get; set; }
    }
}
