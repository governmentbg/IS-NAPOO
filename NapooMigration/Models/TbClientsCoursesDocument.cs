using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbClientsCoursesDocument
    {
        public long Id { get; set; }
        public long? IntClientsCoursesId { get; set; }
        public long? IntDocumentTypeId { get; set; }
        public int? IntCourseFinishedYear { get; set; }
        public string? VcDocumentPrnNo { get; set; }
        public string? VcDocumentRegNo { get; set; }
        public DateTime? DtDocumentDate { get; set; }
        public string? VcDocumentProt { get; set; }
        public decimal? NumTheoryResult { get; set; }
        public decimal? NumPracticeResult { get; set; }
        public string? VcQualificationName { get; set; }
        public string? VcQualificatiojLevel { get; set; }
        [Column(TypeName = "oid")]
        public uint? Document1File { get; set; }
        [Column(TypeName = "oid")]
        public uint? Document2File { get; set; }
        public string? VcDocumentPrnSer { get; set; }
        public long? IntDocumentStatus { get; set; }
    }
}
