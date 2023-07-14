using RegiX.Class.AVTR.GetActualState;
using RegiX.Class.AVTR.GetActualStateV2;
using RegiX.Class.AVTR.GetValidUICInfo;
using RegiX.Class.AVTR.PersonInCompaniesSearch;
using RegiX.Class.GraoNBD.ValidPersonSearch;
using RegiX.Class.NapooStudentDocuments.GetDocumentsByStudent;
using RegiX.Class.NKPD.GetNKPDAllData;
using RegiX.Class.NRAEmploymentContracts.GetEmploymentContracts;
using RegiX.Class.RDSO.GetDiplomaInfo;
using RegiXServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using RegiX.Class.AVBulstat2.GetStateOfPlay;
using RegiX.Class.Nacid.RdpzsdRegisterEntriesSearch;

namespace RegiX
{
    public class RegiXServiceLocal : IRegiXService
    {
        
        public DiplomaDocumentsType DiplomaDocumentsType(string StudentID, Class.RDSO.GetDiplomaInfo.Request.IdentifierType IDType, string DocumentNumber, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        public EmploymentContractsResponse EmploymentContractsResponse(string ID, Class.NRAEmploymentContracts.GetEmploymentContracts.Request.EikTypeType TYPE, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        public ActualStateResponseType GetActualState(string UIC, CallContext callContext)
        {
            XDocument xDocument = new XDocument();
            xDocument = XDocument.Load(Directory.GetCurrentDirectory() + @"\wwwroot\Data\RegiX\GetActualStateResponse.xml");

            return RegiXSerializerService.GetXDocument<ActualStateResponseType>(xDocument);
        }

        public ActualStateResponseV2 GetActualStateV2(string UIC, CallContext callContext)
        {
            throw new NotImplementedException();
        }

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

        public DocumentsByStudentResponse GetDocumentsByStudent(string StudentIdentifier, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        public AllNKPDDataType GetNKPDAllData(DateTime validDate, CallContext callContext)
        {
            XDocument xDocument = new XDocument();
            //@"\wwwroot\Data\RegiX\GetNKPDAllData.xml"
            //@"\Pages\Test\GetNKPDAllData.xml"
            xDocument = XDocument.Load(Directory.GetCurrentDirectory() + @"\wwwroot\Data\RegiX\GetNKPDAllData.xml");
            
            return RegiXSerializerService.GetXDocument<AllNKPDDataType>(xDocument);
        }

        public DocumentsByStudentResponse GetStudentDocument(string StudentIdentifier, string DocumentRegistrationNumber, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        public ValidUICResponseType GetValidUICInfo(string UIC, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        public SearchParticipationInCompaniesResponseType PersonInCompaniesSearch(string EGN, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        public ValidPersonResponseType ValidPersonResponseType(string EGN, CallContext callContext)
        {
            XDocument xDocument = new XDocument();
            xDocument = XDocument.Load(Directory.GetCurrentDirectory() + @"\wwwroot\Data\RegiX\ValidPersonSearchResponse.xml");

            return RegiXSerializerService.GetXDocument<ValidPersonResponseType>(xDocument); ;
        }

        public string GetFormattedAddress(AddressType addressType)
        {
            string address = string.Empty;

            address =$"{(string.IsNullOrEmpty(addressType.HousingEstate) ? "" : ", жк. " + addressType.HousingEstate)}" +
                     $"{(string.IsNullOrEmpty(addressType.Street) ? "" : ", ул. " + addressType.Street)}" +
                     $"{(string.IsNullOrEmpty(addressType.StreetNumber) ? "" : " " + addressType.StreetNumber)}" +
                     $"{(string.IsNullOrEmpty(addressType.Block) ? "" : ", бл. " + addressType.Block)}" +
                     $"{(string.IsNullOrEmpty(addressType.Entrance) ? "" : ", вх. " + addressType.Entrance)}" +
                     $"{(string.IsNullOrEmpty(addressType.Floor) ? "" : ", ет. " + addressType.Floor)}" +
                     $"{(string.IsNullOrEmpty(addressType.Apartment) ? "" : ", ап. " + addressType.Apartment)}";


            return address.TrimStart(',');
        }

        public StateOfPlay GetStateOfPlay(string UIC, string code, string number, int year, CallContext callContext)
        {


            XDocument xDocument = new XDocument();
            xDocument = XDocument.Load(Directory.GetCurrentDirectory() + @"\wwwroot\Data\RegiX\GetStateOfPlayResponse.xml");

            return RegiXSerializerService.GetXDocument<StateOfPlay>(xDocument);
        }

        public RdpzsdRegisterEntriesResponseType RdpzsdRegisterEntriesSearch(string Uan, string Uin, string IdNumber, string FullName, CallContext callContext)
        {
            throw new NotImplementedException();
        }

        Class.NapooStudentDocuments.GetStudentDocument.DocumentsByStudentResponse IRegiXService.GetStudentDocument(string StudentIdentifier, string DocumentRegistrationNumber, CallContext callContext)
        {
            throw new NotImplementedException();
        }
    }
}
