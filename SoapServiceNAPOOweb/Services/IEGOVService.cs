using SoapServiceNAPOOweb.Models.EGOV;

namespace SoapServiceNAPOOweb.Services
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace = "http://is.navet.government.bg/ws/", ConfigurationName = "Data")]
    public interface IData
	{
        [System.ServiceModel.OperationContractAttribute(Action = "urn:IEGOVService#egovSearchStudentDocument", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
       
        StudentDocumentResponseType egovSearchStudentDocument(string identifier, string document_no);

        [System.ServiceModel.OperationContractAttribute(Action = "urn:IEGOVService#egovSearchStudentDocumentByStudent", ReplyAction = "*")]
        [System.ServiceModel.XmlSerializerFormatAttribute()]
    
        DocumentsByStudentResponseType egovSearchStudentDocumentByStudent(string identifier);
    }
}
