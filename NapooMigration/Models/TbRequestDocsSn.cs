using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbRequestDocsSn
    {
        public long Id { get; set; }
        public long? IntRequestDocsManagementId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntRequestDocTypeId { get; set; }
        public int? IntReceiveDocsYear { get; set; }
        public string? VcRequestDocNumber { get; set; }
        public int? IntRequestDocsOperationId { get; set; }
        public int? BoolChkFabn { get; set; }
    }
}
