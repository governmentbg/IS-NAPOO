using System;

namespace Data.Models.Common
{
    public abstract class AbstractUploadFile
    {
        public abstract string UploadedFileName { get; set; }

        public abstract string MigrationNote { get; set; }

        public abstract int IdModifyUser { get; set; }

        public abstract DateTime ModifyDate { get; set; }
    }
}
