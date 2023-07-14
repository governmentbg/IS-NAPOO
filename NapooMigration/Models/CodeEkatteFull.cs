using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeEkatteFull
    {
        public long Id { get; set; }
        public long? IntOblId { get; set; }
        public long? IntMunicipalityId { get; set; }
        public long? IntVillageTypeId { get; set; }
        public string? VcKati { get; set; }
        public string? VcName { get; set; }
        public string? VcTextCode { get; set; }
        public string? VcCat { get; set; }
        public string? VcHeight { get; set; }
        public int? IntPostCode { get; set; }
        public string? VcPhoneCode { get; set; }
    }
}
