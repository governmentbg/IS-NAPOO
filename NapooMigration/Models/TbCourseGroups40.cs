using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCourseGroups40
    {
        public long Id { get; set; }
        public long? IntClientId { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public long? IntCourseTypeId { get; set; }
        public long? IntCourseFrCurrId { get; set; }
        public long? IntEkatteId { get; set; }
        public long? IntProviderPremise { get; set; }
        public long? IntCourseEdFormId { get; set; }
        public int? IntCourseDuration { get; set; }
        public int? IntSelectableHours { get; set; }
        public int? IntMandatoryHours { get; set; }
        public decimal? NumCourseCost { get; set; }
        public DateTime? DtStartDate { get; set; }
        public DateTime? DtEndDate { get; set; }
        public DateTime? DtExamTheoryDate { get; set; }
        public DateTime? DtExamPracticeDate { get; set; }
        public string? VcExamCommMembers { get; set; }
        public string? VcAdditionalNotes { get; set; }
        public long? IntAssignTypeId { get; set; }
    }
}
