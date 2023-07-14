namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList
{
    public class LoadSPPOOResponseType
    {
        private bool statusField;

        private string messageField;

        private vet_group[] dataField;

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
        public vet_group[] data
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
