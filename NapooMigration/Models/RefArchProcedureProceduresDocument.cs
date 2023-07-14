using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefArchProcedureProceduresDocument
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntProceduresDocumentsId { get; set; }
        public string? TxtProceduresDocumentsNotes { get; set; }
        public long? IntExpertId { get; set; }
        public string? TxtProceduresDocumentsFile { get; set; }
    }
}
