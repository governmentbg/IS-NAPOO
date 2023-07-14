
using Microsoft.Extensions.Logging;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using RegiX.Class.AVTR.GetActualState;
using RegiX.Class.AVTR.GetActualState.Request;
using RegiX.Class.AVTR.GetActualStateV2;
using RegiX.Class.AVTR.GetActualStateV2.Request;
using RegiX.Class.AVTR.GetValidUICInfo;
using RegiX.Class.AVTR.GetValidUICInfo.Request;
using RegiX.Class.AVTR.PersonInCompaniesSearch;
using RegiX.Class.AVTR.PersonInCompaniesSearch.Request;
using RegiX.Class.GraoNBD.ValidPersonSearch;
using RegiX.Class.NapooStudentDocuments.GetDocumentsByStudent;
using RegiX.Class.NapooStudentDocuments.GetDocumentsByStudent.Request;
using RegiX.Class.NapooStudentDocuments.GetStudentDocument.Request;
using RegiX.Class.NKPD.GetNKPDAllData;
using RegiX.Class.NRAEmploymentContracts.GetEmploymentContracts;
using RegiX.Class.NRAEmploymentContracts.GetEmploymentContracts.Request;
using RegiX.Class.RDSO.GetDiplomaInfo;
using RegiX.Class.RDSO.GetDiplomaInfo.Request;
using RegiXServiceReference;
using System.Xml;
using System.Xml.Linq;
using RegiX.Class.AVBulstat2.GetStateOfPlay.Request;
using static RegiXServiceReference.RegiXEntryPointV2Client;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using RegiX.Class.Nacid.RdpzsdRegisterEntriesSearch;
using ISNAPOO.Common.HelperClasses;
using Microsoft.Extensions.Options;
using GetStudentDocumentResponse = RegiX.Class.NapooStudentDocuments.GetStudentDocument.DocumentsByStudentResponse;

namespace RegiX
{
    public class RegiXService : IRegiXService
    {
        private readonly ILogger<RegiXService> _logger;
        private readonly IConfiguration _Configuration;
        private readonly ApplicationSetting applicationSetting;

        public RegiXService(ILogger<RegiXService> logger, ILoggerFactory loggerFactory, IConfiguration Configuration, IOptions<ApplicationSetting> _applicationSetting)
        {
            // Log category is Full Qualified Name of HomeController class
            _logger = logger;
            _Configuration = Configuration;
            applicationSetting = _applicationSetting.Value;
        }

        private XDocument RegiXSearch(ServiceRequestData request, CallContext callContext)
        {
            //RegiXEntryPointClient client = new RegiXEntryPointClient(EndpointConfiguration.BasicHttpBinding_IRegiXEntryPoint);
            RegiXEntryPointV2Client client = new RegiXEntryPointV2Client(
                RegiXServiceReference.RegiXEntryPointV2Client.EndpointConfiguration.BasicHttpBinding_IRegiXEntryPointV2
                );
            
            string regiXCertificatePath = _Configuration.GetSection("AppSettings")["RegiXCertificatePath"].ToString();
            string regiXCertificatePassword = _Configuration.GetSection("AppSettings")["RegiXCertificatePassword"].ToString();

            X509Certificate2 certificate = new X509Certificate2(regiXCertificatePath, regiXCertificatePassword);

            //
            //string thumbprint = "765f268bc20c64ecd461f36ebffae152470589b3";
            //
            //client.ClientCredentials.ClientCertificate.SetCertificate(
            //    System.Security.Cryptography.X509Certificates.StoreLocation.LocalMachine,
            //    System.Security.Cryptography.X509Certificates.StoreName.Root, System.Security.Cryptography.X509Certificates.X509FindType.FindByThumbprint,
            //    thumbprint);

            client.ClientCredentials.ClientCertificate.Certificate= certificate;

            try
            {
                ServiceResultData respose = Task.Run(async () =>
                {
                    return (await client.ExecuteAsync(new RequestWrapper() { ServiceRequestData = request })).ServiceResultData;
                }).GetAwaiter().GetResult();

                
                if (!respose.HasError)
                {
                    XmlElement result = respose.Data.Response.Any;
                    return XDocument.Load(result.CreateNavigator().ReadSubtree());
                }
                else
                {
                    _logger.LogError("Error in RegiXSearch");
                    _logger.LogError($"response.Error :{respose.Error}");
                    _logger.LogError($"request.Operation:{request.Operation}");
                    

                    return null;
                }
            }
            catch (Exception e)
            {

                _logger.LogError("Error in RegiXSearch");
                _logger.LogError($"Exception.ToString:{e.ToString()}");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");


                
            }

            return null;



        }

