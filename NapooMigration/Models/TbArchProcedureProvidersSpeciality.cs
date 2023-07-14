using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbArchProcedureProvidersSpeciality
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public string? TxtSpecialityNotes { get; set; }
        public bool? BoolIsAdded { get; set; }
        public string? TxtSpecialityFile { get; set; }
        public bool? BoolIsApproved { get; set; }
    }
}
