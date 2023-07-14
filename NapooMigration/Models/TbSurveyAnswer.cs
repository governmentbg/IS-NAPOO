using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbSurveyAnswer
    {
        public long Id { get; set; }
        public long? IntQuestionId { get; set; }
        public string? VcAnswerValue { get; set; }
        public long? IntUserId { get; set; }
        public DateTime? DtTimestamp { get; set; }
    }
}