        //public static string GetEndpointBehaviorThumbprint()
        //{
        //    ///TODO: Да се направи да се взема от app.config
        //    return "765f268bc20c64ecd461f36ebffae152470589b3";
        //}

        public CallContext GetCallContext()
        {

            CallContext callContext = new CallContext();

            callContext.AdministrationName = "НАПОО";
            callContext.AdministrationOId = "2.16.100.1.1.23.1.3";
            callContext.EmployeeIdentifier = "test@smcon.com";
            callContext.EmployeeNames = "Първо Второ Трето";
            callContext.EmployeePosition = "Експерт в отдел";
            callContext.LawReason = "На основание чл. X от Наредба/Закон/Нормативен акт";
            callContext.Remark = "За тестване на системата";
            callContext.ServiceType = "За административна услуга";
            callContext.ServiceURI = "123-12345-01.01.2017"; //номер на преписка по която се прави справката

           

            return callContext;
        }

        /// <summary>
        ///  REGIX – Агенция по вписванията
        ///  Справка по физическо лице за участие в търговски дружества
        ///  TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.PersonInCompaniesSearch
        ///  https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/PersonInCompaniesSearch
        /// </summary>
        /// <param name="EGN">ЕГН</param>
        /// <returns>Структура на данните (SearchParticipationInCompaniesResponseType)</returns>
        public SearchParticipationInCompaniesResponseType PersonInCompaniesSearch(string EGN, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            SearchParticipationInCompaniesRequestType requestType = new SearchParticipationInCompaniesRequestType();
            requestType.EGN = EGN;
            using (var writer = xDoc.CreateWriter())
            {                
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }
       
            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);

            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.PersonInCompaniesSearch";
            request.Argument = xmlDocument.DocumentElement;

            request.CallContext = callContext;

            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            request.CitizenEGN = "1111111110";
            request.EmployeeEGN = "XXXXXXXXXX";

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<SearchParticipationInCompaniesResponseType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in SearchParticipationInCompaniesResponseType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");                
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – Агенция по вписванията
        /// Справка по код на БУЛСТАТ или по фирмено дело за актуално състояние на субект на БУЛСТАТ
        /// TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualState
        /// TEST UIC = "1212120908"
        /// TEST code = "Code1"
        /// TEST number = ""
        /// TEST year = 0
        /// </summary>
        /// <param name="UIC">ЕИК на фирма</param>
        /// <param name="code">Съд</param>
        /// <param name="number">Номер</param>
        /// <param name="year">Година</param>
        /// <param name="callContext"></param>
        /// <returns>Структура на данните (GetStateOfPlay)</returns>
        public StateOfPlay GetStateOfPlay(string UIC, string code, string number, int year, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            GetStateOfPlayRequest requestType = new GetStateOfPlayRequest();
            GetStateOfPlayRequestCase getStateOfPlayRequestCase = new GetStateOfPlayRequestCase();

            getStateOfPlayRequestCase.Court = new Class.AVBulstat2.GetStateOfPlay.Request.NomenclatureEntry();
            getStateOfPlayRequestCase.Court.Code = code;
            getStateOfPlayRequestCase.Number = number;
            getStateOfPlayRequestCase.Year = year;

            requestType.UIC = UIC;
            requestType.Case = getStateOfPlayRequestCase;

            using (var writer = xDoc.CreateWriter())
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }


            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.AVBulstat2Adapter.APIService.IAVBulstat2API.GetStateOfPlay";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            request.CitizenEGN = "";
            request.EmployeeEGN = "";
            
            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                if (xDocument == null) return null;
                
