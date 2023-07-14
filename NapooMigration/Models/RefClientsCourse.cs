using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefClientsCourse
    {
        public long Id { get; set; }
        public long? IntClientId { get; set; }
        public long? IntCourseGroupId { get; set; }
        public string? VcFirstName { get; set; }
        public string? VcSecondName { get; set; }
        public string? VcFamilyName { get; set; }
        public long? IntNationalityId { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public long? IntCfinishedTypeId { get; set; }
        public DateTime? DtCourseFinished { get; set; }
        public long? IntAssignTypeId { get; set; }
        public int? IntClientGender { get; set; }
        public string? VcEgn { get; set; }
        public int? IntEgnTypeId { get; set; }
        public DateTime? DtClientBirthDate { get; set; }
        public long? IntEducationId { get; set; }
        public long? IntVetAreaId { get; set; }
        public long? IntQualLevel { get; set; }
        public long? IntQualVetArea { get; set; }
    }
}
