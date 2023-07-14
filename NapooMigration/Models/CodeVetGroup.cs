using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeVetGroup
    {
        public long Id { get; set; }
        public long? IntVetListId { get; set; }
        public long? IntVetGroupNumber { get; set; }
        public string? VcVetGroupName { get; set; }
        public bool? BoolIsValid { get; set; }
        public long? IntVetGroupCorrection { get; set; }
        public long? IntVetGroupCorrectionParent { get; set; }
        public string? VcVetGroupCorrectionNotes { get; set; }
        public string? VcVetGroupNameEn { get; set; }

        //public decimal Id { get; set; }
        //public decimal? IntVetListId { get; set; }
        //public decimal? IntVetGroupNumber { get; set; }
        //public string? VcVetGroupName { get; set; }
        //public string? BoolIsValid { get; set; }
        //public decimal? IntVetGroupCorrection { get; set; }
        //public decimal? IntVetGroupCorrectionParent { get; set; }
        //public string? VcVetGroupCorrectionNotes { get; set; }
        //public string? VcVetGroupNameEn { get; set; }
    }
}
