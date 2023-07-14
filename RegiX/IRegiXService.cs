using RegiX.Class.AVBulstat2.GetStateOfPlay;
using RegiX.Class.AVTR.GetActualState;
using RegiX.Class.AVTR.GetActualStateV2;
using RegiX.Class.AVTR.GetValidUICInfo;
using RegiX.Class.AVTR.PersonInCompaniesSearch;
using RegiX.Class.GraoNBD.ValidPersonSearch;
using RegiX.Class.Nacid.RdpzsdRegisterEntriesSearch;
using RegiX.Class.NapooStudentDocuments.GetDocumentsByStudent;
using RegiX.Class.NKPD.GetNKPDAllData;
using RegiX.Class.NRAEmploymentContracts.GetEmploymentContracts;
using RegiX.Class.RDSO.GetDiplomaInfo;
using RegiXServiceReference;

namespace RegiX
{
    public interface IRegiXService
    {
        CallContext GetCallContext();
        ActualStateResponseType GetActualState(string UIC, CallContext callContext);     
        ValidUICResponseType GetValidUICInfo(string UIC, CallContext callContext);
        SearchParticipationInCompaniesResponseType PersonInCompaniesSearch(string EGN, CallContext callContext);
        ActualStateResponseV2 GetActualStateV2(string UIC, CallContext callContext);
        Class.NapooStudentDocuments.GetStudentDocument.DocumentsByStudentResponse GetStudentDocument(string StudentIdentifier, string DocumentRegistrationNumber, CallContext callContext);
        DocumentsByStudentResponse GetDocumentsByStudent(string StudentIdentifier, CallContext callContext);
        DiplomaDocumentsType DiplomaDocumentsType(string StudentID, Class.RDSO.GetDiplomaInfo.Request.IdentifierType IDType, string DocumentNumber, CallContext callContext);
        AllNKPDDataType GetNKPDAllData(DateTime validDate, CallContext callContext);
        EmploymentContractsResponse EmploymentContractsResponse(string ID, Class.NRAEmploymentContracts.GetEmploymentContracts.Request.EikTypeType TYPE, CallContext callContext);
        ValidPersonResponseType ValidPersonResponseType(string EGN, CallContext callContext);
        string GetFormattedAddress(AddressType addressType);
        StateOfPlay GetStateOfPlay(string UIC, string code, string number, int year, CallContext callContext);
        RdpzsdRegisterEntriesResponseType RdpzsdRegisterEntriesSearch(string Uan, string Uin, string IdNumber, string FullName, CallContext callContext);
    }
}