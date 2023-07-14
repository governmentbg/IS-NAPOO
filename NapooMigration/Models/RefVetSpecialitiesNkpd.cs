using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefVetSpecialitiesNkpd
    {
        public long Id { get; set; }
        public long? IntVetSpecialityId { get; set; }
        public long? IntNkpdId { get; set; }
    }
}
