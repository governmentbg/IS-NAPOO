namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList
{
    public class vet_profession
    {
        private long vet_proffesion_idField;

        private long vet_profession_numberField;

        private string vet_profession_nameField;

        private bool vet_profession_is_validField;

        private string vet_profession_correction_notesField;

        private vet_speciality[] vet_specialitiesField;

        /// <remarks/>
        public long vet_proffesion_id
        {
            get
            {
                return this.vet_proffesion_idField;
            }
            set
            {
                this.vet_proffesion_idField = value;
            }
        }

        /// <remarks/>
        public long vet_profession_number
        {
            get
            {
                return this.vet_profession_numberField;
            }
            set
            {
                this.vet_profession_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_profession_name
        {
            get
            {
                return this.vet_profession_nameField;
            }
            set
            {
                this.vet_profession_nameField = value;
            }
        }

        /// <remarks/>
        public bool vet_profession_is_valid
        {
            get
            {
                return this.vet_profession_is_validField;
            }
            set
            {
                this.vet_profession_is_validField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string vet_profession_correction_notes
        {
            get
            {
                return this.vet_profession_correction_notesField;
            }
            set
            {
                this.vet_profession_correction_notesField = value;
            }
        }

        /// <remarks/>
        public vet_speciality[] vet_specialities
        {
            get
            {
                return this.vet_specialitiesField;
            }
            set
            {
                this.vet_specialitiesField = value;
            }
        }

    }
}
