using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCipoManagement
    {
        public long Id { get; set; }
        public string? VcCodeCipoManagementIdentNew { get; set; }
        public string? VcCodeCipoManagementIdentAdd { get; set; }
        public string? VcCodeCipoManagementName { get; set; }
        public long? Version { get; set; }
        public int? IntUiControlType { get; set; }
        public string? VcPleaseText { get; set; }
        public string? VcListTable { get; set; }
        public string? VcExtraInfo { get; set; }
        public long? IntDocumentsManagementId { get; set; }
    }
}
