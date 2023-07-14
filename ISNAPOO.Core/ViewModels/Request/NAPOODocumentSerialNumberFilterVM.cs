using System;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class NAPOODocumentSerialNumberFilterVM
    {
        public string SeriesName { get; set; }

        public string SerialNumber { get; set; }

        public int IdTypeOfRequestedDocument { get; set; }

        public int? DocumentYear { get; set; }

        public DateTime? DocumentDate { get; set; }

        public int IdCandidateProvider { get; set; }

        public int IdDocumentOperation { get; set; }
    }
}
