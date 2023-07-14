using SoapCore;
using System.ServiceModel.Channels;
using System.Xml;

namespace SoapServiceNAPOOweb.Models.EGOV
{
    public class CustomNamespaceMessage : CustomMessage
    {
        private readonly IEnumerable<string> _customNamespaces = new string[]
        {
            "ns1:urn:DataWSDL#egovSearchStudentDocumentByStudent",
            "ns2:http://is.navet.government.bg/ws/egov"
        };

        private const string EnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";

        public CustomNamespaceMessage() { }

        public CustomNamespaceMessage(Message message) : base(message) { }

        public override MessageHeaders Headers => Message.Headers;

        public override MessageProperties Properties => Message.Properties;

        public override MessageVersion Version => Message.Version;

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("ns1", "egovSearchStudentDocumentByStudentResponse", "urn:DataWSDL#egovSearchStudentDocumentByStudent");
            writer.WriteStartElement(null, "params", null);

            this.Message.WriteBodyContents(writer);
        }

        protected override void OnWriteStartBody(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("Body", EnvelopeNamespace);
        }

        protected override void OnWriteStartEnvelope(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("SOAP-ENV", "Envelope", EnvelopeNamespace);

            foreach (string ns in _customNamespaces)
            {
                var tokens = ns.Split(new char[] { ':' }, 2);
                writer.WriteAttributeString("xmlns", tokens[0], null, tokens[1]);
            }
        }
    }
}
