namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList
{
    public class vet_speciality
    {
        private long vet_speciality_idField;

        private long vet_speciality_numberField;

        private string vet_speciality_nameField;

        private System.Nullable<int> vet_speciality_vqsField;

        private bool vet_speciality_is_validField;

        private string vet_speciality_correction_notesField;

        private System.Nullable<System.DateTime> vet_speciality_start_dateField;

        private System.Nullable<System.DateTime> vet_speciality_end_dateField;

        /// <remarks/>
        public long vet_speciality_id
        {
            get
            {
                return this.vet_speciality_idField;
            }
            set
            {
                this.vet_speciality_idField = value;
            }
        }

        /// <remarks/>
        public long vet_speciality_number
        {
            get
            {
                return this.vet_speciality_numberField;
            }
            set
            {
                this.vet_speciality_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_speciality_name
        {
            get
            {
                return this.vet_speciality_nameField;
            }
            set
            {
                this.vet_speciality_nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<int> vet_speciality_vqs
        {
            get
            {
                return this.vet_speciality_vqsField;
            }
            set
            {
                this.vet_speciality_vqsField = value;
            }
        }

        /// <remarks/>
        public bool vet_speciality_is_valid
        {
            get
            {
                return this.vet_speciality_is_validField;
            }
            set
            {
                this.vet_speciality_is_validField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string vet_speciality_correction_notes
        {
            get
            {
                return this.vet_speciality_correction_notesField;
            }
            set
            {
                this.vet_speciality_correction_notesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> vet_speciality_start_date
        {
            get
            {
                return this.vet_speciality_start_dateField;
            }
            set
            {
                this.vet_speciality_start_dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> vet_speciality_end_date
        {
            get
            {
                return this.vet_speciality_end_dateField;
            }
            set
            {
                this.vet_speciality_end_dateField = value;
            }
        }

    }
}
