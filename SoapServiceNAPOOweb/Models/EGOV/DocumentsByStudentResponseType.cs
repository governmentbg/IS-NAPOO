using Data.Models.Data.SqlView.WebIntegrationService;
using SoapServiceNAPOOweb.Services;

namespace SoapServiceNAPOOweb.Models.EGOV
{
    public class DocumentsByStudentResponseType
    {
        private bool statusField;

        private string messageField;

        private List<StudentDocument> dataField;

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

        public List<StudentDocument> data
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
