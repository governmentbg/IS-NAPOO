using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace NapooMigration.Models
{
    public partial class TbExpert
    {
        public long Id { get; set; }
        public string? VcExpertFirstName { get; set; }
        public string? VcExpertSecondName { get; set; }
        public string? VcExpertFamilyName { get; set; }
        public long? IntExpertGenderId { get; set; }
        public string? VcEgn { get; set; }
        public long? IntEgnTypeId { get; set; }
        public long? IntNationalityId { get; set; }
        public string? VcTitle { get; set; }
        public DateTime? DtBirthDate { get; set; }
        public DateTime? DtInceptionDate { get; set; }
        public string? VcInceptionOrder { get; set; }
        public long? IntExpertEkatteId { get; set; }
        public string? VcExpertZipcode { get; set; }
        public string? VcExpertAddress { get; set; }
        public string? VcExpertPhone1 { get; set; }
        public string? VcExpertPhone2 { get; set; }
        public string? VcExpertFax { get; set; }
        public string? VcExpertEmail1 { get; set; }
        public string? VcExpertEmail2 { get; set; }
        public long? IntExpertUserId { get; set; }
        [Column(TypeName = "oid")]

        public uint? TxtExpertCvFile { get; set; }
        public long? IntCommMemberId { get; set; }
        public string? VcExpertOccupation { get; set; }
        public string? VcEducation { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
