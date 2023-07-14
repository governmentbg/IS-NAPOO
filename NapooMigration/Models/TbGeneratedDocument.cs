using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbGeneratedDocument
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCodeProceduresDocumentsId { get; set; }
        public string? OidFile { get; set; }
    }
}
