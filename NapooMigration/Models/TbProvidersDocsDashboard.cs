using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProvidersDocsDashboard
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public long? IntDocTypeId { get; set; }
        public int? IntDocsYear { get; set; }
        public int? IntCntRecv { get; set; }
        public int? IntCntSent { get; set; }
        public int? IntCntPrnt { get; set; }
        public int? IntCntNull { get; set; }
        public int? IntCntDstr { get; set; }
        public int? IntCntAvlb { get; set; }
        public bool? BoolHasSerialNumber { get; set; }
    }
}
