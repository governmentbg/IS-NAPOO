using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProcedureProgressDocument
    {
        public long Id { get; set; }
        public long? IntProcedureProgressId { get; set; }
        public string? VcDocumentName { get; set; }
        public string? VcDocumentFile { get; set; }
    }
}
