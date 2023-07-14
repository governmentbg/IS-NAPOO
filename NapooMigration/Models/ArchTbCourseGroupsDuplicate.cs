using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ArchTbCourseGroupsDuplicate
    {
        public long Id { get; set; }
        public long? IntClientId { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public long IntCodeDocumentTypeId { get; set; }
        public DateTime? DtStartDate { get; set; }
        public DateTime? DtEndDate { get; set; }
        public int? IntCourseFinishedYear { get; set; }
        public string? VcOriginalPrnNo { get; set; }
        public string? VcOriginalRegNo { get; set; }
        public DateTime? DtOriginalDate { get; set; }
        public long? IntOriginalRefClientId { get; set; }
        public long? IntOriginalDocumentId { get; set; }
        public long IntYear { get; set; }
    }
}
