using SoapCore;
using System.Web;
using SoapServiceNAPOOweb.Models.WebIntegrationService.GetCpoLicencedSpecialities;
using SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList;
using SoapServiceNAPOOweb.Models.WebIntegrationService.GetStatistics;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCipo;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCourses;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCpo;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchDocument;
using SoapServiceNAPOOweb.Models.WebIntegrationService.SearchStudent;
using System.ServiceModel;
namespace SoapServiceNAPOOweb.Services
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://is.navet.government.bg/ws/", ConfigurationName = "Data")]
    public interface IWebIntegrationService
{
        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#searchCpoBase", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        searchCpoResponse searchCpo(string username, string password, int licence_status, string keywords, string language);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#getCpoLicencedSpecialitiesTest", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        getCpoLicencedSpecialitiesResponse getCpoLicencedSpecialities(string username, string password, int cpoId, string language);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#searchCipo", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        searchCipoResponse searchCipo(string username, string password, int licence_status, string keywords, string language);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#searchCourses", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        searchCoursesResponse searchCourses(string username, string password, string keywords, string language);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#searchStudent", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        searchStudentResponse searchStudent(string username, string password, string egn, string language);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#searchDocument", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        searchDocumentResponse searchDocument(string username, string password, string egn, string document_id, string language);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#getStatistics", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        getStatisticsResponse getStatistics(string username, string password, int year, int course_type);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IWebIntegrationService#getSPPOOList", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        getSPPOOListResponse getSPPOOList(string username, string password);
    }
}
