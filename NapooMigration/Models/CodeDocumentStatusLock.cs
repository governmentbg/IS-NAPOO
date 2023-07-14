using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeDocumentStatusLock
    {
        public int Id { get; set; }
        public string VcStatusLockName { get; set; } = null!;
    }
}
