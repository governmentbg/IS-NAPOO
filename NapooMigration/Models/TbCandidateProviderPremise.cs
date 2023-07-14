using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCandidateProviderPremise
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public int? IntProviderPremiseNo { get; set; }
        public string? TxtProviderPremiseName { get; set; }
        public string? TxtProviderPremiseNotes { get; set; }
        public long? IntProviderPremiseEkatte { get; set; }
        public string? TxtProviderPremiseAddress { get; set; }
        public long? IntProviderPremiseStatus { get; set; }
        public bool? BoolIsVisited { get; set; }
    }
}
