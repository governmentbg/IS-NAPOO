using Data.Models.Data.SqlView.WebIntegrationService;
namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCipo
{
    public class LoadNAPOOSearchCipoResponseType
    {

        private bool statusField;

        private string messageField;

        private NAPOOSearchCipo[] dataField;

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
        public NAPOOSearchCipo[] data
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
