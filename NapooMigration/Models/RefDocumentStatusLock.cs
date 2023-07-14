using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefDocumentStatusLock
    {
        public long IntDocumentStatusId { get; set; }
        public int IntCodeDocumentStatusLocksId { get; set; }
        public int IntCodeValidityCheckTargetId { get; set; }
        public bool? IsValid { get; set; }
    }
}
