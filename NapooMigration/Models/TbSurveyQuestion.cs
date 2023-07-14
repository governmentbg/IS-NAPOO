using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class TbSurveyQuestion
    {
        public long Id { get; set; }
        public int? IntQuestionType { get; set; }
        public string? VcQuestionText { get; set; }
        public long? IntOrderId { get; set; }
        public bool? BoolQuestionActive { get; set; }
    }
}
