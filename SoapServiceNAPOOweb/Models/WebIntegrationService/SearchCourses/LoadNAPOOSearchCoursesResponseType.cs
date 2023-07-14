using Data.Models.Data.SqlView.WebIntegrationService;

namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchCourses
{
    public class LoadNAPOOSearchCoursesResponseType
    {

        private bool statusField;

        private string messageField;

        private NAPOOSearchCourses[] dataField;

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
        public NAPOOSearchCourses[] data
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
