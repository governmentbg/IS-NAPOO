using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbArchProcedureTrainerDocument
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntTrainerId { get; set; }
        public string? TxtDocumentsManagementTitle { get; set; }
        public string? TxtDocumentsManagementFile { get; set; }
    }
}
