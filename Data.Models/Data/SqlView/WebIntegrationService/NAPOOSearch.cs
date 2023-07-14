using System;
using System.ComponentModel.DataAnnotations;


namespace Data.Models.Data.SqlView.WebIntegrationService
{
    public class NAPOOSearch
    {
        [Key]
        public int id { get; set; }

        private string? _licence_number;
        public string? licence_number
        {
            get
            {
                if (String.IsNullOrEmpty(_licence_number))
                {
                    return "";
                }
                else return _licence_number;
            }
            set
            {
                _licence_number = value;
            }
        }

        private string? _provider_name;
        public string? provider_name
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_name))
                {
                    return "";
                }
                else return _provider_name;
            }
            set{
                _provider_name = value;
            } 
        }
        public string ProviderOwner { get; set; }
        
        public int? licence_status_id { get; set; }
       

        private string? _licence_status_name;
        public string? licence_status_name
        {
            get
            {
                if (String.IsNullOrEmpty(_licence_status_name))
                {
                    return "";
                }
                else return _licence_status_name;
            }
            set
            {
                _licence_status_name = value;
            }
        }

        public int?      obl_id { get; set; }

        private string? _obl_name;
        public string?    obl_name
        {
            get
            {
                if (String.IsNullOrEmpty(_obl_name))
                {
                    return "";
                }
                else return _obl_name;
            }
            set
            {
                _obl_name = value;
            }
        }


        public int?      municipality_id { get; set; }

        private string? _municipality_name;
        public string? municipality_name
        {
            get
            {
                if (String.IsNullOrEmpty(_municipality_name))
                {
                    return "";
                }
                else return _municipality_name;
            }
            set
            {
                _municipality_name = value;
            }
        }
        public string?       ekatte_id { get; set; }

        private string? _ekatte_name;
        public string? ekatte_name
        {
            get
            {
                if (String.IsNullOrEmpty(_ekatte_name))
                {
                    return "";
                }
                else return _ekatte_name;
            }
            set
            {
                _ekatte_name = value;
            }
        }

        private string? _zip_code;
        public string? zip_code
        {
            get
            {
                if (String.IsNullOrEmpty(_zip_code))
                {
                    return "";
                }
                else return _zip_code;
            }
            set
            {
                _zip_code = value;
            }
        }

        private string? _provider_address;
        public string? provider_address
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_address))
                {
                    return "";
                }
                else return _provider_address;
            }
            set
            {
                _provider_address = value;
            }
        }

        private string? _provider_phone1;
        public string? provider_phone1
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_phone1))
                {
                    return "";
                }
                else return _provider_phone1;
            }
            set
            {
                _provider_phone1 = value;
            }
        }
        private string? _provider_phone2;
        public string? provider_phone2
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_phone2))
                {
                    return "";
                }
                else return _provider_phone2;
            }
            set
            {
                _provider_phone2 = value;
            }
        }

        private string? _provider_email;
        public string? provider_email
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_email))
                {
                    return "";
                }
                else return _provider_email;
            }
            set
            {
                _provider_email = value;
            }
        }

        private string? _contact_person;
        public string?       contact_person
        {
            get
            {
                if (String.IsNullOrEmpty(_contact_person))
                {
                    return "";
                }
                else return _contact_person;
            }
            set
            {
                _contact_person = value;
            }
        }
        public int?          has_upcoming_course { get; set; }
        public string?   revocation_document { get; set; }
        //Оригиналното е DateTime, променил съм го на string защото от sql връщам забито празен string
        //public DateTime revocation_date { get; set; }
        public string?    revocation_date { get; set; }
        public string  ?    revocation_period { get; set; }
        public string  ?    rev_period_short { get; set; }
        public int     ?    documents_count { get; set; }
        public string?    procedure_date { get; set; }
        public int     ?    reg_state { get; set; }
        public int     ?    reg_state8 { get; set; }


    }
}
