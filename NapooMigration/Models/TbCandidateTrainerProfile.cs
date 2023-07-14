using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbCandidateTrainerProfile
    {
        public long Id { get; set; }
        public long? IntTrainerId { get; set; }
        public long? IntVetAreaId { get; set; }
        public bool? BoolVetAreaQualified { get; set; }
        public bool? BoolVetAreaTheory { get; set; }
        public bool? BoolVetAreaPractice { get; set; }
        public long? IntVetSpecialityId { get; set; }
    }
}
