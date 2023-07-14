namespace SoapServiceNAPOOweb.Models.Az.getMTB
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://is.navet.government.bg/ws/")]
    public partial class NAPOOgetMTBData
    {

        private int idField;

        private string provider_premise_ekatteField;

        private string cityField;

        private string provider_premise_addressField;

        private string provider_premise_nameField;

        private string provider_premise_notesField;

        private int provider_idField;

        private int licence_numberField;

        private int provider_premise_speciality_usageField;

        private int post_codeField;

        /// <remarks/>
        public int id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        public string provider_premise_ekatte
        {
            get
            {
                return this.provider_premise_ekatteField;
            }
            set
            {
                this.provider_premise_ekatteField = value;
            }
        }

        /// <remarks/>
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        public string provider_premise_address
        {
            get
            {
                return this.provider_premise_addressField;
            }
            set
            {
                this.provider_premise_addressField = value;
            }
        }

        /// <remarks/>
        public string provider_premise_name
        {
            get
            {
                return this.provider_premise_nameField;
            }
            set
            {
                this.provider_premise_nameField = value;
            }
        }

        /// <remarks/>
        public string provider_premise_notes
        {
            get
            {
                return this.provider_premise_notesField;
            }
            set
            {
                this.provider_premise_notesField = value;
            }
        }

        /// <remarks/>
        public int provider_id
        {
            get
            {
                return this.provider_idField;
            }
            set
            {
                this.provider_idField = value;
            }
        }

        /// <remarks/>
        public int licence_number
        {
            get
            {
                return this.licence_numberField;
            }
            set
            {
                this.licence_numberField = value;
            }
        }

        /// <remarks/>
        public int provider_premise_speciality_usage
        {
            get
            {
                return this.provider_premise_speciality_usageField;
            }
            set
            {
                this.provider_premise_speciality_usageField = value;
            }
        }

        /// <remarks/>
        public int post_code
        {
            get
            {
                return this.post_codeField;
            }
            set
            {
                this.post_codeField = value;
            }
        }
    }
}
