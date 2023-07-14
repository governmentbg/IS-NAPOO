namespace SoapServiceNAPOOweb.Models.Az.getMTB
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://is.navet.government.bg/ws/")]
    public partial class LoadNAPOOgetMTBDataResponseType
    {

        private bool statusField;

        private string messageField;

        private NAPOOgetMTBData[] dataField;

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
        public NAPOOgetMTBData[] data
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
