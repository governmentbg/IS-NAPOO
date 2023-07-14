using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeRequestDocsSeries
    {
        public long Id { get; set; }
        public long? IntDocType { get; set; }
        public int? IntDocYear { get; set; }
        public string? VcSeriesName { get; set; }
        public string? VcRequestDocTypeOfficialNumber { get; set; }
    }
}
