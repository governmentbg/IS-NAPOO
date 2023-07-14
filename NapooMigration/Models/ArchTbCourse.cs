using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ArchTbCourse
    {
        public long Id { get; set; }
        public long? IntCourseNo { get; set; }
        public string? VcCourseName { get; set; }
        public string? VcCourseAddNotes { get; set; }
        public int? IntCourseEducRequirement { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public long? IntCourseTypeId { get; set; }
        public long? IntCourseFrCurrId { get; set; }
        public long? IntProviderId { get; set; }
        public long IntYear { get; set; }
        public int? IntMandatoryHours { get; set; }
        public int? IntSelectableHours { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
