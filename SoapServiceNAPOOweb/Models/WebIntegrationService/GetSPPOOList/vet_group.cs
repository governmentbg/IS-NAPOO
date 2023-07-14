namespace SoapServiceNAPOOweb.Models.WebIntegrationService.GetSPPOOList
{
    public class vet_group
    {
        private long vet_group_idField;

        private long vet_group_numberField;

        private string vet_group_nameField;

        private bool vet_group_is_validField;

        private string vet_group_correction_notesField;

        private vet_area[] vet_areasField;

        /// <remarks/>
        public long vet_group_id
        {
            get
            {
                return this.vet_group_idField;
            }
            set
            {
                this.vet_group_idField = value;
            }
        }

        /// <remarks/>
        public long vet_group_number
        {
            get
            {
                return this.vet_group_numberField;
            }
            set
            {
                this.vet_group_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_group_name
        {
            get
            {
                return this.vet_group_nameField;
            }
            set
            {
                this.vet_group_nameField = value;
            }
        }

        /// <remarks/>
        public bool vet_group_is_valid
        {
            get
            {
                return this.vet_group_is_validField;
            }
            set
            {
                this.vet_group_is_validField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string vet_group_correction_notes
        {
            get
            {
                return this.vet_group_correction_notesField;
            }
            set
            {
                this.vet_group_correction_notesField = value;
            }
        }

        /// <remarks/>
        public vet_area[] vet_areas
        {
            get
            {
                return this.vet_areasField;
            }
            set
            {
                this.vet_areasField = value;
            }
        }

    }
}
