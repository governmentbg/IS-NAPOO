using ISNAPOO.Core.ViewModels.Candidate;
using System.Collections.Generic;

namespace ISNAPOO.Core.ViewModels.Request
{
    public class RequestDocumentManagementControlModel
    {
        public RequestDocumentManagementControlModel()
        {
            this.DocumentSerialNumbers = new List<DocumentSerialNumberVM>();
            this.RequestDocumentTypes = new List<RequestDocumentTypeVM>();
        }

        public int Id { get; set; }

        public int EntityId { get; set; }

        public string CPONameDocumentYearAndDocumentType => $"{this.Provider.ProviderOwner}{this.DocumentYear}{this.TypeOfRequestedDocument.NumberWithName}";

        public int DocumentYear { get; set; }

        public CandidateProviderVM Provider { get; set; }

        public TypeOfRequestedDocumentVM TypeOfRequestedDocument { get; set; }

        public ProviderRequestDocumentVM ProviderRequestDocument { get; set; }

        public List<DocumentSerialNumberVM> DocumentSerialNumbers { get; set; }

        public List<RequestDocumentTypeVM> RequestDocumentTypes { get; set; }

        public int ReceivedCount { get; set; }

        public int HandedOverCount { get; set; }

        public int PrintedCount { get; set; }

        public int CancelledCount { get; set; }

        public int DestroyedCount { get; set; }

        public int AvailableCount { get; set; }
    }
}
