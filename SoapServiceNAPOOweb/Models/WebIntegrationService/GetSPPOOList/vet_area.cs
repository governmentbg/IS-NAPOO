namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList
{
    public class vet_area
    {
        private long vet_area_idField;

        private long vet_area_numberField;

        private string vet_area_nameField;

        private bool vet_area_is_validField;

        private string vet_area_correction_notesField;

        private vet_profession[] vet_professionsField;

        /// <remarks/>
        public long vet_area_id
        {
            get
            {
                return this.vet_area_idField;
            }
            set
            {
                this.vet_area_idField = value;
            }
        }

        /// <remarks/>
        public long vet_area_number
        {
            get
            {
                return this.vet_area_numberField;
            }
            set
            {
                this.vet_area_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_area_name
        {
            get
            {
                return this.vet_area_nameField;
            }
            set
            {
                this.vet_area_nameField = value;
            }
        }

        /// <remarks/>
        public bool vet_area_is_valid
        {
            get
            {
                return this.vet_area_is_validField;
            }
            set
            {
                this.vet_area_is_validField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string vet_area_correction_notes
        {
            get
            {
                return this.vet_area_correction_notesField;
            }
            set
            {
                this.vet_area_correction_notesField = value;
            }
        }

        /// <remarks/>
        public vet_profession[] vet_professions
        {
            get
            {
                return this.vet_professionsField;
            }
            set
            {
                this.vet_professionsField = value;
            }
        }

    }
}
