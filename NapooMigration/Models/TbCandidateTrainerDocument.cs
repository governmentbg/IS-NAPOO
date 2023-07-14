using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbCandidateTrainerDocument
    {
        public long Id { get; set; }
        public long? IntTrainerId { get; set; }
        public string? TxtDocumentsManagementTitle { get; set; }
        [Column(TypeName = "oid")]
        public uint? TxtDocumentsManagementFile { get; set; }
    }
}
