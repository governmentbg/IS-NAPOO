using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefArchProceduresDocument
    {
        public long Id { get; set; }
        public long? IntArchProviderId { get; set; }
        public long? IntProceduresDocumentsId { get; set; }
        public string? TxtProceduresDocumentsNotes { get; set; }
        public long? IntExpertId { get; set; }
        public string? TxtProceduresDocumentsFile { get; set; }
    }
}