                return RegiXSerializerService.GetXDocument<StateOfPlay>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in StateOfPlay.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – Агенция по вписванията
        /// Справка за актуално състояние(v1)
        /// TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualState
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualState
        /// TEST UIC = "201593301"
        /// </summary>
        /// <param name="UIC">ЕИК на фирма</param>
        /// <param name="callContext"></param>
        /// <returns>Структура на данните (ActualStateResponseType)</returns>
        public ActualStateResponseType GetActualState(string UIC, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            ActualStateRequestType requestType = new ActualStateRequestType();
            requestType.UIC = UIC;
            using (var writer = xDoc.CreateWriter())
            {                
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }
           

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualState";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<ActualStateResponseType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in ActualStateResponseType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – Агенция по вписванията
        /// Справка за Валидност на ЕИК номер
        /// TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetValidUICInfo
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetValidUICInfo
        /// </summary>
        /// <param name="UIC">UIC = "201593301"</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public ValidUICResponseType GetValidUICInfo(string UIC, CallContext callContext)
        {
            XDocument xDoc = new XDocument();

            ValidUICRequestType requestType = new ValidUICRequestType();
            requestType.UIC = UIC;

            using (var writer = xDoc.CreateWriter())
            {             
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }           

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetValidUICInfo";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<ValidUICResponseType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in ValidUICResponseType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – Агенция по вписванията
        /// Справка за актуално състояние за всички вписани обстоятелства(v2)
        /// TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualStateV2
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI/GetActualStateV2
        /// </summary>
        /// <param name="UIC">
        /// UIC - ЕИК
        /// *FieldList - Списък с полета, които да се филтрират. Може да се ползва разделител запетая. Пример: 001, 00020 - ще върне всички полета които започват с 001 и тези които започват с 00020
        /// </param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public ActualStateResponseV2 GetActualStateV2(string UIC, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            ActualStateRequestV2 requestType = new ActualStateRequestV2();
            requestType.UIC = UIC;
            using (var writer = xDoc.CreateWriter())
            {
            
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }            

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.AVTRAdapter.APIService.ITRAPI.GetActualStateV2";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<ActualStateResponseV2>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in ActualStateResponseV2.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX - НАПОО
        /// Справка за издаден документ на лице по подаден идентификатор и идентификационен (или регистрационен) номер
        /// TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI.GetStudentDocument
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI/GetStudentDocument
        /// </summary>
        /// <param name="StudentIdentifier">Идентификатор на физическо лице - ЕГН/ЛНЧ/Друг вид идентификатор</param>
        /// <param name="DocumentRegistrationNumber">Регистрационен номер на документ</param>
        /// <returns></returns>
        public GetStudentDocumentResponse GetStudentDocument(string StudentIdentifier, string DocumentRegistrationNumber, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            StudentDocumentRequestType requestType = new StudentDocumentRequestType();
            requestType.StudentIdentifier = StudentIdentifier;
            requestType.DocumentRegistrationNumber = DocumentRegistrationNumber;
            using (var writer = xDoc.CreateWriter())
            {                
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }
            
            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI.GetStudentDocument";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<GetStudentDocumentResponse>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in GetStudentDocumentResponse.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX - НАПОО
        /// Справка за издадените документи на лице по подаден идентификатор
        /// TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI.GetDocumentsByStudent
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI/GetDocumentsByStudent
        /// </summary>
        /// <param name="StudentIdentifier">Идентификатор на физическо лице - ЕГН/ЛНЧ/Друг вид идентификатор</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public DocumentsByStudentResponse GetDocumentsByStudent(string StudentIdentifier, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            DocumentsByStudentRequestType requestType = new DocumentsByStudentRequestType();
            requestType.StudentIdentifier = StudentIdentifier;
            using (var writer = xDoc.CreateWriter())
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }           

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.NapooStudentDocumentsAdapter.APIService.INapooStudentDocumentsAPI.GetDocumentsByStudent";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<DocumentsByStudentResponse>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in DocumentsByStudentResponse.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX - МОН (RDSO) 
        /// Регистърът на дипломи и свидетелства за завършено основно и средно образование и придобита степен на професионална квалификация
        /// Справка за диплома за средно образование на определено лице
        /// TechnoLogica.RegiX.RDSOAdapter.APIService.IRDSOAPI.GetDiplomaInfo
        /// </summary>
        /// <param name="StudentID">ЕГН, ЛНЧ или служебен номер на ученика / студента</param>
        /// <param name="IDType">EGN, LNCh, IDN - каквото е подадено като StudentID</param>
        /// <param name="DocumentNumber">Номер на документ - въвежда се само цифровата част на номера</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public DiplomaDocumentsType DiplomaDocumentsType(string StudentID, Class.RDSO.GetDiplomaInfo.Request.IdentifierType IDType,string DocumentNumber, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            DiplomaSearchType requestType = new DiplomaSearchType();
            requestType.StudentID = StudentID;            
            requestType.IDType = IDType;
            requestType.DocumentNumber = DocumentNumber;

            using (var writer = xDoc.CreateWriter())
            {                
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }            

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.RDSOAdapter.APIService.IRDSOAPI.GetDiplomaInfo";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<DiplomaDocumentsType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in DiplomaDocumentsType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – МТСП
        /// ИС на НАПОО ще получава от REGIX данни за Национална класификация на професиите и длъжностите
        /// Справка за търсене на целия класификатор НКПД
        /// TechnoLogica.RegiX.NKPDAdapter.APIService.INKPDAPI.GetNKPDAllData
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NKPDAdapter.APIService.INKPDAPI/GetNKPDAllData
        /// </summary>
        /// <param name="validDate">Дата на валидност</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public AllNKPDDataType GetNKPDAllData(DateTime validDate , CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            
            AllNKPDDataSearchType allNKPDDataSearchType = new AllNKPDDataSearchType();
            allNKPDDataSearchType.ValidDate = validDate;

            using (var writer = xDoc.CreateWriter())
            {                
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(allNKPDDataSearchType.GetType());
                x.Serialize(writer, allNKPDDataSearchType);
            }
            

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.NKPDAdapter.APIService.INKPDAPI.GetNKPDAllData";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<AllNKPDDataType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in AllNKPDDataType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – НАП
        /// ИС на НАПОО ще получава от REGIX данни за сключените трудови договори от завършилите обучение
        /// Справка за сключване, изменение или прекратяване на трудовите договори и уведомления за промяна на работодател
        /// TechnoLogica.RegiX.NRAEmploymentContractsAdapter.APIService.INRAEmploymentContractsAPI.GetEmploymentContracts
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.NRAEmploymentContractsAdapter.APIService.INRAEmploymentContractsAPI/GetEmploymentContracts
        /// </summary>
        /// <param name="ID">Идентификатор(с дължина от 6 до 16 символа)</param>
        /// <param name="TYPE">Вид на идентификатора</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public EmploymentContractsResponse EmploymentContractsResponse(string ID, Class.NRAEmploymentContracts.GetEmploymentContracts.Request.EikTypeType TYPE, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            EmploymentContractsRequest requestType = new EmploymentContractsRequest();
            
            IdentityTypeRequest identityType = new IdentityTypeRequest();          
            identityType.ID = ID;
            identityType.TYPE = TYPE;
            
            requestType.Identity = identityType;

            using (var writer = xDoc.CreateWriter())
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }
            

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);

            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.NRAEmploymentContractsAdapter.APIService.INRAEmploymentContractsAPI.GetEmploymentContracts";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<EmploymentContractsResponse>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in EmploymentContractsResponse.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;

        }

