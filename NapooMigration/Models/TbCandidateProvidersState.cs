using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCandidateProvidersState
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntCandidateProvidersStateId { get; set; }
        public DateTime? DtCandidateProvidersStateChange { get; set; }
        public long? IntUserId { get; set; }
    }
}
