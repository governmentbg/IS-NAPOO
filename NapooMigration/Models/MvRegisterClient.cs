using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class MvRegisterClient
    {
        public long? Id { get; set; }
        public long? IntLicenceNumber { get; set; }
        public string? VcProviderName { get; set; }
        public string? VcClientName { get; set; }
        public string? VcEgn { get; set; }
        public string? VcEgnType { get; set; }
        public long? IntCourseTypeId { get; set; }
        public string? VcCourseTypeName { get; set; }
        public long? IntVetAreaNumber { get; set; }
        public string? VcVetAreaName { get; set; }
        public long? IntVetProfessionNumber { get; set; }
        public string? VcVetProfessionName { get; set; }
        public long? IntVetSpecialityNumber { get; set; }
        public string? VcVetSpecialityName { get; set; }
        public long? IntSpecialityVqs { get; set; }
        public string? VcSpecialityVqs { get; set; }
        public int? IntCourseFinishedYear { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntClientsCoursesId { get; set; }
        public long? DocumentId { get; set; }
        public long? IntOblId { get; set; }
        public string? VcOblName { get; set; }
        public long? IntMunicipalityId { get; set; }
        public string? VcMunicipalityName { get; set; }
        public long? IntEkkateId { get; set; }
        public string? VcEkkateName { get; set; }
    }
}
