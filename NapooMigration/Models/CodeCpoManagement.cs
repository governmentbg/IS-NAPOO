using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeCpoManagement
    {
        public long Id { get; set; }
        public string? VcCodeCpoManagementIdentNew { get; set; }
        public string? VcCodeCpoManagementIdentAdd { get; set; }
        public string? VcCodeCpoManagementName { get; set; }
        public long? Version { get; set; }
        public int? IntUiControlType { get; set; }
        public string? VcPleaseText { get; set; }
        public string? VcListTable { get; set; }
        public string? VcExtraInfo { get; set; }
        public long? IntDocumentsManagementId { get; set; }
        public string? VcCodeCpoManagementIdentP4 { get; set; }
    }
}
