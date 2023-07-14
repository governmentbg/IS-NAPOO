using System.ServiceModel;
using System.ComponentModel;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Common.TransportObjects;
using TechnoLogica.RegiX.Adapters.Common.Attributes;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService
{
    [ServiceContract]
    [XmlSerializerFormat]
    [Description("API на Адаптер за комуникация с Национална агенция за професионално образование и обучение -  Регистър на документите по чл. 38 и чл. 40, издадени от центровете за професионално обучени")]
    public interface INapooStudentDocumentsAPI : IAPIService
    {
        //Справка за издадените документи на лице по подаден идентификатор
        [OperationContract]
        [Description("Справка за издадените документи на лице по подаден идентификатор")]
        [Info(
            requestXSD: "DocumentsByStudentRequest.xsd",
            responseXSD: "DocumentsByStudentResponse.xsd",
            commonXSD: "NAPOOCommon.xsd",
            requestXSLT: "DocumentsByStudentRequest.xslt",
            responseXSLT: "DocumentsByStudentResponse.xslt")]
        ServiceResultDataSigned <DocumentsByStudentRequestType, DocumentsByStudentResponse> GetDocumentsByStudent(ServiceRequestData<DocumentsByStudentRequestType> argument);

        //Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер
        [OperationContract]
        [Description("Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер")]
        [Info(
            requestXSD: "StudentDocumentRequest.xsd",
            responseXSD: "DocumentsByStudentResponse.xsd",
            commonXSD: "NAPOOCommon.xsd",
            requestXSLT: "StudentDocumentRequest.xslt",
            responseXSLT: "DocumentsByStudentResponse.xslt")]
        ServiceResultDataSigned<StudentDocumentRequestType, DocumentsByStudentResponse> GetStudentDocument(ServiceRequestData<StudentDocumentRequestType> argument);
    }
}
