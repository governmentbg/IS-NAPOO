using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeProceduresDocument
    {
        public long Id { get; set; }
        public string? VcProceduresDocumentsName { get; set; }
        public long? IntCandidateTypeId { get; set; }
        public int? IntOrderId { get; set; }
        public int? IntDocumentId { get; set; }
        public int? IntRowId { get; set; }
    }
}
