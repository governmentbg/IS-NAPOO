namespace SoapServiceNAPOOweb.Models.Az.getCPODataResponse
{
    public partial class LoadNAPOOgetCpoDataResponseType
    {

        private bool statusField;

        private string messageField;

        private NAPOOgetCpoData[] dataField;

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
        public NAPOOgetCpoData[] data
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
