using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefExpertsCommission
    {
        public long Id { get; set; }
        public long? IntExpertId { get; set; }
        public long? IntExpCommId { get; set; }
        public bool? BoolIsChairman { get; set; }
        public long? IntCommissionInstitutionType { get; set; }
        public string? VcExpertInstitution { get; set; }
        public string? VcExpertOccupation { get; set; }
        public string? VcExpertProtokol { get; set; }
        public DateTime? DtExpertProtokolDate { get; set; }
    }
}
