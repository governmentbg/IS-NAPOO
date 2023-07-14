using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeVetSpeciality
    {
        public long Id { get; set; }
        public long? IntVetProfessionId { get; set; }
        public long? IntVetAreaId { get; set; }
        public long? IntVetGroupId { get; set; }
        public long? IntVetListId { get; set; }
        public long? IntVetSpecialityNumber { get; set; }
        public string? VcVetSpecialityName { get; set; }
        public long? IntSpecialityVqs { get; set; }
        public bool? BoolIsValid { get; set; }
        public long? IntVetSpecialityCorrection { get; set; }
        public long? IntVetSpecialityCorrectionParent { get; set; }
        public string? VcVetSpecialityCorrectionNotes { get; set; }
        public DateTime? DtStartDateEvent { get; set; }
        public DateTime? DtEndDateEvent { get; set; }
        public string? VcVetSpecialityNameEn { get; set; }

        //public decimal Id { get; set; }
        //public decimal? IntVetProfessionId { get; set; }
        //public decimal? IntVetAreaId { get; set; }
        //public decimal? IntVetGroupId { get; set; }
        //public decimal? IntVetListId { get; set; }
        //public decimal? IntVetSpecialityNumber { get; set; }
        //public string? VcVetSpecialityName { get; set; }
        //public decimal? IntSpecialityVqs { get; set; }
        //public string? BoolIsValid { get; set; }
        //public decimal? IntVetSpecialityCorrection { get; set; }
        //public decimal? IntVetSpecialityCorrectionParent { get; set; }
        //public string? VcVetSpecialityCorrectionNotes { get; set; }
        //public DateTime? DtStartDateEvent { get; set; }
        //public DateTime? DtEndDateEvent { get; set; }
        //public string? VcVetSpecialityNameEn { get; set; }
    }
}
