using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeRequestDocType
    {
        public long Id { get; set; }
        public string? VcRequestDocTypeOfficialNumber { get; set; }
        public string? VcRequestDocTypeName { get; set; }
        public bool? BoolIsValid { get; set; }
        public int? IntCurrentPeriod { get; set; }
        public float? IntDocPrice { get; set; }
        public int? IntOrderId { get; set; }
        public bool? BoolHasSerialNumber { get; set; }
        public bool IsDestroyable { get; set; }
        public long? IntCodeDocumentTypeId { get; set; }
    }
}
