using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbProviderPremisesRoom
    {
        public long Id { get; set; }
        public long? IntPremiseId { get; set; }
        public int? IntProviderPremiseRoomNo { get; set; }
        public string? TxtProviderPremiseRoomName { get; set; }
        public long? IntProviderPremiseRoomUsage { get; set; }
        public long? IntProviderPremiseRoomType { get; set; }
        public int? IntProviderPremiseRoomArea { get; set; }
        public int? IntProviderPremiseRoomWorkplaces { get; set; }
        public string? TxtProviderPremiseRoomEquipment { get; set; }
    }
}
