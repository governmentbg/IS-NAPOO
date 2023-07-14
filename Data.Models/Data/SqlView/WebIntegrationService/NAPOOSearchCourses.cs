using System;
using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.SqlView.WebIntegrationService
{
        public class NAPOOSearchCourses
        {
            [Key]
            public int id {get; set;}

            public string? course_name         {
            get
            {
                if (String.IsNullOrEmpty(_course_name))
                {
                    return "";
                }
                else return _course_name;
            }
            set
            {
                _course_name = value;
            }
        }

            public string? course_group_name         {
            get
            {
                if (String.IsNullOrEmpty(_course_group_name))
                {
                    return "";
                }
                else return _course_group_name;
            }
            set
            {
                _course_group_name = value;
            }
        }

            public string? start_date         {
            get
            {
                if (String.IsNullOrEmpty(_start_date))
                {
                    return "";
                }
                else return _start_date;
            }
            set
            {
                _start_date = value;
            }
        }

            public string? end_date         {
            get
            {
                if (String.IsNullOrEmpty(_end_date))
                {
                    return "";
                }
                else return _end_date;
            }
            set
            {
                _end_date = value;
            }
        }

            public string? course_group_city         {
            get
            {
                if (String.IsNullOrEmpty(_course_group_city))
                {
                    return "";
                }
                else return _course_group_city;
            }
            set
            {
                _course_group_city = value;
            }
        }

            public int? provider_id { get; set; }

            public int? course_type_id { get; set; }

            public string? course_type_name         {
            get
            {
                if (String.IsNullOrEmpty(_course_type_name))
                {
                    return "";
                }
                else return _course_type_name;
            }
            set
            {
                _course_type_name = value;
            }
        }

            public string? vet_area_number         {
            get
            {
                if (String.IsNullOrEmpty(_vet_area_number))
                {
                    return "";
                }
                else return _vet_area_number;
            }
            set
            {
                _vet_area_number = value;
            }
        }

            public string? vet_area_name         {
            get
            {
                if (String.IsNullOrEmpty(_vet_area_name))
                {
                    return "";
                }
                else return _vet_area_name;
            }
            set
            {
                _vet_area_name = value;
            }
        }

            public string? vet_profession_number         {
            get
            {
                if (String.IsNullOrEmpty(_vet_profession_number))
                {
                    return "";
                }
                else return _vet_profession_number;
            }
            set
            {
                _vet_profession_number = value;
            }
        }

            public string? vet_profession_name         {
            get
            {
                if (String.IsNullOrEmpty(_vet_profession_name))
                {
                    return "";
                }
                else return _vet_profession_name;
            }
            set
            {
                _vet_profession_name = value;
            }
        }

            public string? vet_speciality_number         {
            get
            {
                if (String.IsNullOrEmpty(_vet_speciality_number))
                {
                    return "";
                }
                else return _vet_speciality_number;
            }
            set
            {
                _vet_speciality_number = value;
            }
        }

            public string? vet_speciality_name         {
            get
            {
                if (String.IsNullOrEmpty(_vet_speciality_name))
                {
                    return "";
                }
                else return _vet_speciality_name;
            }
            set
            {
                _vet_speciality_name = value;
            }
        }

            public int? speciality_vqs { get; set; }

            public string? speciality_vqs_recoded         {
            get
            {
                if (String.IsNullOrEmpty(_speciality_vqs_recoded))
                {
                    return "";
                }
                else return _speciality_vqs_recoded;
            }
            set
            {
                _speciality_vqs_recoded = value;
            }
        }

            public string? provider_name         {
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

            public string? licence_number         {
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

            public string? ekatte_id {
            get
            {
                if (String.IsNullOrEmpty(_ekatte_id))
                {
                    return "";
                }
                else return _ekatte_id;
            }
            set
            {
                _ekatte_id = value;
            }
            }

        public string? vc_name         {
            get
            {
                if (String.IsNullOrEmpty(_vc_name))
                {
                    return "";
                }
                else return _vc_name;
            }
            set
            {
                _vc_name = value;
            }
        }

            public string? contact_pers         {
            get
            {
                if (String.IsNullOrEmpty(_contact_pers))
                {
                    return "";
                }
                else return _contact_pers;
            }
            set
            {
                _contact_pers = value;
            }
        }

            public string? contact_pers_city         {
            get
            {
                if (String.IsNullOrEmpty(_contact_pers_city))
                {
                    return "";
                }
                else return _contact_pers_city;
            }
            set
            {
                _contact_pers_city = value;
            }
        }

            public string? contact_pers_address         {
            get
            {
                if (String.IsNullOrEmpty(_contact_pers_address))
                {
                    return "";
                }
                else return _contact_pers_address;
            }
            set
            {
                _contact_pers_address = value;
            }
        }

            public string? contact_pers_email         {
            get
            {
                if (String.IsNullOrEmpty(_contact_pers_email))
                {
                    return "";
                }
                else return _contact_pers_email;
            }
            set
            {
                _contact_pers_email = value;
            }
        }

            public string? contact_pers_phone1         {
            get
            {
                if (String.IsNullOrEmpty(_contact_pers_phone1))
                {
                    return "";
                }
                else return _contact_pers_phone1;
            }
            set
            {
                _contact_pers_phone1 = value;
            }
        }

            public string? contact_pers_phone2         {
            get
            {
                if (String.IsNullOrEmpty(_contact_pers_phone2))
                {
                    return "";
                }
                else return _contact_pers_phone2;
            }
            set
            {
                _contact_pers_phone2 = value;
            }
        }
        
        //fields
        private string? _course_name ;

        private string? _course_group_name { get; set; }

        private string? _start_date { get; set; }

        private string? _end_date { get; set; }

        private string? _course_group_city { get; set; }

        private int? _provider_id { get; set; }

        private int? _course_type_id { get; set; }

        private string? _course_type_name { get; set; }

        private string? _vet_area_number { get; set; }

        private string? _vet_area_name { get; set; }

        private string? _vet_profession_number { get; set; }

        private string? _vet_profession_name { get; set; }

        private string? _vet_speciality_number { get; set; }

        private string? _vet_speciality_name { get; set; }

        private int? _speciality_vqs { get; set; }

        private string? _speciality_vqs_recoded { get; set; }

        private string? _provider_name { get; set; }

        private string? _licence_number { get; set; }

        private string? _ekatte_id { get; set; }

        private string? _vc_name { get; set; }

        private string? _contact_pers { get; set; }

        private string? _contact_pers_city { get; set; }

        private string? _contact_pers_address { get; set; }

        private string? _contact_pers_email { get; set; }

        private string? _contact_pers_phone1 { get; set; }

        private string? _contact_pers_phone2 { get; set; }
    }
}
