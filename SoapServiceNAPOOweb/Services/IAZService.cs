using SoapServiceNAPOOweb.Models.Az.getCipoData;
using SoapServiceNAPOOweb.Models.Az.getCPODataResponse;
using SoapServiceNAPOOweb.Models.Az.getMTB;
using SoapServiceNAPOOweb.Models.Az.getSPOOList;
using SoapServiceNAPOOweb.Models.Az.getTrainers;
using SoapServiceNAPOOweb.Models.Az.checkSpecialityStatus;

namespace SoapServiceNAPOOweb.Services
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://is.navet.government.bg/ws/", ConfigurationName = "Data")]
    public interface IAZService
    {
        [System.ServiceModel.OperationContractAttribute(Action = "urn:DataWSDL#getSPPOOList", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        LoadNAPOOgetSPPOOResponseType getSPPOOList(string username, string password, bool active);


        [System.ServiceModel.OperationContractAttribute(Action = "urn:DataWSDL#getCPOData", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        LoadNAPOOgetCpoDataResponseType getCPOData(string username, string password, string licence_number, string bulstat);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:DataWSDL#getCipoData", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        LoadNAPOOgetCipoDataResponseType getCipoData(string username, string password, string licence_number, string bulstat);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:DataWSDL#getTrainers", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        LoadNAPOOgetTrainersDataResponseType getTrainers(string username, string password, int licence_number, string bulstat, int int_area);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:DataWSDL#getMTB", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        LoadNAPOOgetMTBDataResponseType getMTB(string username, string password, int licence_number, string bulstat);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:DataWSDL#checkSpecialityStatus", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
        LoadNAPOOgcheckSpecialityStatusDataResponseType checkSpecialityStatus(string username, string password, int licence_number, string bulstat, int int_speciality);
    }
}

