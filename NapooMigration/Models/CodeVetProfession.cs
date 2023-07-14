using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeVetProfession
    {
        public long Id { get; set; }
        public long? IntVetAreaId { get; set; }
        public long? IntVetGroupId { get; set; }
        public long? IntVetListId { get; set; }
        public long? IntVetProfessionNumber { get; set; }
        public string? VcVetProfessionName { get; set; }
        public bool? BoolIsValid { get; set; }
        public long? IntVetProfessionCorrection { get; set; }
        public long? IntVetProfessionCorrectionParent { get; set; }
        public string? VcVetProfessionCorrectionNotes { get; set; }
        public string? VcVetProfessionNameEn { get; set; }
        //public decimal Id { get; set; }
        //public decimal? IntVetAreaId { get; set; }
        //public decimal? IntVetGroupId { get; set; }
        //public decimal? IntVetListId { get; set; }
        //public decimal? IntVetProfessionNumber { get; set; }
        //public string? VcVetProfessionName { get; set; }
        //public string? BoolIsValid { get; set; }
        //public decimal? IntVetProfessionCorrection { get; set; }
        //public decimal? IntVetProfessionCorrectionParent { get; set; }
        //public string? VcVetProfessionCorrectionNotes { get; set; }
        //public string? VcVetProfessionNameEn { get; set; }
    }
}
