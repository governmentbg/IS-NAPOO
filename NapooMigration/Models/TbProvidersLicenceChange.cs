using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProvidersLicenceChange
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntLicenceStatusId { get; set; }
        public DateTime? DtChangeDate { get; set; }
        public string? VcNumberCommand { get; set; }
        public long? IntLicenceStatusDetailsId { get; set; }
        public string? VcNotes { get; set; }
        public long? UserId { get; set; }
        public DateTime? DtInsertDate { get; set; }
    }
}
