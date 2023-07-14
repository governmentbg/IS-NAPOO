using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbCandidateProvidersDocumentsManagement
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntDocumentsManagementId { get; set; }
        public string? TxtDocumentsManagementTitle { get; set; }

        [Column(TypeName = "oid")]
        public uint? TxtDocumentsManagementFile { get; set; }
        public DateTime? TsDocument { get; set; }
    }
}
