namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCpo
{
    public class searchCpoResponse
    { 
        public LoadNAPOOSearchResponseType param { get; set; }

        public searchCpoResponse()
        {
            this.param = new LoadNAPOOSearchResponseType();
        }

        public searchCpoResponse(LoadNAPOOSearchResponseType param)
        {
            this.param = param;
        }

    }
}
