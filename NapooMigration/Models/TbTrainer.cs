using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbTrainer
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public string? VcEgn { get; set; }
        public long? IntEgnTypeId { get; set; }
        public string? VcTrainerFirstName { get; set; }
        public string? VcTrainerSecondName { get; set; }
        public string? VcTrainerFamilyName { get; set; }
        public int? IntBirthYear { get; set; }
        public long? IntGenderId { get; set; }
        public long? IntNationalityId { get; set; }
        public long? IntEducationId { get; set; }
        public string? TxtEducationSpecialityNotes { get; set; }
        public string? TxtEducationCertificateNotes { get; set; }
        public string? TxtEducationAcademicNotes { get; set; }
        public bool? BoolIsAndragog { get; set; }
        public long? IntTcontractTypeId { get; set; }
        public DateTime? DtTcontractDate { get; set; }
        public string? VcEmail { get; set; }
    }
}
