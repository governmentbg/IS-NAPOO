namespace SoapServiceNAPOOweb.Models.Az.checkSpecialityStatus
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://is.navet.government.bg/ws/")]
    public partial class NAPOOcheckSpecialityStatusData
    {

        private bool is_validField;

        /// <remarks/>
        public bool is_valid
        {
            get
            {
                return this.is_validField;
            }
            set
            {
                this.is_validField = value;
            }
        }
    }
}
