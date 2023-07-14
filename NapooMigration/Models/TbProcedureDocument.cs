using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbProcedureDocument
    {
        public long Id { get; set; }
        public long? StartedProcedureId { get; set; }
        [Column(TypeName = "oid")]
        public uint? OidFile { get; set; }
        public bool? IsValid { get; set; }
        public DateTime? Ts { get; set; }
        public long? StageDocumentId { get; set; }
        public long? ProviderId { get; set; }
        public string? MimeType { get; set; }
        public string? Extension { get; set; }
        public string? Filename { get; set; }
        public long? Uin { get; set; }
        public DateTime? DsDate { get; set; }
        public long? DsId { get; set; }
        public string? DsOfficialId { get; set; }
        public DateTime? DsOfficialDate { get; set; }
        public string? DsOfficialNo { get; set; }
        public string? DsPrep { get; set; }
        public long? IntExpertId { get; set; }
    }
}
