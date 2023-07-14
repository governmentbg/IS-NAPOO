using System.ServiceModel;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCourses
{
    public class searchCoursesResponse
    {
        public LoadNAPOOSearchCoursesResponseType param;

        public searchCoursesResponse()
        {
        }

        public searchCoursesResponse(LoadNAPOOSearchCoursesResponseType param)
        {
            this.param = param;
        }
    }
}
