﻿using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class ArchTbTrainerQualification
    {
        public long Id { get; set; }
        public long? IntTrainerId { get; set; }
        public string? TxtQualificationName { get; set; }
        public long? IntQualificationTypeId { get; set; }
        public long? IntProfessionId { get; set; }
        public long? IntTqualificationTypeId { get; set; }
        public int? IntQualificationDuration { get; set; }
        public DateTime? DtStartDate { get; set; }
        public long IntYear { get; set; }
    }
}
