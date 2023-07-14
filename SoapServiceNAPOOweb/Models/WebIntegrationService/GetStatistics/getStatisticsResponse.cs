using System.ServiceModel;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetStatistics
{
    public class getStatisticsResponse
    {
        public LoadNAPOOStatisticsResponseType param;

        public getStatisticsResponse()
        {
        }

        public getStatisticsResponse(LoadNAPOOStatisticsResponseType param)
        {
            this.param = param;
        }
    }
}
