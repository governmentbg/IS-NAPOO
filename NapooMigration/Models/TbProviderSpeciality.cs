using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProviderSpeciality
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public DateTime? DtLicenceData { get; set; }
        public string? IntLicenceProtNo { get; set; }
        public string? TxtSpecialityNotes { get; set; }
        public bool? BoolIsAdded { get; set; }
        public bool? BoolIsValid { get; set; }
        public string? TxtSpecialityFile { get; set; }
    }
}
