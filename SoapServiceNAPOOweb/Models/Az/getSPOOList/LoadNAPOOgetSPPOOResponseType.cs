namespace SoapServiceNAPOOweb.Models.Az.getSPOOList
{
    public partial class LoadNAPOOgetSPPOOResponseType
    {

        private bool statusField;

        private string messageField;

        private NAPOOgetSPPOO[] dataField;

        /// <remarks/>
        public bool status
        {
            get
            {
                return statusField;
            }
            set
            {
                statusField = value;
            }
        }

        /// <remarks/>
        public string message
        {
            get
            {
                return messageField;
            }
            set
            {
                messageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItem("entries")]
        public NAPOOgetSPPOO[] data
        {
            get
            {
                return dataField;
            }
            set
            {
                dataField = value;
            }
        }
    }
}
