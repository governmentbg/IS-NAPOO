using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefExpertsDoi
    {
        public long Id { get; set; }
        public long? IntExpertId { get; set; }
        public long? IntDoiCommId { get; set; }
        public long? IntWgdoiFunctionId { get; set; }
    }
}
