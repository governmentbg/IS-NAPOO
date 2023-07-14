using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ArchTbClientsCoursesDocument
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
        public string? Document1File { get; set; }
        public string? Document2File { get; set; }
        public long IntYear { get; set; }
        public string? VcDocumentPrnSer { get; set; }
        public long? IntDocumentStatus { get; set; }
    }
}
