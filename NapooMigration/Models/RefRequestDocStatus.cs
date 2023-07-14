using System;
using System.Collections.Generic;

namespace NapooMigration.Models
{
    public partial class RefRequestDocStatus
    {
        public long Id { get; set; }
        public int? IntProviderId { get; set; }
        public int? IntRequestDocStatusId { get; set; }
        public int? IntRequestId { get; set; }
        public DateTime? Ts { get; set; }
    }
}
