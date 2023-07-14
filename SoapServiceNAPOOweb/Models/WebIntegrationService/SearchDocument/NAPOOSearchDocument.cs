namespace SoapServiceNAPOOweb.Models.WebIntegrationService.SearchDocument
{
    public class NAPOOSearchDocument
    {
        private int idField;

        private string vc_egnField;

        private int document_type_idField;

        private string document_type_nameField;

        private string document_prn_noField;

        private string document_reg_noField;

        private string document_dateField;

        private string document_1_file_nameField;

        private string document_1_mime_typeField;

        private byte[] document_1_file_contentsField;

        private string document_2_file_nameField;

        private string document_2_mime_typeField;

        private byte[] document_2_file_contentsField;

        private int licence_numberField;

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
        public string vc_egn
        {
            get
            {
                return this.vc_egnField;
            }
            set
            {
                this.vc_egnField = value;
            }
        }

        /// <remarks/>
        public int document_type_id
        {
            get
            {
                return this.document_type_idField;
            }
            set
            {
                this.document_type_idField = value;
            }
        }

        /// <remarks/>
        public string document_type_name
        {
            get
            {
                return this.document_type_nameField;
            }
            set
            {
                this.document_type_nameField = value;
            }
        }

        /// <remarks/>
        public string document_prn_no
        {
            get
            {
                return this.document_prn_noField;
            }
            set
            {
                this.document_prn_noField = value;
            }
        }

        /// <remarks/>
        public string document_reg_no
        {
            get
            {
                return this.document_reg_noField;
            }
            set
            {
                this.document_reg_noField = value;
            }
        }

        /// <remarks/>
        public string document_date
        {
            get
            {
                return this.document_dateField;
            }
            set
            {
                this.document_dateField = value;
            }
        }

        /// <remarks/>
        public string document_1_file_name
        {
            get
            {
                return this.document_1_file_nameField;
            }
            set
            {
                this.document_1_file_nameField = value;
            }
        }

        /// <remarks/>
        public string document_1_mime_type
        {
            get
            {
                return this.document_1_mime_typeField;
            }
            set
            {
                this.document_1_mime_typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] document_1_file_contents
        {
            get
            {
                return this.document_1_file_contentsField;
            }
            set
            {
                this.document_1_file_contentsField = value;
            }
        }

        /// <remarks/>
        public string document_2_file_name
        {
            get
            {
                return this.document_2_file_nameField;
            }
            set
            {
                this.document_2_file_nameField = value;
            }
        }

        /// <remarks/>
        public string document_2_mime_type
        {
            get
            {
                return this.document_2_mime_typeField;
            }
            set
            {
                this.document_2_mime_typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "base64Binary")]
        public byte[] document_2_file_contents
        {
            get
            {
                return this.document_2_file_contentsField;
            }
            set
            {
                this.document_2_file_contentsField = value;
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

        public string Oid { get; set; }

    }
}
