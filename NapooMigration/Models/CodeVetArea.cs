using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeVetArea
    {
        public long Id { get; set; }
        public long? IntVetGroupId { get; set; }
        public long? IntVetListId { get; set; }
        public long? IntVetAreaNumber { get; set; }
        public string? VcVetAreaName { get; set; }
        public bool? BoolIsValid { get; set; }
        public long? IntVetAreaCorrection { get; set; }
        public long? IntVetAreaCorrectionParent { get; set; }
        public string? VcVetAreaCorrectionNotes { get; set; }
        public string? VcVetAreaNameEn { get; set; }
        //public decimal Id { get; set; }
        //public decimal? IntVetGroupId { get; set; }
        //public decimal? IntVetListId { get; set; }
        //public decimal? IntVetAreaNumber { get; set; }
        //public string? VcVetAreaName { get; set; }
        //public string? BoolIsValid { get; set; }
        //public decimal? IntVetAreaCorrection { get; set; }
        //public decimal? IntVetAreaCorrectionParent { get; set; }
        //public string? VcVetAreaCorrectionNotes { get; set; }
        //public string? VcVetAreaNameEn { get; set; }
    }
}
