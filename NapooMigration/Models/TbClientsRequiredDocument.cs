using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbClientsRequiredDocument
    {
        public long Id { get; set; }
        public long? IntCodeQualLevelId { get; set; }
        public long? IntCodeEducationId { get; set; }
        public long? IntCodeCourseGroupRequiredDocumentsTypeId { get; set; }
        public long? IntClientId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public string? VcDesciption { get; set; }
        public DateTime? DtDocumentDate { get; set; }
        [Column(TypeName = "oid")]
        public uint? OidFile { get; set; }
        public string? VcDocumentRegNo { get; set; }
        public string? VcDocumentPrnNo { get; set; }
        public DateTime? DtDocumentOfficialDate { get; set; }
        public bool? BoolBeforeDate { get; set; }
        public bool? IsValid { get; set; }
        public int? IntCodeExtRegisterId { get; set; }
    }
}
