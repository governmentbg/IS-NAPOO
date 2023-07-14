using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbClient
    {
        public long Id { get; set; }
        public string? VcClientFirstName { get; set; }
        public string? VcClientSecondName { get; set; }
        public string? VcClientFamilyName { get; set; }
        public long? IntVetAreaId { get; set; }
        public long? IntEducationId { get; set; }
        public int? IntClientGender { get; set; }
        public long? IntProviderId { get; set; }
        public string? VcEgn { get; set; }
        public long? IntEgnTypeId { get; set; }
        public long? IntNationalityId { get; set; }
        public DateTime? DtClientBirthDate { get; set; }
    }
}
