using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbProvidersRequestDoc
    {
        public long Id { get; set; }
        public int? IntProviderId { get; set; }
        public int? IntCurrentYear { get; set; }
        public DateTime? DtRequestDoc { get; set; }
        public string? VcPosition { get; set; }
        public string? VcName { get; set; }
        public string? VcAddress { get; set; }
        public string? VcTelephone { get; set; }
        public DateTime? Ts { get; set; }
        [Column(TypeName = "oid")]
        public uint? OidRequestPdf { get; set; }
        public int? IntNapooRequestId { get; set; }
        public bool? BoolIsSent { get; set; }
    }
}
