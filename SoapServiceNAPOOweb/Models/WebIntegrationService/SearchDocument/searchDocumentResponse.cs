using System.ServiceModel;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchDocument
{
    public class searchDocumentResponse
    {
        public LoadNAPOOSearchDocumentResponseType param;

        public searchDocumentResponse()
        {
        }

        public searchDocumentResponse(LoadNAPOOSearchDocumentResponseType param)
        {
            this.param = param;
        }
    }
}
