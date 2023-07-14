using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProvidersCpoManagement
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCpoManagementId { get; set; }
        public string? TxtCpoManagementNotes { get; set; }
    }
}
