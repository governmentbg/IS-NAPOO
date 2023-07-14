namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchStudent
{
    public class LoadNAPOOSearchStudentResponseType
    {
        private bool statusField;

        private string messageField;

        private NAPOOSearchStudent[] dataField;

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
        public NAPOOSearchStudent[] data
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
