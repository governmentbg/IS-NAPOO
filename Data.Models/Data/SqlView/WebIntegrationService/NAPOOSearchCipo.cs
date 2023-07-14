using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.SqlView.WebIntegrationService
{
    public class NAPOOSearchCipo
    {
        [Key]
        public int id
        {
            get; set;
        }

        /// <remarks/>
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
            set
            {
                _provider_name = value;
            }
        }

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

        public int? obl_id { get; set; }

        private string? _obl_name;
        public string? obl_name
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


        public int? municipality_id { get; set; }

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
        public string? ekatte_id { get; set; }

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

        /// <remarks/>
        private string? _vc_provider_manager;
        public string? vc_provider_manager
        {
            get
            {
                if (String.IsNullOrEmpty(_vc_provider_manager))
                {
                    return "";
                }
                else return _vc_provider_manager;
            }
            set
            {
                _vc_provider_manager = value;
            }
        }

        /// <remarks/>
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

        /// <remarks/>
        private string? _provider_fax;
        public string? provider_fax
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_fax))
                {
                    return "";
                }
                else return _provider_fax;
            }
            set
            {
                _provider_fax = value;
            }
        }

        /// <remarks/>
        private string? _provider_web;
        public string? provider_web
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_web))
                {
                    return "";
                }
                else return _provider_web;
            }
            set
            {
                _provider_web = value;
            }
        }

        private string? _provider_contact_person;
        public string? provider_contact_person
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_contact_person))
                {
                    return "";
                }
                else return _provider_contact_person;
            }
            set
            {
                _provider_contact_person = value;
            }
        }


        /// <remarks/>
        public int? provider_contact_pers_ekatte_id
        {
            get; set;
        }

        /// <remarks/>
        /// 
        private string? _contact_person_vc_obl_name;
        public string? contact_person_vc_obl_name
        {
                       get
            {
                if (String.IsNullOrEmpty(_contact_person_vc_obl_name))
                {
                    return "";
                }
                else return _contact_person_vc_obl_name;
            }
            set
            {
                _contact_person_vc_obl_name = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _contact_person_vc_municipality_name;
        public string? contact_person_vc_municipality_name
        {
                       get
            {
                if (String.IsNullOrEmpty(_contact_person_vc_municipality_name))
                {
                    return "";
                }
                else return _contact_person_vc_municipality_name;
            }
            set
            {
                _contact_person_vc_municipality_name = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _contact_person_ekatte_name;
        public string? contact_person_ekatte_name
        {
                       get
            {
                if (String.IsNullOrEmpty(_contact_person_ekatte_name))
                {
                    return "";
                }
                else return _contact_person_ekatte_name;
            }
            set
            {
                _contact_person_ekatte_name = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _provider_contact_pers_zipcode;
        public string? provider_contact_pers_zipcode
        {
                       get
            {
                if (String.IsNullOrEmpty(_provider_contact_pers_zipcode))
                {
                    return "";
                }
                else return _provider_contact_pers_zipcode;
            }
            set
            {
                _provider_contact_pers_zipcode = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _provider_contact_pers_address;
        public string? provider_contact_pers_address
        {
            get
            {
                if (String.IsNullOrEmpty(_provider_contact_pers_address))
                {
                    return "";
                }
                else return _provider_contact_pers_address;
            }
            set
            {
                _provider_contact_pers_address = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _provider_contact_pers_phone1;
        public string? provider_contact_pers_phone1
        {
                       get
            {
                if (String.IsNullOrEmpty(_provider_contact_pers_phone1))
                {
                    return "";
                }
                else return _provider_contact_pers_phone1;
            }
            set
            {
                _provider_contact_pers_phone1 = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _provider_contact_pers_phone2;
        public string? provider_contact_pers_phone2
        {
                       get
            {
                if (String.IsNullOrEmpty(_provider_contact_pers_phone2))
                {
                    return "";
                }
                else return _provider_contact_pers_phone2;
            }
            set
            {
                _provider_contact_pers_phone2 = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _provider_contact_pers_fax;
        public string? provider_contact_pers_fax
        {
                       get
            {
                if (String.IsNullOrEmpty(_provider_contact_pers_fax))
                {
                    return "";
                }
                else return _provider_contact_pers_fax;
            }
            set
            {
                _provider_contact_pers_fax = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _provider_contact_pers_email;
        public string? provider_contact_pers_email
        {
                       get
            {
                if (String.IsNullOrEmpty(_provider_contact_pers_email))
                {
                    return "";
                }
                else return _provider_contact_pers_email;
            }
            set
            {
                _provider_contact_pers_email = value;
            }
        }

        /// <remarks/>
        /// 
        private string? _licance_data;
        public string? licence_data
        {
            get
            {
                if (String.IsNullOrEmpty(_licance_data))
                {
                    return "";
                }
                else return _licance_data;
            }
            set
            {
                _licance_data = value;
            }
        }

        /// <remarks/>
        public int? documents_count
        {
            get; set;
        }

        /// <remarks/>
        public string? procedure_date
        {
            get; set;
        }

        /// <remarks/>
        public int? reg_state
        {
            get; set;
        }

        /// <remarks/>
        public int? reg_state8
        {
            get; set;
        }
    }
}
