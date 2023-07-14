using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class CodeStageDocument
    {
        public long Id { get; set; }
        public long? RefProcedureStepStageId { get; set; }
        public long? TypeId { get; set; }
        public bool? CanBeMoreThanOne { get; set; }
        public string? Name { get; set; }
        public long? Iorder { get; set; }
        public bool? HasSignedCopy { get; set; }
        public bool? IsValid { get; set; }
        public string? MnemCode { get; set; }
        public int? Parent { get; set; }
        public bool? Uploadbyexpert { get; set; }
        public int? EDelivery { get; set; }
    }
}
