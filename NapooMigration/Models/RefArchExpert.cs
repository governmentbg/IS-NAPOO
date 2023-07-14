using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefArchExpert
    {
        public long Id { get; set; }
        public long? IntArchProviderId { get; set; }
        public long? IntExpertId { get; set; }
    }
}
