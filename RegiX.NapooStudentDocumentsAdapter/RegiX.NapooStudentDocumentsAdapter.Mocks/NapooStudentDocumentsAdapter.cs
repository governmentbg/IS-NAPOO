//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TechnoLogica.RegiX.Common;
//using TechnoLogica.RegiX.Common.ObjectMapping;
//using TechnoLogica.RegiX.Common.AdapterCore;
//using TechnoLogica.RegiX.Common.Utils;
//using System.Data;
//using System.ComponentModel.Composition;
//using System.Net;
//using TechnoLogica.RegiX.Common.DataContracts;
//using TechnoLogica.RegiX.Common.ServiceContracts;
//using TechnoLogica.RegiX.Common.TransportObject;
//using TechnoLogica.RegiX.WebServiceAdapterService;

//namespace TechnoLogica.RegiX.NapooStudentDocumentsAdapter.AdapterService
//{
//    public class NapooStudentDocumentsAdapter : SoapServiceBaseAdapterService, INapooStudentDocumentsAdapter, IAdapterService
//    {
//        [Export(typeof(ParameterInfo))]
//        [ExportFullName(typeof(NapooStudentDocumentsAdapter), typeof(ParameterInfo))]
//        public static ParameterInfo<string> WebServiceUrl =
//           new ParameterInfo<string>("http://is.navet.government.bg/ws/egov/index.php")
//           //new ParameterInfo<string>("http://localhost:63823/ServiceRefForMethod2Service.svc")
//           {
//               Key = Constants.WebServiceUrlParameterKeyName,
//               Description = "Web Service Url",
//               OwnerAssembly = typeof(NapooStudentDocumentsAdapter).Assembly
//           };

//        [Export(typeof(ParameterInfo))]
//        [ExportFullName(typeof(NapooStudentDocumentsAdapter), typeof(ParameterInfo))]
//        public static ParameterInfo<string> WebServiceUrlForGetStudentDocument =
//            new ParameterInfo<string>("http://is.navet.government.bg/ws/egov/index.php")
//          // new ParameterInfo<string>("http://localhost:63823/NapooStudentDocumentsService.svc")
//           {
//               Key = "WebServiceUrlForGetStudentDocument",
//               Description = "Web Service Url for GetStudentDocument, should be same as WebServiceUrl. For mockup it's different!",
//               OwnerAssembly = typeof(NapooStudentDocumentsAdapter).Assembly
//           };

//        public CommonSignedResponse<DocumentsByStudentRequestType, DocumentsByStudentResponse> GetDocumentsByStudent(DocumentsByStudentRequestType argument, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters)
//        {
//            Guid id = Guid.NewGuid();
//            LogData(aditionalParameters, new { Request = argument.Serialize(), Guid = id });
//            try
//            {
//                ServiceRefForMethod2.DataClient serviceClient = new ServiceRefForMethod2.DataClient("DataWSDLPort1", WebServiceUrl.Value); // ServiceRefForMethod2ServiceMockupUrl.Value
//                ServiceRefForMethod2.DocumentsByStudentRequestType arg = new ServiceRefForMethod2.DocumentsByStudentRequestType();
//                arg.identifier = argument.StudentIdentifier;
//                ServiceRefForMethod2.DocumentsByStudentResponseType serviceResult = serviceClient.egovSearchStudentDocumentByStudent(arg);
//                ObjectMapper<ServiceRefForMethod2.DocumentsByStudentResponseType, DocumentsByStudentResponse> mapper = CreateDocumentsByStudentMapper(accessMatrix);
//                DocumentsByStudentResponse searchResults = new DocumentsByStudentResponse();
//                mapper.Map(serviceResult, searchResults);
//                return
//                    SigningUtils.CreateAndSign(
//                        argument,
//                        searchResults,
//                        accessMatrix,
//                        aditionalParameters
//                    );
//            }
//            catch (Exception ex)
//            {
//                LogError(aditionalParameters, ex, new { Guid = id });
//                throw ex;
//            }
//        }

//        private static ObjectMapper<ServiceRefForMethod2.DocumentsByStudentResponseType, DocumentsByStudentResponse> CreateDocumentsByStudentMapper(AccessMatrix accessMatrix)
//        {
//            ObjectMapper<ServiceRefForMethod2.DocumentsByStudentResponseType, DocumentsByStudentResponse> mapper = new ObjectMapper<ServiceRefForMethod2.DocumentsByStudentResponseType, DocumentsByStudentResponse>(accessMatrix);

