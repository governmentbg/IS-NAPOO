using System.ComponentModel.DataAnnotations;

namespace Data.Models.Data.SqlView.WebIntegrationService
{
    public class StudentDocumentVM
    {
        [Key]
        public int? client_id { get; set; }

        public string? vc_egn { get; set; }

        public string? first_name { get; set; }

        public string? second_name { get; set; }

        public string? family_name { get; set; }

        public string? licence_number { get; set; }

        public string? provider_owner { get; set; }

        public string? city_name { get; set; }

        public int? document_type_id { get; set; }

        public string? document_type_name { get; set; }

        public int? course_type_id { get; set; }

        public string? course_type_name { get; set; }

        public string? profession_name { get; set; }

        public string? speciality_name { get; set; }

        public int? speciality_vqs { get; set; }

        public int? year_finished { get; set; }

        public string? document_prn_ser { get; set; }

        public string? document_prn_no { get; set; }

        public string? document_reg_no { get; set; }

        public System.DateTime? document_issue_date { get; set; }
    }
}
