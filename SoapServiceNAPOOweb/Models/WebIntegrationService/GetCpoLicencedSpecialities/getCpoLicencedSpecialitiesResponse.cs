using System.ServiceModel;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetCpoLicencedSpecialities
{
    public class getCpoLicencedSpecialitiesResponse
    {
        public NAPOOgetCpoLicencedSpecialitiesResponseType param;

        public getCpoLicencedSpecialitiesResponse()
        {
           this.param = new NAPOOgetCpoLicencedSpecialitiesResponseType();
        }

        public getCpoLicencedSpecialitiesResponse(NAPOOgetCpoLicencedSpecialitiesResponseType param)
        {
            this.param = param;
        }
    }
}
