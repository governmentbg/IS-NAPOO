using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbNapooRequestDoc
    {
        public long Id { get; set; }
        public DateTime? DtRequestDate { get; set; }
        public int? IntRequestYear { get; set; }
        [Column(TypeName = "oid")]
        public uint? OidRequestPdf { get; set; }
        public DateTime? Ts { get; set; }
        public bool? BoolIsSent { get; set; }
    }
}
