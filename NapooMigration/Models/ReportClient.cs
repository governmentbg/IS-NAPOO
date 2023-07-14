using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ReportClient
    {
        public long? IntYear { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public string? VcEgn { get; set; }
        public long? IntEgnTypeId { get; set; }
        public int? IntClientGender { get; set; }
        public int? IntBirthYear { get; set; }
        public long? IntNationalityId { get; set; }
        public long? IntCourseTypeId { get; set; }
        public long? IntCourseFrCurrId { get; set; }
        public long? IntVetAreaId { get; set; }
        public long? IntVetGroupId { get; set; }
        public long? IntVetProfessionId { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public long? IntVetQualificationLevel { get; set; }
        public long? IntEkatteId { get; set; }
        public long? IntMunicipalityId { get; set; }
        public long? IntOblId { get; set; }
        public long? IntNutsId { get; set; }
        public long? IntCourseMeasureTypeId { get; set; }
        public long? IntAssignTypeId { get; set; }
        public long? IntCourseEdFormId { get; set; }
        public int? IntCourseDuration { get; set; }
        public decimal? NumCourseCost { get; set; }
        public long? IntCfinishedTypeId { get; set; }
        public int? IntClientStatus { get; set; }
    }
}