        /// <summary>
        /// REGIX – МРРБ
        /// Справка за валидност на физическо лице
        /// Справка за валидност на физическо лице (Регистър на населението – Национална база данни „Население“).
        /// TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.ValidPersonSearch
        /// https://info-regix.egov.bg/public/administrations/AV/registries/operations/TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI/ValidPersonSearch
        /// </summary>
        /// <param name="EGN">ЕГН</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public ValidPersonResponseType ValidPersonResponseType(string EGN, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            ValidPersonRequestType requestType = new ValidPersonRequestType();
            requestType.EGN = EGN;

            using (var writer = xDoc.CreateWriter())
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }            

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "TechnoLogica.RegiX.GraoNBDAdapter.APIService.INBDAPI.ValidPersonSearch";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<ValidPersonResponseType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in ValidPersonResponseType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;
        }

        /// <summary>
        /// REGIX – NCID
        /// Регистър на всички действащи, прекъснали и завършили студенти и докторанти, по степени на обучение и по професионални направления
        /// Изпълнява услуга по извличане на Справка за статистически и персонални данни и основание за приемане на студенти във висшите училища, техните основни звена и филиали
        /// Nacid.RegiX.RdpzsdAdapter.APIService.IRdpzsdAPI.RdpzsdRegisterEntriesSearch
        /// https://info-regix.egov.bg/public/administrations/NCID/registries/operations/Nacid.RegiX.RdpzsdAdapter.APIService.IRdpzsdAPI/RdpzsdRegisterEntriesSearch
        /// </summary>
        /// <param name="Uan">ЕАН</param>
        /// <param name="Uin">ЕГН</param>
        /// <param name="IdNumber">ЛНЧ</param>
        /// <param name="FullName">Име на кирилица</param>
        /// <param name="callContext"></param>
        /// <returns></returns>
        public RdpzsdRegisterEntriesResponseType RdpzsdRegisterEntriesSearch(string Uan, string Uin, string IdNumber, string FullName, CallContext callContext)
        {
            XDocument xDoc = new XDocument();
            RdpzsdRegisterEntriesRequestType requestType = new RdpzsdRegisterEntriesRequestType();
            requestType.Uan = Uan;
            requestType.Uin = Uin;
            requestType.IdNumber = IdNumber;
            requestType.FullName = FullName;

            using (var writer = xDoc.CreateWriter())
            {
                System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(requestType.GetType());
                x.Serialize(writer, requestType);
            }

            XmlDocument xmlDocument = DocumentExtensions.ToXmlDocument(xDoc);
            ServiceRequestData request = new ServiceRequestData();

            request.Operation = "Nacid.RegiX.RdpzsdAdapter.APIService.IRdpzsdAPI.RdpzsdRegisterEntriesSearch";
            request.Argument = xmlDocument.DocumentElement;
            request.CallContext = callContext;
            request.ReturnAccessMatrix = false;
            request.SignResult = true;

            XDocument xDocument = RegiXSearch(request, callContext);

            try
            {
                return RegiXSerializerService.GetXDocument<RdpzsdRegisterEntriesResponseType>(xDocument);
            }
            catch (Exception e)
            {
                _logger.LogError("Error in RdpzsdRegisterEntriesResponseType.Cast");
                _logger.LogError($"Message:{e.Message}");
                _logger.LogError($"StackTrace:{e.StackTrace}");
                _logger.LogError($"InnerException:{e.InnerException?.Message ?? "none"}");
            }

            return null;
        }

        public string GetFormattedAddress(AddressType addressType)
        {
            string address = string.Empty;

            address = $"{(string.IsNullOrEmpty(addressType.HousingEstate) ? "" : "жк. " + addressType.HousingEstate)}" +
                      $"{(string.IsNullOrEmpty(addressType.Street) ? "" : ", ул. " + addressType.Street)}" +
                      $"{(string.IsNullOrEmpty(addressType.StreetNumber) ? "" : " " + addressType.StreetNumber)}" +
                      $"{(string.IsNullOrEmpty(addressType.Block) ? "" : ", бл. " + addressType.Block)}" +
                      $"{(string.IsNullOrEmpty(addressType.Entrance) ? "" : ", вх. " + addressType.Entrance)}" +
                      $"{(string.IsNullOrEmpty(addressType.Floor) ? "" : ", ет. " + addressType.Floor)}" +
                      $"{(string.IsNullOrEmpty(addressType.Apartment) ? "" : ", ап. " + addressType.Apartment)}";


            return address.TrimStart(',');
        }

    }
}