//            mapper.AddCollectionMap<ServiceRefForMethod2.DocumentsByStudentResponseType>((o) => o.StudentDocument, (c) => c.data);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentIssueDate, (c) => c.data[0].document_issue_date);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentRegistrationNumber, c => c.data[0].document_reg_no);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentSerialNumber, (c) => c.data[0].document_prn_no);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentSeries, (c) => c.data[0].document_prn_ser);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentType, (c) => c.data[0].document_type_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].EducationType, (c) => c.data[0].course_type_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].FirstName, (c) => c.data[0].first_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].MiddleName, (c) => c.data[0].second_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].LastName, (c) => c.data[0].family_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].GraduationYear, (c) => c.data[0].year_finished);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ProfessionalEduCenterLocation, (c) => c.data[0].city_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ProfessionalEduCenter, (c) => c.data[0].provider_owner);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].QualificationDegree, (c) => c.data[0].speciality_vqs);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ProfessionCodeAndName, (c) => c.data[0].profession_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].StudentIdentifier, (c) => c.data[0].vc_egn);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].SubjectCodeAndName, (c) => c.data[0].speciality_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ClientID, (c) => c.data[0].client_id);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].LicenceNumber, (c) => c.data[0].licence_number);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentTypeID, (c) => c.data[0].document_type_id);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].EducationTypeID, (c) => c.data[0].course_type_id);
//            mapper.AddPropertyMap((o) => o.Status, (c) => c.status);
//            mapper.AddPropertyMap((o) => o.Message, (c) => c.message);
//            return mapper;
//        }

//        public CommonSignedResponse<StudentDocumentRequestType, DocumentsByStudentResponse> GetStudentDocument(StudentDocumentRequestType argument, AccessMatrix accessMatrix, AdapterAdditionalParameters aditionalParameters)
//        {
//            Guid id = Guid.NewGuid();
//            LogData(aditionalParameters, new { Request = argument.Serialize(), Guid = id });
//            try
//            {
//                NSDServiceReference.DataClient serviceClient = new NSDServiceReference.DataClient("DataWSDLPort", WebServiceUrl.Value); // NapooStudentDocumentsServiceMockupUrl.Value
//                NSDServiceReference.StudentDocumentRequestType arg = new NSDServiceReference.StudentDocumentRequestType();
//                arg.document_no = argument.DocumentRegistrationNumber;
//                arg.identifier = argument.StudentIdentifier;
//                NSDServiceReference.StudentDocumentResponseType serviceResult = serviceClient.egovSearchStudentDocument(arg);
//                ObjectMapper<NSDServiceReference.StudentDocumentResponseType, DocumentsByStudentResponse> mapper = CreateDocumentsByStudentMapper1(accessMatrix);
//                DocumentsByStudentResponse searchResults = new DocumentsByStudentResponse();
//                mapper.Map(serviceResult, searchResults);
//                return
//                    SigningUtils.CreateAndSign(
//                        argument,
//                        searchResults,
//                        accessMatrix,
//                        aditionalParameters
//                    );
//            }
//            catch (Exception ex)
//            {
//                LogError(aditionalParameters, ex, new { Guid = id });
//                throw ex;
//            }
//        }

//        private static ObjectMapper<NSDServiceReference.StudentDocumentResponseType, DocumentsByStudentResponse> CreateDocumentsByStudentMapper1(AccessMatrix accessMatrix)
//        {
//            ObjectMapper<NSDServiceReference.StudentDocumentResponseType, DocumentsByStudentResponse> mapper = new ObjectMapper<NSDServiceReference.StudentDocumentResponseType, DocumentsByStudentResponse>(accessMatrix);

//            mapper.AddCollectionMap<NSDServiceReference.StudentDocumentResponseType>((o) => o.StudentDocument, (c) => c.data);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentIssueDate, (c) => c.data[0].document_issue_date);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentRegistrationNumber, c => c.data[0].document_reg_no);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentSerialNumber, (c) => c.data[0].document_prn_no);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentSeries, (c) => c.data[0].document_prn_ser);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentType, (c) => c.data[0].document_type_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].EducationType, (c) => c.data[0].course_type_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].FirstName, (c) => c.data[0].first_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].MiddleName, (c) => c.data[0].second_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].LastName, (c) => c.data[0].family_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].GraduationYear, (c) => c.data[0].year_finished);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ProfessionalEduCenterLocation, (c) => c.data[0].city_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ProfessionalEduCenter, (c) => c.data[0].provider_owner);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].QualificationDegree, (c) => c.data[0].speciality_vqs);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ProfessionCodeAndName, (c) => c.data[0].profession_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].StudentIdentifier, (c) => c.data[0].vc_egn);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].SubjectCodeAndName, (c) => c.data[0].speciality_name);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].ClientID, (c) => c.data[0].client_id);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].LicenceNumber, (c) => c.data[0].licence_number);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].DocumentTypeID, (c) => c.data[0].document_type_id);
//            mapper.AddPropertyMap((o) => o.StudentDocument[0].EducationTypeID, (c) => c.data[0].course_type_id);
//            mapper.AddPropertyMap((o) => o.Status, (c) => c.status);
//            mapper.AddPropertyMap((o) => o.Message, (c) => c.message);


//            return mapper;
//        }
//    }
//}
