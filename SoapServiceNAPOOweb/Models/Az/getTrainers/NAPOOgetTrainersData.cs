namespace SoapServiceNAPOOweb.Models.Az.getTrainers
{
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://is.navet.government.bg/ws/")]
    public partial class NAPOOgetTrainersData
    {

        private int idField;

        private string egnField;

        private string trainer_nameField;

        private int licence_numberField;

        private int vet_area_qualifiedField;

        private string vet_area_qualified_recodedField;

        private int area_idField;

        private string area_nameField;

        private int theory_practiceField;

        private string theory_practice_recodedField;

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
        public string egn
        {
            get
            {
                return this.egnField;
            }
            set
            {
                this.egnField = value;
            }
        }

        /// <remarks/>
        public string trainer_name
        {
            get
            {
                return this.trainer_nameField;
            }
            set
            {
                this.trainer_nameField = value;
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
        public int vet_area_qualified
        {
            get
            {
                return this.vet_area_qualifiedField;
            }
            set
            {
                this.vet_area_qualifiedField = value;
            }
        }

        /// <remarks/>
        public string vet_area_qualified_recoded
        {
            get
            {
                return this.vet_area_qualified_recodedField;
            }
            set
            {
                this.vet_area_qualified_recodedField = value;
            }
        }

        /// <remarks/>
        public int area_id
        {
            get
            {
                return this.area_idField;
            }
            set
            {
                this.area_idField = value;
            }
        }

        /// <remarks/>
        public string area_name
        {
            get
            {
                return this.area_nameField;
            }
            set
            {
                this.area_nameField = value;
            }
        }

        /// <remarks/>
        public int theory_practice
        {
            get
            {
                return this.theory_practiceField;
            }
            set
            {
                this.theory_practiceField = value;
            }
        }

        /// <remarks/>
        public string theory_practice_recoded
        {
            get
            {
                return this.theory_practice_recodedField;
            }
            set
            {
                this.theory_practice_recodedField = value;
            }
        }
    }

}
