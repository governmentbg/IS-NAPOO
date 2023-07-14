using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbArchProcedureProvidersDocumentsManagement
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntDocumentsManagementId { get; set; }
        public string? TxtDocumentsManagementTitle { get; set; }
        public string? TxtDocumentsManagementFile { get; set; }
        public DateTime? TsDocument { get; set; }
    }
}
