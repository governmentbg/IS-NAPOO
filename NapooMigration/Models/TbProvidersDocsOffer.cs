using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProvidersDocsOffer
    {
        public long Id { get; set; }
        public long? IntProviderId { get; set; }
        public int? IntSeekOffer { get; set; }
        public long? IntDocTypeId { get; set; }
        public int? IntCountOffered { get; set; }
        public DateTime? DtOffered { get; set; }
        public DateTime? DtClosed { get; set; }
        public bool? BoolOfferValid { get; set; }
    }
}
