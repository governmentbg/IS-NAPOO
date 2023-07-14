using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeDocumentStatus
    {
        public long Id { get; set; }
        public string VcStatusName { get; set; } = null!;
        public bool? IsValid { get; set; }
        public string? VcButtonName { get; set; }
        public bool? BoolWithNote { get; set; }
        public bool? BoolLockCg { get; set; }
        public bool? BoolLockClient { get; set; }
        public bool? BoolLockClientDocs { get; set; }
        public bool? BoolLockRollback { get; set; }
        public bool? BoolLockAddProtokols { get; set; }
        public bool? BoolLockAddClients { get; set; }
        public bool? BoolLockAddDocument { get; set; }
        public bool? BoolLockProtokols { get; set; }
    }
}
