namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchDocument
{
    public class LoadNAPOOSearchDocumentResponseType
    {
        private bool statusField;

        private string messageField;

        private NAPOOSearchDocument[] dataField;

        /// <remarks/>
        public bool status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }

        /// <remarks/>
        public string message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("entries")]
        public NAPOOSearchDocument[] data
        {
            get
            {
                return this.dataField;
            }
            set
            {
                this.dataField = value;
            }
        }

    }
}
