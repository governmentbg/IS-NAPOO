namespace SoapServiceNAPOOweb.Models.Az.getSPOOList
{
    public partial class NAPOOgetSPPOO
    {

        private int vet_group_idField;

        private int vet_group_numberField;

        private string vet_group_nameField;

        private bool group_is_validField;

        private int vet_area_idField;

        private int vet_area_numberField;

        private string vet_area_nameField;

        private bool area_is_validField;

        private int vet_profession_idField;

        private int vet_profession_numberField;

        private string vet_profession_nameField;

        private bool profession_is_validField;

        private int vet_speciality_idField;

        private int vet_speciality_numberField;

        private string vet_speciality_nameField;

        private string vet_speciality_vqsField;

        private bool speciality_is_validField;

        private DateTime? speciality_start_dateField;

        private DateTime? speciality_end_dateField;

        /// <remarks/>
        public int vet_group_id
        {
            get
            {
                return vet_group_idField;
            }
            set
            {
                vet_group_idField = value;
            }
        }

        /// <remarks/>
        public int vet_group_number
        {
            get
            {
                return vet_group_numberField;
            }
            set
            {
                vet_group_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_group_name
        {
            get
            {
                return vet_group_nameField;
            }
            set
            {
                vet_group_nameField = value;
            }
        }

        /// <remarks/>
        public bool group_is_valid
        {
            get
            {
                return group_is_validField;
            }
            set
            {
                group_is_validField = value;
            }
        }

        /// <remarks/>
        public int vet_area_id
        {
            get
            {
                return vet_area_idField;
            }
            set
            {
                vet_area_idField = value;
            }
        }

        /// <remarks/>
        public int vet_area_number
        {
            get
            {
                return vet_area_numberField;
            }
            set
            {
                vet_area_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_area_name
        {
            get
            {
                return vet_area_nameField;
            }
            set
            {
                vet_area_nameField = value;
            }
        }

        /// <remarks/>
        public bool area_is_valid
        {
            get
            {
                return area_is_validField;
            }
            set
            {
                area_is_validField = value;
            }
        }

        /// <remarks/>
        public int vet_profession_id
        {
            get
            {
                return vet_profession_idField;
            }
            set
            {
                vet_profession_idField = value;
            }
        }

        /// <remarks/>
        public int vet_profession_number
        {
            get
            {
                return vet_profession_numberField;
            }
            set
            {
                vet_profession_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_profession_name
        {
            get
            {
                return vet_profession_nameField;
            }
            set
            {
                vet_profession_nameField = value;
            }
        }

        /// <remarks/>
        public bool profession_is_valid
        {
            get
            {
                return profession_is_validField;
            }
            set
            {
                profession_is_validField = value;
            }
        }

        /// <remarks/>
        public int vet_speciality_id
        {
            get
            {
                return vet_speciality_idField;
            }
            set
            {
                vet_speciality_idField = value;
            }
        }

        /// <remarks/>
        public int vet_speciality_number
        {
            get
            {
                return vet_speciality_numberField;
            }
            set
            {
                vet_speciality_numberField = value;
            }
        }

        /// <remarks/>
        public string vet_speciality_name
        {
            get
            {
                return vet_speciality_nameField;
            }
            set
            {
                vet_speciality_nameField = value;
            }
        }

        /// <remarks/>
        public string vet_speciality_vqs
        {
            get
            {
                return vet_speciality_vqsField;
            }
            set
            {
                vet_speciality_vqsField = value;
            }
        }

        /// <remarks/>
        public bool speciality_is_valid
        {
            get
            {
                return speciality_is_validField;
            }
            set
            {
                speciality_is_validField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(DataType = "date", IsNullable = true)]
        public DateTime? speciality_start_date
        {
            get
            {
                return speciality_start_dateField;
            }
            set
            {
                speciality_start_dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(DataType = "date", IsNullable = true)]
        public DateTime? speciality_end_date
        {
            get
            {
                return speciality_end_dateField;
            }
            set
            {
                speciality_end_dateField = value;
            }
        }
    }
}
