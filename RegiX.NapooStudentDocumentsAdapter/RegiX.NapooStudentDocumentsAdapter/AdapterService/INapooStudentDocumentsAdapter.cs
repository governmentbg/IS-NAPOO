using System.ComponentModel;
using System.ServiceModel;
using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.Common.DataContracts;
using TechnoLogica.RegiX.Common.ObjectMapping;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService
{
    [ServiceContract]
    [Description("Адаптер за комуникация с Национална агенция за професионално образование и обучение -  Регистър на документите по чл. 38 и чл. 40, издадени от центровете за професионално обучени")]
    public interface INapooStudentDocumentsAdapter : IAdapterServiceWCF
    {
        [OperationContract]
        [Description("Справка за издадените документи на лице по подаден идентификатор")]
        CommonSignedResponse<DocumentsByStudentRequestType, DocumentsByStudentResponse> GetDocumentsByStudent(DocumentsByStudentRequestType argument, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters);

        [OperationContract]
        [Description("Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер")]
        CommonSignedResponse<StudentDocumentRequestType, DocumentsByStudentResponse> GetStudentDocument(StudentDocumentRequestType argument, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters);
    }
}

