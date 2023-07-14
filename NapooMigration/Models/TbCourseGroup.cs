using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCourseGroup
    {
        public long Id { get; set; }
        public long? IntCourseId { get; set; }
        public DateTime? DtCourseSubscribeDate { get; set; }
        public string? VcAdditionalNotes { get; set; }
        public long? IntCourseMeasureTypeId { get; set; }
        public long? IntEkatteId { get; set; }
        public string? VcCourseGroupName { get; set; }
        public long? IntCourseStatusId { get; set; }
        public long? IntAssignTypeId { get; set; }
        public long? IntCourseEdFormId { get; set; }
        public string? VcCourseAssignType { get; set; }
        public string? VcCourseNotes { get; set; }
        public int? IntCourseDuration { get; set; }
        public decimal? NumCourseCost { get; set; }
        public DateTime? DtStartDate { get; set; }
        public DateTime? DtEndDate { get; set; }
        public DateTime? DtExamTheoryDate { get; set; }
        public DateTime? DtExamPracticeDate { get; set; }
        public string? VcExamCommMembers { get; set; }
        public int? IntSelectableHours { get; set; }
        public int? IntMandatoryHours { get; set; }
        public long? IntProviderPremise { get; set; }
        public long? IntCourseTypeId { get; set; }
        public long? IntCourseFrCurrId { get; set; }
        public int? IntPDisabilityCount { get; set; }
        public bool? BoolIsArchived { get; set; }
    }
}
