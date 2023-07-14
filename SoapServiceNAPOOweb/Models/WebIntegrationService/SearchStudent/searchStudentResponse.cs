namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchStudent
{
    //[MessageContract(WrapperNamespace = "Napoo", IsWrapped = true)]
    public class searchStudentResponse
    {
        //[MessageHeader(Namespace = "Napoo")]
        public LoadNAPOOSearchStudentResponseType param;

        public searchStudentResponse()
        {
        }

        public searchStudentResponse(LoadNAPOOSearchStudentResponseType param)
        {
            this.param = param;
        }
    }
}
