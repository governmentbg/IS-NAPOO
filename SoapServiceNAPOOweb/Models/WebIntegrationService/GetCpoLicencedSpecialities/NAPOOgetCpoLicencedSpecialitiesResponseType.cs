namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetCpoLicencedSpecialities
{
    public class NAPOOgetCpoLicencedSpecialitiesResponseType
    {
            private bool statusField;

            private string messageField;

            private NAPOOgetCpoLicencedSpecialities[] dataField;

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
            public NAPOOgetCpoLicencedSpecialities[] data
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
