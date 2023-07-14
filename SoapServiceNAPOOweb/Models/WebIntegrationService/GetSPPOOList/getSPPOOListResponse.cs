using System.ServiceModel;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList
{
    public class getSPPOOListResponse
    {
        public LoadSPPOOResponseType param;

        public getSPPOOListResponse()
        {
        }

        public getSPPOOListResponse(LoadSPPOOResponseType param)
        {
            this.param = param;
        }
    }
}
