using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbArchProcedureProvidersCipoManagement
    {
        public long Id { get; set; }
        public long IntStartedProcedureId { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCipoManagementId { get; set; }
        public string? TxtCipoManagementNotes { get; set; }
    }
}
