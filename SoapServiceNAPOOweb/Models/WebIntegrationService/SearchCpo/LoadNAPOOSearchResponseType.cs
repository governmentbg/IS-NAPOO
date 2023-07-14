using Data.Models.Data.SqlView.WebIntegrationService;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCpo
{
    public partial class LoadNAPOOSearchResponseType
    {
        private bool statusField;

        private string messageField;

        private NAPOOSearch[] dataField;

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
        public NAPOOSearch[] data
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
