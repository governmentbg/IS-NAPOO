using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbRequestDocsManagement
    {
        public long Id { get; set; }
        public int? IntRequestId { get; set; }
        public int? IntNapooRequestId { get; set; }
        public long IntProviderId { get; set; }
        public long? IntPartnerId { get; set; }
        public long? IntRequestDocTypeId { get; set; }
        public int? IntReceiveDocsYear { get; set; }
        public int? IntReceiveDocsCount { get; set; }
        public DateTime? DtReceiveDocsDate { get; set; }
        public int? IntRequestDocsOperationId { get; set; }
        public string? VcTbProviderUploadedDocsIds { get; set; }
        public string? VcOrigTbClCourDocs { get; set; }
    }
}
