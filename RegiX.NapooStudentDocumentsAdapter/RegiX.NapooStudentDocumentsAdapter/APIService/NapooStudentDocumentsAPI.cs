using TechnoLogica.RegiX.Common;
using TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService;
using TechnoLogica.RegiX.Adapters.Common.ExportExtension;
using System.ComponentModel.Composition;
using TechnoLogica.RegiX.Adapters.Common;
using TechnoLogica.RegiX.Common.TransportObjects;

namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService
{
    [ExportFullName(typeof(INapooStudentDocumentsAPI), typeof(IAPIService))]
    [Export(typeof(IAPIService))]
    public class NapooStudentDocumentsAPI : BaseAPIService<INapooStudentDocumentsAPI>, INapooStudentDocumentsAPI
    {
        public ServiceResultDataSigned<DocumentsByStudentRequestType, DocumentsByStudentResponse> GetDocumentsByStudent(ServiceRequestData<DocumentsByStudentRequestType> argument)
        {
            return  AdapterClient.Execute<INapooStudentDocumentsAdapter, DocumentsByStudentRequestType, DocumentsByStudentResponse>(
                (i, r, a, o) => i.GetDocumentsByStudent(r, a, o),
                 argument);
        }

        public ServiceResultDataSigned<StudentDocumentRequestType, DocumentsByStudentResponse> GetStudentDocument(ServiceRequestData<StudentDocumentRequestType> argument)
        {
            return AdapterClient.Execute<INapooStudentDocumentsAdapter, StudentDocumentRequestType, DocumentsByStudentResponse>(
                (i, r, a, o) => i.GetStudentDocument(r, a, o),
                 argument);
        }
    }
}
