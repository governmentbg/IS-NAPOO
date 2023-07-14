using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProviderActivity
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public int? IntCurrentYear { get; set; }
        public string? TxtProviderActivities { get; set; }
    }
}
