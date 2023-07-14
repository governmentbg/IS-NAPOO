using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NapooMigration.Migrations
{
    public partial class INIT : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "arch_tb_course_groups_40_id_seq",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "arch_tb_course_groups_duplicates_id_seq",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "ref_course_type_fr_curr_int_code_course_fr_curr_id_seq",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "ref_course_type_fr_curr_int_code_course_type_id_seq",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_accs_adminlogid",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_accs_groupnames",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_accs_sessionid",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_clients_courses_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_courses_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_courses_providers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_providers_department_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_providers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_arch_trainers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_assign_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_candidate_providers_state_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_candidate_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_ccontract_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_cfinished_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_cipo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_comm_member_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_commission_institution_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_correction_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_course_ed_form_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_course_fr_curr_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_course_group_required_documents_type_id",
                minValue: 1L,
                maxValue: 2048L);

            migrationBuilder.CreateSequence(
                name: "seq_code_course_measure_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_course_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_course_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_cpo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_curric_hours_type_id",
                minValue: 1L,
                maxValue: 900L);

            migrationBuilder.CreateSequence(
                name: "seq_code_document_status",
                minValue: 1L,
                maxValue: 900L);

            migrationBuilder.CreateSequence(
                name: "seq_code_document_status_locks_id",
                minValue: 1L,
                maxValue: 900L);

            migrationBuilder.CreateSequence(
                name: "seq_code_document_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_documents_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_education_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_egn_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_ekatte_full_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_ekatte_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_employment_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_expert_position_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_expert_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_gender_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_licence_status_details_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_licence_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_municipality_details_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_municipality_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_nationality_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_nkpd_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_obl_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_operation_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_premises_correspondence_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_premises_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_premises_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_premises_usage_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_procedures_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_provider_ownership_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_provider_registration_id",
                minValue: 1L,
                maxValue: 922337L);

            migrationBuilder.CreateSequence(
                name: "seq_code_provider_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_qual_level_id",
                minValue: 1L,
                maxValue: 900L);

            migrationBuilder.CreateSequence(
                name: "seq_code_qualification_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_request_doc_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_request_doc_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_speciality_curriculum_update_reason_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_speciality_vqs_id",
                minValue: 1L,
                maxValue: 100L);

            migrationBuilder.CreateSequence(
                name: "seq_code_tcontract_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_tqualification_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_training_type_id",
                minValue: 1L,
                maxValue: 900L);

            migrationBuilder.CreateSequence(
                name: "seq_code_validity_check_target",
                minValue: 1L,
                maxValue: 512L);

            migrationBuilder.CreateSequence(
                name: "seq_code_vet_area_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_vet_group_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_vet_list_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_vet_profession_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_vet_speciality_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_village_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_visit_result_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_code_wgdoi_function_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_groups_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_expert_commissions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_procedure_expert_commissions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_procedure_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_procedure_procedures_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_procedure_provider_premises_specialities_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_arch_procedures_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_candidates_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_candidates_procedures_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_cg_curric_files_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_clients_courses_documents_status",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_clients_courses_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_clients_lab_offices_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_course_document_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_course_group_premises_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_courses_providers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_document_type_cfinished_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_experts_commissions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_experts_doi_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_experts_types_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_experts_vet_area_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_fr_curr_ed_form_id",
                minValue: 1L,
                maxValue: 900L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_fr_curr_educ_level_id",
                minValue: 1L,
                maxValue: 9000L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_fr_curr_qual_level_id",
                minValue: 1L,
                maxValue: 9000L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_main_expert_commissions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_main_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_procedures_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_provider_premises_specialities_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_request_doc_status_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_request_doc_type_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_role_acl_actions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_role_groups_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_role_users_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_trainers_courses_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_vet_specialities_nkpds_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_visits_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_ref_visits_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_sys_acl_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_sys_mail_log_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_sys_operation_log_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_sys_sign_log_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_acl_actions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_acl_defaults_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_annual_info_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_provider_premises_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_provider_premises_rooms_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_provider_specialities_curriculum_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_provider_specialities_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_providers_cipo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_providers_cpo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_providers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_trainer_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_trainer_profiles_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_trainer_qualifications_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_procedure_trainers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_arch_providers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_candidate_providers_cipo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_candidate_providers_cpo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_candidate_providers_documents_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_candidate_providers_state_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_clients_courses_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_clients_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_clients_required_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_course_groups_40_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_course_groups_duplicates_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_course_groups_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_course40_competences_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_courses_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_curric_modules_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_doi_commissions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_doi_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_e_signers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_expert_commissions_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_expert_profiles_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_experts_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_generated_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_import_xml_id",
                startValue: 10L,
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_lab_offices_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_napoo_request_doc_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_payments_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_procedure_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_procedure_prices_id",
                minValue: 1L,
                maxValue: 1000L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_procedure_progress_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_procedure_progress_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_activities_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_departments_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_premises_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_premises_rooms_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_specialities_curriculum_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_specialities_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_provider_uploaded_docs_id",
                startValue: 74L,
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_cipo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_cpo_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_docs_dashboard_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_docs_offers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_documents_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_licence_change_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_providers_request_doc_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_request_docs_management_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_request_docs_sn_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_roles_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_sign_content_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_started_procedure_progress_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_started_procedures_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_survey_answer_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_survey_question_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_trainer_documents_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_trainer_profiles_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_trainer_qualifications_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_tb_trainers_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateSequence(
                name: "seq_users_id",
                minValue: 1L,
                maxValue: 2147483647L);

            migrationBuilder.CreateTable(
                name: "accs_accesslog",
                columns: table => new
                {
                    sessionid = table.Column<long>(type: "bigint", nullable: false),
                    timestart = table.Column<long>(type: "bigint", nullable: true),
                    timeend = table.Column<long>(type: "bigint", nullable: true),
                    timeused = table.Column<long>(type: "bigint", nullable: true),
                    userid = table.Column<long>(type: "bigint", nullable: true),
                    ggid = table.Column<long>(type: "bigint", nullable: true),
                    lgid = table.Column<long>(type: "bigint", nullable: true),
                    ip = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("accs_accesslog_pkey", x => x.sessionid)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "accs_adminlog",
                columns: table => new
                {
                    al_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    t = table.Column<long>(type: "bigint", nullable: true),
                    sessionid = table.Column<long>(type: "bigint", nullable: true),
                    t_userid = table.Column<long>(type: "bigint", nullable: true),
                    action = table.Column<long>(type: "bigint", nullable: true),
                    adata = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("accs_adminlog_pkey", x => x.al_id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "accs_blockedip",
                columns: table => new
                {
                    ip = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    nt = table.Column<long>(type: "bigint", nullable: true),
                    lt = table.Column<long>(type: "bigint", nullable: true),
                    tf = table.Column<long>(type: "bigint", nullable: true),
                    ts = table.Column<long>(type: "bigint", nullable: true),
                    sf = table.Column<long>(type: "bigint", nullable: true),
                    un = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "accs_failures",
                columns: table => new
                {
                    t = table.Column<long>(type: "bigint", nullable: true),
                    ip = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    uname = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    errorcode = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "accs_messages",
                columns: table => new
                {
                    mask = table.Column<long>(type: "bigint", nullable: false),
                    msg = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("accs_messages_pkey", x => x.mask)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "accs_webuserdata",
                columns: table => new
                {
                    sessiondata = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: false),
                    lastlogin = table.Column<long>(type: "bigint", nullable: true),
                    firstlogin = table.Column<long>(type: "bigint", nullable: true),
                    nsec = table.Column<long>(type: "bigint", nullable: true),
                    udat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    adat = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("accs_webuserdata_pkey", x => x.sessiondata)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "accs_webusergroups",
                columns: table => new
                {
                    group_id = table.Column<int>(type: "int", nullable: true),
                    group_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    permissions = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "arch_ref_clients_courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_second_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_family_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cfinished_type_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_course_finished = table.Column<DateTime>(type: "date", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_gender = table.Column<int>(type: "int", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<int>(type: "int", nullable: true),
                    dt_client_birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qual_level = table.Column<long>(type: "bigint", nullable: true),
                    int_qual_vet_area = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_ref_clients_courses_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_ref_provider_premises_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_speciality_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_speciality_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_speciality_correspondence = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_ref_provider_premises_specialities_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_clients",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    vc_client_first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_client_second_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_client_family_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_gender = table.Column<int>(type: "int", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_client_birth_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_clients_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_clients_courses_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_clients_courses_id = table.Column<long>(type: "bigint", nullable: true),
                    int_document_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_finished_year = table.Column<int>(type: "int", nullable: true),
                    vc_document_prn_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_document_reg_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dt_document_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_document_prot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    num_theory_result = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    num_practice_result = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    vc_qualification_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_qualificatioj_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    document_1_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    document_2_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_document_prn_ser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    int_document_status = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_clients_courses_documents_id", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_course_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_course_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_course_subscribe_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_additional_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_measure_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_group_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_course_status_id = table.Column<long>(type: "bigint", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_ed_form_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_assign_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_course_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_duration = table.Column<int>(type: "int", nullable: true),
                    num_course_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_theory_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_practice_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_exam_comm_members = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_selectable_hours = table.Column<int>(type: "int", nullable: true),
                    int_mandatory_hours = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_p_disability_count = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_course_groups_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_course_groups_40",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise = table.Column<long>(type: "bigint", nullable: true),
                    int_course_ed_form_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_duration = table.Column<int>(type: "int", nullable: true),
                    int_selectable_hours = table.Column<int>(type: "int", nullable: true),
                    int_mandatory_hours = table.Column<int>(type: "int", nullable: true),
                    num_course_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_theory_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_practice_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_exam_comm_members = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_additional_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_assign_type_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_course_groups_40_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_course_groups_duplicates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_document_type_id = table.Column<long>(type: "bigint", nullable: false),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_course_finished_year = table.Column<int>(type: "int", nullable: true),
                    vc_original_prn_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_original_reg_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dt_original_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_original_ref_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_original_document_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_course_groups_duplicates_pk", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_course_no = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_course_add_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_educ_requirement = table.Column<int>(type: "int", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_mandatory_hours = table.Column<int>(type: "int", nullable: true),
                    int_selectable_hours = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_courses_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_provider_premises",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_provider_premise_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_ekatte = table.Column<long>(type: "bigint", nullable: true),
                    txt_provider_premise_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_status = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_visited = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_provider_premises_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_provider_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_licence_data = table.Column<DateTime>(type: "date", nullable: true),
                    int_licence_prot_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    txt_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_added = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_provider_specialities_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_provider_specialities_curriculum",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_updated = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    dt_update_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_speciality_curriculum_update_reason_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_provider_specialities_curriculum_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_providers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_status_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_status_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_local_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_zip_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_web = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_contact_pers_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_contact_pers_zipcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_contact_pers_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_brra = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    vc_provider_profile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_provider_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_licence_data = table.Column<DateTime>(type: "date", nullable: true),
                    int_provider_registration_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_providers_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_trainer_qualifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_qualification_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_qualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_tqualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qualification_duration = table.Column<int>(type: "int", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_trainer_qualifications_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "arch_tb_trainers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_trainer_first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_trainer_second_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_trainer_family_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_birth_year = table.Column<int>(type: "int", nullable: true),
                    int_gender_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_education_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_certificate_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_academic_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_andragog = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_tcontract_type_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_tcontract_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("arch_tb_trainers_pkey", x => new { x.id, x.int_year })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_assign_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_assign_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    int_order_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_assign_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_candidate_providers_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_candidate_providers_state_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_candidate_providers_state_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_candidate_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_candidate_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_candidate_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_ccontract_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_ccontract_type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_ccontract_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_cfinished_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_cfinished_type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bool_finished = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_cfinished_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_cipo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_code_cipo_management_ident_new = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_code_cipo_management_ident_add = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_code_cipo_management_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: true),
                    int_ui_control_type = table.Column<int>(type: "int", nullable: true),
                    vc_please_text = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    vc_list_table = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_extra_info = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    int_documents_management_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_cipo_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_comm_member",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_comm_member_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_comm_member_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_commission_institution_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_commission_institution_type_name = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    vc_short_name = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_commission_institution_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_correction",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_correction_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_correction_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_course_ed_form",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_course_ed_form_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_order = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_course_ed_form_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_course_fr_curr",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_course_fr_curr_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    int_order = table.Column<int>(type: "int", nullable: true),
                    vc_short_desc = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_course_type = table.Column<long>(type: "bigint", nullable: true),
                    int_course_validation_type = table.Column<long>(type: "bigint", nullable: true),
                    vc_description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    vc_desc_in_edu = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    vc_desc_in_qual = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    vc_ed_forms = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    int_duration_months = table.Column<long>(type: "bigint", nullable: true),
                    int_mandatory_hours = table.Column<long>(type: "bigint", nullable: true),
                    int_selectable_hours = table.Column<long>(type: "bigint", nullable: true),
                    int_total_hours = table.Column<long>(type: "bigint", nullable: true),
                    int_min_perc_practice = table.Column<long>(type: "bigint", nullable: true),
                    bool_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_vqs = table.Column<int>(type: "int", nullable: true),
                    int_min_perc_common = table.Column<long>(type: "bigint", nullable: true),
                    vc_desc_perc = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_course_fr_curr_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_course_group_required_documents_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_document_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    bool_for_client = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_mandatory = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_course_type = table.Column<long>(type: "bigint", nullable: true),
                    vc_checkbox_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    checkbox_order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_course_group_required_documents_type_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_course_measure_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_course_measure_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_course_measure_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_course_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_course_status_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_course_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_course_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_course_type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_course_type_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    bool_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_group = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    vc_course_type_short = table.Column<string>(type: "nvarchar(155)", maxLength: 155, nullable: true),
                    bool_has_fr_curr = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_rdpk_check = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    bool_has_speciality = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_cfinished_type_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_course_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_cpo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_code_cpo_management_ident_new = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_code_cpo_management_ident_add = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_code_cpo_management_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    version = table.Column<long>(type: "bigint", nullable: true),
                    int_ui_control_type = table.Column<int>(type: "int", nullable: true),
                    vc_please_text = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_list_table = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_extra_info = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    int_documents_management_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_code_cpo_management_ident_p4 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_cpo_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_curric_hours_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_curric_hours_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_code_training_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_section_code = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_curric_hours_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_document_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_status_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    vc_button_name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    bool_with_note = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_cg = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_client = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_client_docs = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_rollback = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_add_protokols = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_add_clients = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_add_document = table.Column<bool>(type: "bit", nullable: true),
                    bool_lock_protokols = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_docuement_status_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_document_status_locks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_status_lock_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_document_status_locks_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_document_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_document_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_document_type_name_en = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bool_more_than_one = table.Column<bool>(type: "bit", nullable: true),
                    int_parent_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_has_fab_number = table.Column<bool>(type: "bit", nullable: true),
                    bool_has_marks = table.Column<bool>(type: "bit", nullable: true),
                    bool_has_file = table.Column<bool>(type: "bit", nullable: true),
                    bool_has_qual = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_document_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_document_validity_checks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    vc_condition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    int_document_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type = table.Column<long>(type: "bigint", nullable: true),
                    bool_mandatory = table.Column<bool>(type: "bit", nullable: true),
                    int_code_validity_check_target = table.Column<int>(type: "int", nullable: true),
                    bool_if_rows0 = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    vc_in_file = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    vc_function_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_duplicate = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "code_documents_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_documents_management_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_candidate_type_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_brra = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    bool_is_mandatory = table.Column<bool>(type: "bit", nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    order_id = table.Column<int>(type: "int", nullable: true),
                    bool_is_not_brra = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    bool_is_prn_only = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    bool_is_conditional = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    vc_condition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    seenbyexpert = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_documents_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_education",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_education_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_mon_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_education_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_egn_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_egn_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_egn_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_ekatte",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_village_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_kati = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    vc_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_text_code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    vc_cat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    vc_height = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    int_post_code = table.Column<int>(type: "int", nullable: true),
                    vc_phone_code = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_ekatte_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_ekatte_full",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_village_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_kati = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    vc_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_text_code = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    vc_cat = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    vc_height = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    int_post_code = table.Column<int>(type: "int", nullable: true),
                    vc_phone_code = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_ekatte_full_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_employment_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_employment_status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_employment_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_expert_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_expert_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_expert_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_ext_register",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_register_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_ext_register_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_gender",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_gender_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_licence_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_licence_status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_licence_status_name_en = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_lic_status_short_name = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    vc_lic_status_short_name_en = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_licence_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_licence_status_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_licence_status_details_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_licence_status_details_name_en = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_licence_status_details_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_municipality",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_municipality_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_municipality_code_name = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_municipality_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_municipality_details",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_municipality_details_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_municipality_details_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_nationality",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_country_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    bool_is_eu_member = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_nationality_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_nkpd",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_nkpd_id1 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    int_nkpd_id2 = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_nkpd_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_nkpd_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_obl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_obl_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_obl_code_name = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_obl_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_operation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_operation_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_premises_correspondence",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_premises_correspondence_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_premises_correspondence_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_premises_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_premises_status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_premises_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_premises_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_premises_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_premises_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_premises_usage",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_premises_usage_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_premises_usage_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_procedure_decision",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    vc_procedure_decision = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_procedure_decision_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_procedure_decision_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_procedure_stages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_procedure_stages_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_procedure_steps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_procedure_steps_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_procedures",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_procedures_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_procedures_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_procedures_documents_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_candidate_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_order_id = table.Column<int>(type: "int", nullable: true),
                    int_document_id = table.Column<int>(type: "int", nullable: true),
                    int_row_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_procedures_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_protokol_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_code_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    bool_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_protokol_type_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_provider_ownership",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_provider_ownership_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_provider_ownership_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_provider_registration",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_provider_registration_type_name = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_provider_registration_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_provider_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_provider_status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    is_cpo = table.Column<bool>(type: "bit", nullable: true),
                    is_brra = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_provider_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_qual_level",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_qual_level_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_grade_spk = table.Column<int>(type: "int", nullable: true),
                    bool_spk_part = table.Column<bool>(type: "bit", nullable: true),
                    bool_same_area = table.Column<bool>(type: "bit", nullable: true),
                    bool_same_prof = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_qual_level_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_qualification_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_qualification_type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_qualification_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_receive_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_receive_documents_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_receive_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_receive_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_receive_type_name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_receive_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_request_doc_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_request_doc_status_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_request_doc_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_request_doc_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_request_doc_type_official_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_request_doc_type_name = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true),
                    int_current_period = table.Column<int>(type: "int", nullable: true),
                    int_doc_price = table.Column<float>(type: "real", nullable: true),
                    int_order_id = table.Column<int>(type: "int", nullable: true),
                    bool_has_serial_number = table.Column<bool>(type: "bit", nullable: true),
                    is_destroyable = table.Column<bool>(type: "bit", nullable: false),
                    int_code_document_type_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_request_doc_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_request_docs_operation",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    vc_request_docs_operation_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_request_docs_operation_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_request_docs_series",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    int_doc_type = table.Column<long>(type: "bigint", nullable: true),
                    int_doc_year = table.Column<int>(type: "int", nullable: true),
                    vc_series_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    vc_request_doc_type_official_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_request_docs_series_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_speciality_curriculum_update_reason",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_speciality_curriculum_update_reason_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_speciality_vqs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_vqs_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    vc_vqs_short_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_speciality_vqs_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_stage_document_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_stage_document_types_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_stage_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    ref_procedure_step_stage_id = table.Column<long>(type: "bigint", nullable: true),
                    type_id = table.Column<long>(type: "bigint", nullable: true),
                    can_be_more_than_one = table.Column<bool>(type: "bit", nullable: true),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    iorder = table.Column<long>(type: "bigint", nullable: true),
                    has_signed_copy = table.Column<bool>(type: "bit", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true),
                    mnem_code = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    parent = table.Column<int>(type: "int", nullable: true),
                    uploadbyexpert = table.Column<bool>(type: "bit", nullable: true),
                    e_delivery = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_stage_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_tcontract_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_tcontract_type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_tcontract_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_tqualification_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_tqualification_type_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_tqualification_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_training_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_training_type_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_group_mtb = table.Column<bool>(type: "bit", nullable: true),
                    bool_group_trainer = table.Column<bool>(type: "bit", nullable: true),
                    vc_section = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_training_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_ui_control_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_furia_name = table.Column<string>(type: "nvarchar(90)", maxLength: 90, nullable: true),
                    vc_please_text = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_ui_control_type_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_upload_doc_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_doc_status_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_upload_doc_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_upload_doc_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_doc_type_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    bool_year_dependent = table.Column<bool>(type: "bit", nullable: false),
                    vc_doc_type_name_short = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bool_for_cpo = table.Column<bool>(type: "bit", nullable: true),
                    bool_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_upload_doc_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_validity_check_target",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false),
                    vc_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_validity_check_target_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_vet_area",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_list_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_area_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_vet_area_correction = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_correction_parent = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_area_correction_notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_vet_area_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_vet_area_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_vet_group",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_vet_list_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_group_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_vet_group_correction = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_correction_parent = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_group_correction_notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_vet_group_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_vet_group_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_vet_list",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_vet_list_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_vet_list_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_vet_profession",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_list_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_profession_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_profession_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_vet_profession_correction = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_profession_correction_parent = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_profession_correction_notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_vet_profession_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_vet_profession_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_vet_speciality",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_vet_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_list_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_speciality_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_speciality_vqs = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_vet_speciality_correction = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_correction_parent = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_speciality_correction_notes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_start_date_event = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date_event = table.Column<DateTime>(type: "date", nullable: true),
                    vc_vet_speciality_name_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_vet_speciality_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_village_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_village_type_name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    vc_village_type_short_name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_village_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_visit_result",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_visit_result_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_visit_result_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "code_wgdoi_function",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_wgdoi_function_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("code_wgdoi_function_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "iscs_iisclientdata",
                columns: table => new
                {
                    iis_server_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    iis_server_url = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    iis_server_key = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValueSql: "('0')"),
                    iis_cdata = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))"),
                    iis_last_request = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true, defaultValueSql: "('0')"),
                    iis_data = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, defaultValueSql: "('0')")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "iscs_iisserverdata",
                columns: table => new
                {
                    iis_client = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    iis_client0 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    iis_last_request = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true, defaultValueSql: "('0')"),
                    iis_last_crequest = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: true, defaultValueSql: "('0')"),
                    iis_last_cdata = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true, defaultValueSql: "('0')"),
                    iis_data = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, defaultValueSql: "('0')")
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "mv_register_clients",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_client_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_egn_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_type_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_vet_area_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_area_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_vet_profession_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_profession_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_vet_speciality_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_vet_speciality_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_speciality_vqs = table.Column<long>(type: "bigint", nullable: true),
                    vc_speciality_vqs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_finished_year = table.Column<int>(type: "int", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_clients_courses_id = table.Column<long>(type: "bigint", nullable: true),
                    document_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_obl_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_municipality_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_ekkate_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_ekkate_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_expert_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_arch_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_commission_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_expert_commissions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_experts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_arch_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_experts_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_procedure_expert_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_commission_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_procedure_expert_commissions_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_procedure_experts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_procedure_experts_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_procedure_procedures_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_procedures_documents_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_procedure_procedures_documents_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_procedure_provider_premises_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_speciality_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_speciality_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_speciality_correspondence = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_procedure_provider_premises_specialities_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_arch_procedures_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_arch_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_procedures_documents_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_arch_procedures_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_candidate_provider_premises_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_speciality_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_speciality_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_speciality_correspondence = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_candidate_provider_premises_specialities_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_candidates_expert_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_commission_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_candidates_expert_commissions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_candidates_experts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_candidates_experts_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_candidates_procedures_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_procedures_documents_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_candidates_procedures_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_cg_curric_files",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_course_id = table.Column<long>(type: "bigint", nullable: false),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_specialities_curriculum_id = table.Column<long>(type: "bigint", nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_cg_curric_files_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_clients_courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_second_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_family_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cfinished_type_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_course_finished = table.Column<DateTime>(type: "date", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_gender = table.Column<int>(type: "int", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<int>(type: "int", nullable: true),
                    dt_client_birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qual_level = table.Column<long>(type: "bigint", nullable: true),
                    int_qual_vet_area = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_clients_courses_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_clients_courses_documents_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: false),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group40_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_courses_documents_id = table.Column<long>(type: "bigint", nullable: false),
                    int_document_status = table.Column<long>(type: "bigint", nullable: false),
                    vc_note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: false),
                    dt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    int_sign_content_id = table.Column<long>(type: "bigint", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_clients_courses_documents_status_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_course_document_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_code_course_type = table.Column<long>(type: "bigint", nullable: true),
                    int_code_document_type = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_course_document_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_course_group_premises",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_premise_id = table.Column<long>(type: "bigint", nullable: false),
                    int_training_type_id = table.Column<long>(type: "bigint", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_course_group_premises_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_course_type_fr_curr",
                columns: table => new
                {
                    int_code_course_type_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_code_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: false),
                    bool_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_course_type_fr_curr_pkey", x => new { x.int_code_course_type_id, x.int_code_course_fr_curr_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_doctype_reqdoctype",
                columns: table => new
                {
                    int_doc_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_request_doc_type_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ref_document_status_locks",
                columns: table => new
                {
                    int_document_status_id = table.Column<long>(type: "bigint", nullable: false),
                    int_code_document_status_locks_id = table.Column<int>(type: "int", nullable: false),
                    int_code_validity_check_target_id = table.Column<int>(type: "int", nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_document_status_locks_pk", x => new { x.int_document_status_id, x.int_code_document_status_locks_id, x.int_code_validity_check_target_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_experts_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_exp_comm_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_chairman = table.Column<bool>(type: "bit", nullable: true),
                    int_commission_institution_type = table.Column<long>(type: "bigint", nullable: true),
                    vc_expert_institution = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_occupation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_protokol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dt_expert_protokol_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_experts_commissions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_experts_doi",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_doi_comm_id = table.Column<long>(type: "bigint", nullable: true),
                    int_wgdoi_function_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_experts_doi_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_experts_types",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_position = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_experts_types_id_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_experts_vet_area",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_experts_vet_area_id_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_fr_curr_ed_form",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_code_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: false),
                    int_code_course_ed_form = table.Column<long>(type: "bigint", nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_fr_curr_ed_form_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_fr_curr_educ_level",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_code_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: false),
                    int_code_education = table.Column<long>(type: "bigint", nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_fr_curr_educ_level_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_fr_curr_qual_level",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_code_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: false),
                    int_code_qual_level = table.Column<long>(type: "bigint", nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_fr_curr_qual_level_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_main_expert_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_commission_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_main_expert_commissions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_main_experts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_main_experts_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_procedure_step_stages",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    stage_id = table.Column<long>(type: "bigint", nullable: true),
                    label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    iorder = table.Column<long>(type: "bigint", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_procedure_step_stages_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_procedure_steps",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    label = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    iorder = table.Column<long>(type: "bigint", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true),
                    label_en = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    label_reg = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    label_reg_en = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_procedure_steps_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_procedures_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_procedures_documents_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_procedures_documents_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_procedures_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_provider_premises_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_speciality_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_speciality_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_speciality_correspondence = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_provider_premises_specialities_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_request_doc_status",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<int>(type: "int", nullable: true),
                    int_request_doc_status_id = table.Column<int>(type: "int", nullable: true),
                    int_request_id = table.Column<int>(type: "int", nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_request_doc_status_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_request_doc_type",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<int>(type: "int", nullable: true),
                    int_request_doc_type_id = table.Column<int>(type: "int", nullable: true),
                    int_request_id = table.Column<int>(type: "int", nullable: true),
                    int_doc_count = table.Column<int>(type: "int", nullable: true),
                    int_napoo_request_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_request_doc_type_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_role_acl_actions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_role_id = table.Column<long>(type: "bigint", nullable: true),
                    int_acl_action_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_role_acl_actions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_role_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_role_id = table.Column<long>(type: "bigint", nullable: true),
                    int_group_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_role_groups_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_role_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_role_id = table.Column<long>(type: "bigint", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_role_users_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_trainers_courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_training_type_id = table.Column<long>(type: "bigint", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_trainers_courses_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_vet_specialities_nkpds",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nkpd_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_vet_specialities_nkpds_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_visits",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_visit_no = table.Column<long>(type: "bigint", nullable: true),
                    int_visit_result_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_visit_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_visit_theme = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_visit_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_visit_prot_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_visit_prot_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_visits_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "ref_visits_experts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_visit_id = table.Column<long>(type: "bigint", nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ref_visits_experts_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "report_clients",
                columns: table => new
                {
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_gender = table.Column<int>(type: "int", nullable: true),
                    int_birth_year = table.Column<int>(type: "int", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_qualification_level = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nuts_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_measure_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_ed_form_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_duration = table.Column<int>(type: "int", nullable: true),
                    num_course_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    int_cfinished_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_status = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "report_courses",
                columns: table => new
                {
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_qualification_level = table.Column<long>(type: "bigint", nullable: true),
                    int_course_measure_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nuts_id = table.Column<long>(type: "bigint", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_ed_form_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_duration = table.Column<int>(type: "int", nullable: true),
                    num_course_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    int_course_status_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "report_curricula",
                columns: table => new
                {
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nuts_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_qualification_level = table.Column<long>(type: "bigint", nullable: true),
                    int_speciality_curriculum_update_reason_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "report_premises",
                columns: table => new
                {
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_no = table.Column<int>(type: "int", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nuts_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_status = table.Column<long>(type: "bigint", nullable: true),
                    int_speciality_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_speciality_correspondence = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "report_providers",
                columns: table => new
                {
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nuts_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_num_courses_a = table.Column<long>(type: "bigint", nullable: true),
                    int_num_courses_b = table.Column<long>(type: "bigint", nullable: true),
                    int_num_clients_a = table.Column<long>(type: "bigint", nullable: true),
                    int_num_clients_b = table.Column<long>(type: "bigint", nullable: true),
                    int_num_clients_c = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "report_trainers_qualification",
                columns: table => new
                {
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_birth_year = table.Column<int>(type: "int", nullable: true),
                    int_gender_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_tcontract_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_municipality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_obl_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nuts_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_tqualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qualification_duration = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "ret",
                columns: table => new
                {
                    column = table.Column<bool>(name: "?column?", type: "bit", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "sys_acl",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_object_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_item_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_acl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sys_acl_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "sys_locks",
                columns: table => new
                {
                    session_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    lock_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "sys_mail_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_mail_type = table.Column<int>(type: "int", nullable: true),
                    int_user_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_id = table.Column<int>(type: "int", nullable: true),
                    vc_mail_text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dt_mail_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sys_mail_log_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "sys_operation_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dt_date_time = table.Column<DateTime>(type: "datetime2", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: true),
                    int_operation_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_additional_info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("sys_operation_log_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "sys_sign_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_user_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_id_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group = table.Column<long>(type: "bigint", nullable: true),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_cert_valid_from = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_cert_valid_to = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_cert_email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    vc_error = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    dt_event = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    vc_cert = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "systransliterate",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: true),
                    vcletterbg = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    vcletterlat = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tb_acl_actions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_action_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_acl_actions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_annual_info",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_year = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_position = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_phone = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_timestamp = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    int_status = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_annual_info_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_provider_premises",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_provider_premise_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_ekatte = table.Column<long>(type: "bigint", nullable: true),
                    txt_provider_premise_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_status = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_visited = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_provider_premises_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_provider_premises_rooms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_room_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_room_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_type = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_area = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_room_workplaces = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_room_equipment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_provider_premises_rooms_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_provider_specialities_curriculum",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_procedure_provider_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_updated = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    dt_update_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_speciality_curriculum_update_reason_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    oid_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_provider_specialities_curriculum_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_providers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_procedure_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_status_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_local_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_zip_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_web = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_contact_pers_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_contact_pers_zipcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_contact_pers_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_operation_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_brra = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    vc_provider_profile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_filing_system_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_filing_system_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_licence_data = table.Column<DateTime>(type: "date", nullable: true),
                    int_licence_prot_no = table.Column<int>(type: "int", nullable: true),
                    vc_provider_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_started_procedure_progress = table.Column<long>(type: "bigint", nullable: true),
                    procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    stage_id = table.Column<long>(type: "bigint", nullable: true),
                    is_returned = table.Column<bool>(type: "bit", nullable: true),
                    int_receive_type_id = table.Column<long>(type: "bigint", nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true),
                    int_provider_registration_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_cpo_management_version = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_providers_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_providers_cipo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cipo_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_cipo_management_notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_providers_cipo_management_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_providers_cpo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cpo_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_cpo_management_notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_providers_cpo_management_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_providers_documents_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_documents_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_documents_management_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_documents_management_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ts_document = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_providers_documents_management_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_providers_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_added = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    txt_speciality_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_approved = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_providers_specialities_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_trainer_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_documents_management_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_documents_management_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_trainer_documents_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_trainer_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_vet_area_qualified = table.Column<bool>(type: "bit", nullable: true),
                    bool_vet_area_theory = table.Column<bool>(type: "bit", nullable: true),
                    bool_vet_area_practice = table.Column<bool>(type: "bit", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_trainer_profiles_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_trainer_qualifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_qualification_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_qualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_tqualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qualification_duration = table.Column<int>(type: "int", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_trainer_qualifications_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_procedure_trainers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_started_procedure_id = table.Column<long>(type: "bigint", nullable: false),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_trainer_first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_trainer_second_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_trainer_family_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_birth_year = table.Column<int>(type: "int", nullable: true),
                    int_gender_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_education_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_certificate_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_academic_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_andragog = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_tcontract_type_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_tcontract_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_procedure_trainers_pkey", x => new { x.id, x.int_started_procedure_id })
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_arch_providers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_decision_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    int_provider_no = table.Column<long>(type: "bigint", nullable: true),
                    int_candidate_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_status_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_local_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_zip_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_web = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_contact_pers_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_contact_pers_zipcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_contact_pers_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_operation_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_arch_providers_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_provider_premises",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_provider_premise_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_ekatte = table.Column<long>(type: "bigint", nullable: true),
                    txt_provider_premise_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_status = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_visited = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_provider_premises_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_provider_premises_rooms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_room_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_room_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_type = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_area = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_room_workplaces = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_room_equipment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_provider_premises_rooms_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_provider_specialities_curriculum",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_candidate_provider_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_updated = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    dt_update_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_speciality_curriculum_update_reason_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    oid_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_provider_specialities_curriculum_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_providers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_candidate_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_status_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_local_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_zip_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_web = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_contact_pers_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_contact_pers_zipcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_contact_pers_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    int_operation_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_brra = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    vc_provider_profile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_filing_system_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_filing_system_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_licence_data = table.Column<DateTime>(type: "date", nullable: true),
                    int_licence_prot_no = table.Column<int>(type: "int", nullable: true),
                    vc_provider_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_started_procedures = table.Column<long>(type: "bigint", nullable: true),
                    int_started_procedure_progress = table.Column<long>(type: "bigint", nullable: true),
                    procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    stage_id = table.Column<long>(type: "bigint", nullable: true),
                    is_returned = table.Column<bool>(type: "bit", nullable: true),
                    int_receive_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_registration_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_cpo_management_version = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_providers_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_providers_cipo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cipo_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_cipo_management_notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_providers_cipo_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_providers_cpo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cpo_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_cpo_management_notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_providers_cpo_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_providers_documents_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_documents_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_documents_management_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_documents_management_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ts_document = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_providers_documents_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_providers_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_added = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    txt_speciality_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_approved = table.Column<bool>(type: "bit", nullable: true),
                    int_vet_spec_replaced = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_providers_specialities_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_providers_state",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_candidate_providers_state_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_candidate_providers_state_change = table.Column<DateTime>(type: "date", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_providers_state_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_trainer_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_documents_management_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_documents_management_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_trainer_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_trainer_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_vet_area_qualified = table.Column<bool>(type: "bit", nullable: true),
                    bool_vet_area_theory = table.Column<bool>(type: "bit", nullable: true),
                    bool_vet_area_practice = table.Column<bool>(type: "bit", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_trainer_profiles_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_trainer_qualifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_qualification_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_qualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_tqualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qualification_duration = table.Column<int>(type: "int", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_trainer_qualifications_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_candidate_trainers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_trainer_first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_trainer_second_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_trainer_family_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_birth_year = table.Column<int>(type: "int", nullable: true),
                    int_gender_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_education_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_certificate_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_academic_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_andragog = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_tcontract_type_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_tcontract_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_candidate_trainers_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_clients",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_client_first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_client_second_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_client_family_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_gender = table.Column<int>(type: "int", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_client_birth_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_clients_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_clients_courses_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_clients_courses_id = table.Column<long>(type: "bigint", nullable: true),
                    int_document_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_finished_year = table.Column<int>(type: "int", nullable: true),
                    vc_document_prn_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_document_reg_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dt_document_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_document_prot = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    num_theory_result = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    num_practice_result = table.Column<decimal>(type: "numeric(3,2)", nullable: true),
                    vc_qualification_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_qualificatioj_level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    document_1_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    document_2_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_document_prn_ser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    int_document_status = table.Column<long>(type: "bigint", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_clients_courses_documents_id", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_clients_required_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_code_qual_level_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_education_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_course_group_required_documents_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_desciption = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_document_date = table.Column<DateTime>(type: "date", nullable: true),
                    oid_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_document_reg_no = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_document_prn_no = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    dt_document_official_date = table.Column<DateTime>(type: "date", nullable: true),
                    bool_before_date = table.Column<bool>(type: "bit", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true),
                    int_code_ext_register_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tb_course_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_course_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_course_subscribe_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_additional_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_measure_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_group_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_course_status_id = table.Column<long>(type: "bigint", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_ed_form_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_assign_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_course_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_duration = table.Column<int>(type: "int", nullable: true),
                    num_course_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_theory_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_practice_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_exam_comm_members = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_selectable_hours = table.Column<int>(type: "int", nullable: true),
                    int_mandatory_hours = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_p_disability_count = table.Column<int>(type: "int", nullable: true),
                    bool_is_archived = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_course_groups_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_course_groups_40",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise = table.Column<long>(type: "bigint", nullable: true),
                    int_course_ed_form_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_duration = table.Column<int>(type: "int", nullable: true),
                    int_selectable_hours = table.Column<int>(type: "int", nullable: true),
                    int_mandatory_hours = table.Column<int>(type: "int", nullable: true),
                    num_course_cost = table.Column<decimal>(type: "numeric(10,2)", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_theory_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_exam_practice_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_exam_comm_members = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_additional_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_assign_type_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_course_groups_40_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_course_groups_duplicates",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_document_type_id = table.Column<long>(type: "bigint", nullable: false),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_end_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_course_finished_year = table.Column<int>(type: "int", nullable: true),
                    vc_original_prn_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_original_reg_no = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    dt_original_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_original_ref_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_original_document_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_course_groups_duplicates_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_course40_competences",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_client_id = table.Column<long>(type: "bigint", nullable: false),
                    vc_competence = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_course40_competences_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_courses",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_course_no = table.Column<long>(type: "bigint", nullable: true),
                    vc_course_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_course_add_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_course_educ_requirement = table.Column<int>(type: "int", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_fr_curr_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_mandatory_hours = table.Column<int>(type: "int", nullable: true),
                    int_selectable_hours = table.Column<int>(type: "int", nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_courses_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_curric_modules",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_course_id = table.Column<long>(type: "bigint", nullable: false),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_module_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    int_hours = table.Column<long>(type: "bigint", nullable: false),
                    int_curric_hours_type = table.Column<long>(type: "bigint", nullable: false),
                    int_training_type = table.Column<long>(type: "bigint", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_curric_order = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((10))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_curric_modules_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_doi",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_doi_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_doi_regualtion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_general_descr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_job_profile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_educ_objectives = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_lrng_outcomes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_mtb_updates = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_pdf_path = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    b_submitted = table.Column<bool>(type: "bit", nullable: true),
                    dt_submitted_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    vc_comment = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_doi_commission = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_certificate_supplement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_doi_job_career = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_search = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_doi_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_doi_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_doi_comm_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_doi_commissions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_e_signers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_id_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_names = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    vc_description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    dt_from = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    dt_to = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "tb_expert_commissions",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_exp_comm_name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_expert_commissions_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_experts",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_expert_first_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_second_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_family_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_expert_gender_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_birth_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_inception_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_inception_order = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    int_expert_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_expert_zipcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_expert_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_email1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_expert_email2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_expert_user_id = table.Column<long>(type: "bigint", nullable: true),
                    int_comm_member_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_expert_occupation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_education = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_deleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_experts_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_generated_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_code_procedures_documents_id = table.Column<long>(type: "bigint", nullable: true),
                    oid_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_generated_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_groups",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_short_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_group_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_group_type = table.Column<int>(type: "int", nullable: true),
                    pid = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_groups_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_import_xml",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    file_name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_id = table.Column<long>(type: "bigint", nullable: true),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    file_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_import_xml_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_napoo_request_doc",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dt_request_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_request_year = table.Column<int>(type: "int", nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true),
                    bool_is_sent = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_napoo_request_doc_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_payments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: false),
                    int_started_procedures_id = table.Column<long>(type: "bigint", nullable: false),
                    int_procedure_price_id = table.Column<int>(type: "int", nullable: false),
                    int_specialities_count = table.Column<int>(type: "int", nullable: true),
                    int_sume = table.Column<int>(type: "int", nullable: false),
                    vc_text = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: true),
                    int_status = table.Column<int>(type: "int", nullable: true),
                    ts_gen = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    int_transaction_id = table.Column<long>(type: "bigint", nullable: true),
                    ts_payed = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_payments_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_procedure_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    started_procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    oid_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_valid = table.Column<bool>(type: "bit", nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true),
                    stage_document_id = table.Column<long>(type: "bigint", nullable: true),
                    provider_id = table.Column<long>(type: "bigint", nullable: true),
                    mime_type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    extension = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    filename = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    uin = table.Column<long>(type: "bigint", nullable: true),
                    ds_date = table.Column<DateTime>(type: "date", nullable: true),
                    ds_id = table.Column<long>(type: "bigint", nullable: true),
                    ds_official_id = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ds_official_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ds_official_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ds_prep = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_expert_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_procedure_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_procedure_prices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_name = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    int_price = table.Column<int>(type: "int", nullable: true),
                    int_pocedure_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_count_dependant = table.Column<bool>(type: "bit", nullable: true),
                    int_count_min = table.Column<int>(type: "int", nullable: true),
                    int_count_max = table.Column<int>(type: "int", nullable: true),
                    bool_cpo = table.Column<bool>(type: "bit", nullable: true),
                    bool_cipo = table.Column<bool>(type: "bit", nullable: true),
                    dt_from = table.Column<DateTime>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    dt_to = table.Column<DateTime>(type: "date", nullable: true),
                    int_payment_period = table.Column<int>(type: "int", nullable: true),
                    int_procedure_steps_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_procedure_prices_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_procedure_progress",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_procedure_decision_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_procedure_number = table.Column<long>(type: "bigint", nullable: true),
                    dt_decision_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_decision_protocol_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_system_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_procedure_progress_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_procedure_progress_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_procedure_progress_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_document_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_document_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_procedure_progress_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_provider_activities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_current_year = table.Column<int>(type: "int", nullable: true),
                    txt_provider_activities = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_provider_activities_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_provider_premises",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_provider_premise_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_ekatte = table.Column<long>(type: "bigint", nullable: true),
                    txt_provider_premise_address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_status = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_visited = table.Column<bool>(type: "bit", nullable: true),
                    int_preaz = table.Column<int>(type: "int", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_provider_premises_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_provider_premises_rooms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_premise_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_no = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_room_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_provider_premise_room_usage = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_type = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_premise_room_area = table.Column<int>(type: "int", nullable: true),
                    int_provider_premise_room_workplaces = table.Column<int>(type: "int", nullable: true),
                    txt_provider_premise_room_equipment = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_provider_premises_rooms_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_provider_specialities",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_licence_data = table.Column<DateTime>(type: "date", nullable: true),
                    int_licence_prot_no = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    txt_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_added = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    bool_is_valid = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    txt_speciality_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_provider_specialities_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_provider_specialities_curriculum",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_speciality_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_updated = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    dt_update_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_speciality_curriculum_update_reason_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_file_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    oid_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_provider_specialities_curriculum_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_provider_uploaded_docs",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_upload_doc_type_id = table.Column<int>(type: "int", nullable: true),
                    int_year = table.Column<long>(type: "bigint", nullable: true),
                    txt_doc_description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_doc_status_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_doc_upload_date = table.Column<DateTime>(type: "date", nullable: true),
                    txt_file_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_file_type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bin_file = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_provider_uploaded_docs_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_status_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_status_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_number = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_local_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_zip_code = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_web = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_contact_pers_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_contact_pers_zipcode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    vc_provider_contact_pers_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_contact_pers_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_ownership_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_is_brra = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))"),
                    vc_provider_profile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_provider_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_licence_data = table.Column<DateTime>(type: "date", nullable: true),
                    int_provider_registration_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_cipo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cipo_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_cipo_management_notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_cipo_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_cpo_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_cpo_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_cpo_management_notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_cpo_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_docs_dashboard",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_doc_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_docs_year = table.Column<int>(type: "int", nullable: true),
                    int_cnt_recv = table.Column<int>(type: "int", nullable: true),
                    int_cnt_sent = table.Column<int>(type: "int", nullable: true),
                    int_cnt_prnt = table.Column<int>(type: "int", nullable: true),
                    int_cnt_null = table.Column<int>(type: "int", nullable: true),
                    int_cnt_dstr = table.Column<int>(type: "int", nullable: true),
                    int_cnt_avlb = table.Column<int>(type: "int", nullable: true),
                    bool_has_serial_number = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_docs_dashboard_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_docs_offers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_seek_offer = table.Column<int>(type: "int", nullable: true),
                    int_doc_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_count_offered = table.Column<int>(type: "int", nullable: true),
                    dt_offered = table.Column<DateTime>(type: "date", nullable: true),
                    dt_closed = table.Column<DateTime>(type: "date", nullable: true),
                    bool_offer_valid = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_docs_offers_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_documents_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_documents_management_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_documents_management_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_documents_management_file = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ts_document = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_documents_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_licence_change",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_licence_status_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_change_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_number_command = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_licence_status_details_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_insert_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_licence_change_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_providers_request_doc",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<int>(type: "int", nullable: true),
                    int_current_year = table.Column<int>(type: "int", nullable: true),
                    dt_request_doc = table.Column<DateTime>(type: "date", nullable: true),
                    vc_position = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    vc_name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    vc_address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    vc_telephone = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true),
                    oid_request_pdf = table.Column<long>(type: "int", nullable: true),
                    int_napoo_request_id = table.Column<int>(type: "int", nullable: true),
                    bool_is_sent = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((0))")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_providers_request_doc_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_request_docs_management",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_request_id = table.Column<int>(type: "int", nullable: true),
                    int_napoo_request_id = table.Column<int>(type: "int", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: false),
                    int_partner_id = table.Column<long>(type: "bigint", nullable: true),
                    int_request_doc_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_receive_docs_year = table.Column<int>(type: "int", nullable: true),
                    int_receive_docs_count = table.Column<int>(type: "int", nullable: true),
                    dt_receive_docs_date = table.Column<DateTime>(type: "date", nullable: true),
                    int_request_docs_operation_id = table.Column<int>(type: "int", nullable: true),
                    vc_tb_provider_uploaded_docs_ids = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    vc_orig_tb_cl_cour_docs = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_request_docs_management_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_request_docs_sn",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_request_docs_management_id = table.Column<long>(type: "bigint", nullable: true),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    int_request_doc_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_receive_docs_year = table.Column<int>(type: "int", nullable: true),
                    vc_request_doc_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    int_request_docs_operation_id = table.Column<int>(type: "int", nullable: true),
                    bool_chk_fabn = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_request_docs_sn_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_roles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    vc_role_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_roles_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_sign_content",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: false),
                    int_course_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_client_id = table.Column<long>(type: "bigint", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: false),
                    vc_content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    vc_signed_content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    int_document_status = table.Column<long>(type: "bigint", nullable: false),
                    dttimestamp = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    signer_egn = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    signer_bulstat = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    subject_email_address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    subject_serial_number = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    subject_cn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    subject_c = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    subject_e = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    subject_ou = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    issuer_o = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    serial_number = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    valid_from = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    valid_to = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    extensions_subject_alt_name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_sign_content_pk", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_started_procedure_progress",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    started_procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    user_id = table.Column<long>(type: "bigint", nullable: true),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    stage_id = table.Column<long>(type: "bigint", nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_started_procedure_progress_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_started_procedures",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    provider_id = table.Column<long>(type: "bigint", nullable: true),
                    procedure_id = table.Column<long>(type: "bigint", nullable: true),
                    ts = table.Column<DateTime>(type: "datetime2", nullable: true),
                    int_candidate_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_owner = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_provider_bulstat = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    int_ekatte_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_provider_phone1 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_phone2 = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_fax = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_web = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_provider_manager = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_legal_book_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_legal_book_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_license_expertise_order_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_license_expertise_order_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_negative_opinion_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_negative_opinion_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_denied_license_order_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_denied_license_order_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_issued_license_order_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_issued_license_order_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_summarized_report_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_summarized_report_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_meeting_protocol_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_meeting_protocol_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_chairman_report_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_chairman_report_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_license_expertise_mail_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_license_expertise_mail_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_issues_mail_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_issues_mail_number = table.Column<DateTime>(type: "date", nullable: true),
                    vc_license_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_license_date = table.Column<DateTime>(type: "date", nullable: true),
                    dt_napoo_report_deadline = table.Column<DateTime>(type: "date", nullable: true),
                    dt_summarized_report_deadline = table.Column<DateTime>(type: "date", nullable: true),
                    dt_report_review_deadline = table.Column<DateTime>(type: "date", nullable: true),
                    vc_licensing_mail_outgoing_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_negative_issues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dt_negative_deadline = table.Column<DateTime>(type: "date", nullable: true),
                    vc_denied_mail_number = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    vc_meeting_hour = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    dt_meeting_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_negative_needed_documents = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    vc_negative_reasons = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_receive_documents_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_expert_report_deadline = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_started_procedures_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_survey_answer",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_question_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_answer_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_user_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_timestamp = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_survey_answer_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_survey_question",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_question_type = table.Column<int>(type: "int", nullable: true),
                    vc_question_text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_order_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_question_active = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_survey_question_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_trainer_documents",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_documents_management_title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_documents_management_file = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_trainer_documents_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_trainer_profiles",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    int_vet_area_id = table.Column<long>(type: "bigint", nullable: true),
                    bool_vet_area_qualified = table.Column<bool>(type: "bit", nullable: true),
                    bool_vet_area_theory = table.Column<bool>(type: "bit", nullable: true),
                    bool_vet_area_practice = table.Column<bool>(type: "bit", nullable: true),
                    int_vet_speciality_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_trainer_profiles_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_trainer_qualifications",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_trainer_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_qualification_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    int_qualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_profession_id = table.Column<long>(type: "bigint", nullable: true),
                    int_tqualification_type_id = table.Column<long>(type: "bigint", nullable: true),
                    int_qualification_duration = table.Column<int>(type: "int", nullable: true),
                    dt_start_date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_trainer_qualifications_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_trainers",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    int_provider_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_egn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_egn_type_id = table.Column<long>(type: "bigint", nullable: true),
                    vc_trainer_first_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_trainer_second_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    vc_trainer_family_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    int_birth_year = table.Column<int>(type: "int", nullable: true),
                    int_gender_id = table.Column<long>(type: "bigint", nullable: true),
                    int_nationality_id = table.Column<long>(type: "bigint", nullable: true),
                    int_education_id = table.Column<long>(type: "bigint", nullable: true),
                    txt_education_speciality_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_certificate_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    txt_education_academic_notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bool_is_andragog = table.Column<bool>(type: "bit", nullable: true, defaultValueSql: "((1))"),
                    int_tcontract_type_id = table.Column<long>(type: "bigint", nullable: true),
                    dt_tcontract_date = table.Column<DateTime>(type: "date", nullable: true),
                    vc_email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_trainers_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_user_press",
                columns: table => new
                {
                    int_id = table.Column<int>(type: "int", nullable: false),
                    vc_user = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_pass = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_obl = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    vc_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_user_press_pk", x => x.int_id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_users",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    vc_fullname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    int_global_group_id = table.Column<long>(type: "bigint", nullable: true),
                    int_local_group_id = table.Column<long>(type: "bigint", nullable: true),
                    upwd = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    unhs = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    udat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    adat = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_users_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "tb_version",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    vc_comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    dt_timestamp = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("tb_version_pkey", x => x.id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "arch_int_course_group_id_int_year",
                table: "arch_ref_clients_courses",
                column: "int_course_group_id");

            migrationBuilder.CreateIndex(
                name: "id_int_year",
                table: "arch_ref_clients_courses",
                columns: new[] { "id", "int_year" });

            migrationBuilder.CreateIndex(
                name: "int_client_id_int_year_idx",
                table: "arch_ref_clients_courses",
                columns: new[] { "int_client_id", "int_year" });

            migrationBuilder.CreateIndex(
                name: "arch_clients_int_provider_egn_key",
                table: "arch_tb_clients",
                columns: new[] { "int_provider_id", "vc_egn", "int_year" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL AND [int_year] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "id_int_year_idx",
                table: "arch_tb_clients",
                columns: new[] { "id", "int_year" });

            migrationBuilder.CreateIndex(
                name: "int_client_courses_id_int_year_idx",
                table: "arch_tb_clients_courses_documents",
                columns: new[] { "int_clients_courses_id", "int_year" });

            migrationBuilder.CreateIndex(
                name: "arch_tb_course_groups_id_int_year_idx",
                table: "arch_tb_course_groups",
                columns: new[] { "id", "int_year" });

            migrationBuilder.CreateIndex(
                name: "int_course_id_idx",
                table: "arch_tb_course_groups",
                column: "int_course_id");

            migrationBuilder.CreateIndex(
                name: "i_arch_tb_courses_int_year",
                table: "arch_tb_courses",
                column: "int_year");

            migrationBuilder.CreateIndex(
                name: "arch_tb_providers_int_provider_bulstat_key",
                table: "arch_tb_providers",
                columns: new[] { "int_provider_bulstat", "int_year" },
                unique: true,
                filter: "([int_provider_bulstat] IS NOT NULL AND [int_year] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "arch_tb_trainers_int_provider_egn_key",
                table: "arch_tb_trainers",
                columns: new[] { "int_provider_id", "vc_egn", "int_year" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL AND [int_year] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ndx_code_assign_type",
                table: "code_assign_type",
                column: "vc_assign_type_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_ccontract_type",
                table: "code_ccontract_type",
                column: "vc_ccontract_type_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_cfinished_type",
                table: "code_cfinished_type",
                column: "vc_cfinished_type_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_course_ed_form",
                table: "code_course_ed_form",
                column: "vc_course_ed_form_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_course_fr_curr",
                table: "code_course_fr_curr",
                column: "vc_course_fr_curr_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_course_measure_type",
                table: "code_course_measure_type",
                column: "vc_course_measure_type_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_course_status",
                table: "code_course_status",
                column: "vc_course_status_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_course_type",
                table: "code_course_type",
                column: "vc_course_type_name");

            migrationBuilder.CreateIndex(
                name: "code_document_validity_checks_uk",
                table: "code_document_validity_checks",
                column: "id",
                unique: true,
                filter: "([id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ndx_code_education",
                table: "code_education",
                column: "vc_education_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_ekatte",
                table: "code_ekatte",
                column: "vc_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_employment_status",
                table: "code_employment_status",
                column: "vc_employment_status_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_gender",
                table: "code_gender",
                column: "vc_gender");

            migrationBuilder.CreateIndex(
                name: "ndx_code_licence_status",
                table: "code_licence_status",
                column: "vc_licence_status_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_municipality",
                table: "code_municipality",
                column: "vc_municipality_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_municipality_details",
                table: "code_municipality_details",
                column: "vc_municipality_details_name");

            migrationBuilder.CreateIndex(
                name: "unique_int_nkpd_id",
                table: "code_nkpd",
                columns: new[] { "int_nkpd_id1", "int_nkpd_id2" },
                unique: true,
                filter: "([int_nkpd_id1] IS NOT NULL AND [int_nkpd_id2] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ndx_code_obl",
                table: "code_obl",
                column: "vc_obl_name");

            migrationBuilder.CreateIndex(
                name: "unique_vc_name",
                table: "code_operation",
                column: "vc_name",
                unique: true,
                filter: "([vc_name] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ndx_code_provider_status",
                table: "code_provider_status",
                column: "vc_provider_status_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_tcontract_type",
                table: "code_tcontract_type",
                column: "vc_tcontract_type_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_vet_area",
                table: "code_vet_area",
                column: "int_vet_area_number");

            migrationBuilder.CreateIndex(
                name: "ndx_code_vet_group",
                table: "code_vet_group",
                column: "vc_vet_group_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_vet_list",
                table: "code_vet_list",
                column: "vc_vet_list_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_vet_profession",
                table: "code_vet_profession",
                column: "vc_vet_profession_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_vet_speciality",
                table: "code_vet_speciality",
                column: "vc_vet_speciality_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_village_type",
                table: "code_village_type",
                column: "vc_village_type_name");

            migrationBuilder.CreateIndex(
                name: "ndx_code_visit_result",
                table: "code_visit_result",
                column: "vc_visit_result_name");

            migrationBuilder.CreateIndex(
                name: "ref_arch_expert_commissions_provider_commission_key",
                table: "ref_arch_expert_commissions",
                columns: new[] { "int_arch_provider_id", "int_expert_commission_id" },
                unique: true,
                filter: "([int_arch_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_arch_experts_provider_expert_key",
                table: "ref_arch_experts",
                columns: new[] { "int_arch_provider_id", "int_expert_id" },
                unique: true,
                filter: "([int_arch_provider_id] IS NOT NULL AND [int_expert_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_arch_procedure_expert_commissions_provider_commission_key",
                table: "ref_arch_procedure_expert_commissions",
                columns: new[] { "int_started_procedure_id", "int_provider_id", "int_expert_commission_id" },
                unique: true,
                filter: "([int_started_procedure_id] IS NOT NULL AND [int_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_arch_procedure_prov_premises_specialities_int_provider_prem",
                table: "ref_arch_procedure_provider_premises_specialities",
                columns: new[] { "int_provider_premise_id", "int_provider_speciality_id" },
                unique: true,
                filter: "([int_provider_premise_id] IS NOT NULL AND [int_provider_speciality_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_cand_prov_premises_specialities_int_provider_premise_id_key",
                table: "ref_candidate_provider_premises_specialities",
                columns: new[] { "int_provider_premise_id", "int_provider_speciality_id" },
                unique: true,
                filter: "([int_provider_premise_id] IS NOT NULL AND [int_provider_speciality_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_candidates_expert_commissions_provider_commission_key",
                table: "ref_candidates_expert_commissions",
                columns: new[] { "int_provider_id", "int_expert_commission_id" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_doctype_reqdoctype_uk",
                table: "ref_doctype_reqdoctype",
                columns: new[] { "int_request_doc_type_id", "int_doc_type_id" },
                unique: true,
                filter: "([int_request_doc_type_id] IS NOT NULL AND [int_doc_type_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_experts_commissions_int_expert_id_key",
                table: "ref_experts_commissions",
                columns: new[] { "int_expert_id", "int_exp_comm_id" },
                unique: true,
                filter: "([int_expert_id] IS NOT NULL AND [int_exp_comm_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_experts_doi_int_expert_id_key",
                table: "ref_experts_doi",
                columns: new[] { "int_expert_id", "int_doi_comm_id" },
                unique: true,
                filter: "([int_expert_id] IS NOT NULL AND [int_doi_comm_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_experts_types_int_expert_id_key",
                table: "ref_experts_types",
                columns: new[] { "int_expert_id", "int_expert_type_id" },
                unique: true,
                filter: "([int_expert_id] IS NOT NULL AND [int_expert_type_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_experts_vet_area_int_expert_id_key",
                table: "ref_experts_vet_area",
                columns: new[] { "int_expert_id", "int_vet_area_id" },
                unique: true,
                filter: "([int_expert_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_main_expert_commissions_provider_commission_key",
                table: "ref_main_expert_commissions",
                columns: new[] { "int_provider_id", "int_expert_commission_id" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [int_expert_commission_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_provider_premises_specialities_int_provider_premise_id_key",
                table: "ref_provider_premises_specialities",
                columns: new[] { "int_provider_premise_id", "int_provider_speciality_id" },
                unique: true,
                filter: "([int_provider_premise_id] IS NOT NULL AND [int_provider_speciality_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_request_doc_type_int_request_doc_type_id_int_request_i_key",
                table: "ref_request_doc_type",
                columns: new[] { "int_request_doc_type_id", "int_request_id" },
                unique: true,
                filter: "([int_request_doc_type_id] IS NOT NULL AND [int_request_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "unique_int_nkpd_speciality",
                table: "ref_vet_specialities_nkpds",
                columns: new[] { "int_vet_speciality_id", "int_nkpd_id" },
                unique: true,
                filter: "([int_vet_speciality_id] IS NOT NULL AND [int_nkpd_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "ref_visits_experts_int_visit_id_key",
                table: "ref_visits_experts",
                columns: new[] { "int_visit_id", "int_expert_id" },
                unique: true,
                filter: "([int_visit_id] IS NOT NULL AND [int_expert_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "i_sys_locks_lock_id",
                table: "sys_locks",
                column: "lock_id");

            migrationBuilder.CreateIndex(
                name: "i_sys_locks_session_id",
                table: "sys_locks",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "i_sys_locks_ts",
                table: "sys_locks",
                column: "ts");

            migrationBuilder.CreateIndex(
                name: "tb_annual_info_int_year_int_provider_id_key",
                table: "tb_annual_info",
                columns: new[] { "int_year", "int_provider_id" },
                unique: true,
                filter: "([int_year] IS NOT NULL AND [int_provider_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_arch_procedure_trainer_id_int_vet_speciality_id_key",
                table: "tb_arch_procedure_trainer_profiles",
                columns: new[] { "int_trainer_id", "int_vet_area_id", "int_vet_speciality_id" },
                unique: true,
                filter: "([int_trainer_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL AND [int_vet_speciality_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_arch_procedure_trainers_int_provider_egn_key",
                table: "tb_arch_procedure_trainers",
                columns: new[] { "int_provider_id", "vc_egn", "int_started_procedure_id" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL AND [int_started_procedure_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_arch_providers_int_provider_bulstat_key",
                table: "tb_arch_providers",
                column: "int_provider_bulstat",
                unique: true,
                filter: "([int_provider_bulstat] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_arch_providers_int_provider_no_key",
                table: "tb_arch_providers",
                columns: new[] { "int_provider_no", "dt_decision_date" },
                unique: true,
                filter: "([int_provider_no] IS NOT NULL AND [dt_decision_date] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_candidate_trainer_id_int_vet_speciality_id_key",
                table: "tb_candidate_trainer_profiles",
                columns: new[] { "int_trainer_id", "int_vet_area_id", "int_vet_speciality_id" },
                unique: true,
                filter: "([int_trainer_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL AND [int_vet_speciality_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_candidate_trainers_int_provider_egn_key",
                table: "tb_candidate_trainers",
                columns: new[] { "int_provider_id", "vc_egn" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_clients_int_provider_egn_key",
                table: "tb_clients",
                columns: new[] { "int_provider_id", "vc_egn" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "i_tb_course_groups_end_date",
                table: "tb_course_groups",
                column: "dt_end_date");

            migrationBuilder.CreateIndex(
                name: "i_tb_course_groups_start_date",
                table: "tb_course_groups",
                column: "dt_start_date");

            migrationBuilder.CreateIndex(
                name: "tb_course_groups_i1",
                table: "tb_course_groups",
                column: "int_course_id");

            migrationBuilder.CreateIndex(
                name: "tb_courses_i1",
                table: "tb_courses",
                columns: new[] { "int_provider_id", "int_vet_speciality_id" });

            migrationBuilder.CreateIndex(
                name: "unique_int_doi_id",
                table: "tb_doi",
                column: "int_doi_id",
                unique: true,
                filter: "([int_doi_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_e_signers_int_provider_id_int_user_id_vc_id_number_key",
                table: "tb_e_signers",
                columns: new[] { "int_provider_id", "int_user_id", "vc_id_number" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [int_user_id] IS NOT NULL AND [vc_id_number] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_procedure_progress_int_provider_id",
                table: "tb_procedure_progress",
                column: "int_provider_id");

            migrationBuilder.CreateIndex(
                name: "tb_providers_docs_dashboard_unique_key",
                table: "tb_providers_docs_dashboard",
                columns: new[] { "int_provider_id", "int_doc_type_id", "int_docs_year" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [int_doc_type_id] IS NOT NULL AND [int_docs_year] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "unique_provider_date",
                table: "tb_providers_licence_change",
                columns: new[] { "int_provider_id", "dt_change_date" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [dt_change_date] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_request_docs_management_unique_key",
                table: "tb_request_docs_management",
                columns: new[] { "int_provider_id", "int_partner_id", "int_request_id", "int_request_doc_type_id", "int_receive_docs_year", "dt_receive_docs_date" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [int_partner_id] IS NOT NULL AND [int_request_id] IS NOT NULL AND [int_request_doc_type_id] IS NOT NULL AND [int_receive_docs_year] IS NOT NULL AND [dt_receive_docs_date] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_request_docs_sn_unique_key",
                table: "tb_request_docs_sn",
                columns: new[] { "int_provider_id", "int_request_docs_management_id", "int_request_doc_type_id", "int_receive_docs_year", "vc_request_doc_number", "int_request_docs_operation_id" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [int_request_docs_management_id] IS NOT NULL AND [int_request_doc_type_id] IS NOT NULL AND [int_receive_docs_year] IS NOT NULL AND [vc_request_doc_number] IS NOT NULL AND [int_request_docs_operation_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_trainer_id_int_vet_speciality_id_key",
                table: "tb_trainer_profiles",
                columns: new[] { "int_trainer_id", "int_vet_area_id", "int_vet_speciality_id" },
                unique: true,
                filter: "([int_trainer_id] IS NOT NULL AND [int_vet_area_id] IS NOT NULL AND [int_vet_speciality_id] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_trainers_int_provider_egn_key",
                table: "tb_trainers",
                columns: new[] { "int_provider_id", "vc_egn" },
                unique: true,
                filter: "([int_provider_id] IS NOT NULL AND [vc_egn] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_users_unhs_index",
                table: "tb_users",
                column: "unhs",
                unique: true,
                filter: "([unhs] IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "tb_users_upwd_index",
                table: "tb_users",
                column: "upwd",
                unique: true,
                filter: "([upwd] IS NOT NULL)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "accs_accesslog");

            migrationBuilder.DropTable(
                name: "accs_adminlog");

            migrationBuilder.DropTable(
                name: "accs_blockedip");

            migrationBuilder.DropTable(
                name: "accs_failures");

            migrationBuilder.DropTable(
                name: "accs_messages");

            migrationBuilder.DropTable(
                name: "accs_webuserdata");

            migrationBuilder.DropTable(
                name: "accs_webusergroups");

            migrationBuilder.DropTable(
                name: "arch_ref_clients_courses");

            migrationBuilder.DropTable(
                name: "arch_ref_provider_premises_specialities");

            migrationBuilder.DropTable(
                name: "arch_tb_clients");

            migrationBuilder.DropTable(
                name: "arch_tb_clients_courses_documents");

            migrationBuilder.DropTable(
                name: "arch_tb_course_groups");

            migrationBuilder.DropTable(
                name: "arch_tb_course_groups_40");

            migrationBuilder.DropTable(
                name: "arch_tb_course_groups_duplicates");

            migrationBuilder.DropTable(
                name: "arch_tb_courses");

            migrationBuilder.DropTable(
                name: "arch_tb_provider_premises");

            migrationBuilder.DropTable(
                name: "arch_tb_provider_specialities");

            migrationBuilder.DropTable(
                name: "arch_tb_provider_specialities_curriculum");

            migrationBuilder.DropTable(
                name: "arch_tb_providers");

            migrationBuilder.DropTable(
                name: "arch_tb_trainer_qualifications");

            migrationBuilder.DropTable(
                name: "arch_tb_trainers");

            migrationBuilder.DropTable(
                name: "code_assign_type");

            migrationBuilder.DropTable(
                name: "code_candidate_providers_state");

            migrationBuilder.DropTable(
                name: "code_candidate_type");

            migrationBuilder.DropTable(
                name: "code_ccontract_type");

            migrationBuilder.DropTable(
                name: "code_cfinished_type");

            migrationBuilder.DropTable(
                name: "code_cipo_management");

            migrationBuilder.DropTable(
                name: "code_comm_member");

            migrationBuilder.DropTable(
                name: "code_commission_institution_type");

            migrationBuilder.DropTable(
                name: "code_correction");

            migrationBuilder.DropTable(
                name: "code_course_ed_form");

            migrationBuilder.DropTable(
                name: "code_course_fr_curr");

            migrationBuilder.DropTable(
                name: "code_course_group_required_documents_type");

            migrationBuilder.DropTable(
                name: "code_course_measure_type");

            migrationBuilder.DropTable(
                name: "code_course_status");

            migrationBuilder.DropTable(
                name: "code_course_type");

            migrationBuilder.DropTable(
                name: "code_cpo_management");

            migrationBuilder.DropTable(
                name: "code_curric_hours_type");

            migrationBuilder.DropTable(
                name: "code_document_status");

            migrationBuilder.DropTable(
                name: "code_document_status_locks");

            migrationBuilder.DropTable(
                name: "code_document_type");

            migrationBuilder.DropTable(
                name: "code_document_validity_checks");

            migrationBuilder.DropTable(
                name: "code_documents_management");

            migrationBuilder.DropTable(
                name: "code_education");

            migrationBuilder.DropTable(
                name: "code_egn_type");

            migrationBuilder.DropTable(
                name: "code_ekatte");

            migrationBuilder.DropTable(
                name: "code_ekatte_full");

            migrationBuilder.DropTable(
                name: "code_employment_status");

            migrationBuilder.DropTable(
                name: "code_expert_type");

            migrationBuilder.DropTable(
                name: "code_ext_register");

            migrationBuilder.DropTable(
                name: "code_gender");

            migrationBuilder.DropTable(
                name: "code_licence_status");

            migrationBuilder.DropTable(
                name: "code_licence_status_details");

            migrationBuilder.DropTable(
                name: "code_municipality");

            migrationBuilder.DropTable(
                name: "code_municipality_details");

            migrationBuilder.DropTable(
                name: "code_nationality");

            migrationBuilder.DropTable(
                name: "code_nkpd");

            migrationBuilder.DropTable(
                name: "code_obl");

            migrationBuilder.DropTable(
                name: "code_operation");

            migrationBuilder.DropTable(
                name: "code_premises_correspondence");

            migrationBuilder.DropTable(
                name: "code_premises_status");

            migrationBuilder.DropTable(
                name: "code_premises_type");

            migrationBuilder.DropTable(
                name: "code_premises_usage");

            migrationBuilder.DropTable(
                name: "code_procedure_decision");

            migrationBuilder.DropTable(
                name: "code_procedure_stages");

            migrationBuilder.DropTable(
                name: "code_procedure_steps");

            migrationBuilder.DropTable(
                name: "code_procedures");

            migrationBuilder.DropTable(
                name: "code_procedures_documents");

            migrationBuilder.DropTable(
                name: "code_protokol_type");

            migrationBuilder.DropTable(
                name: "code_provider_ownership");

            migrationBuilder.DropTable(
                name: "code_provider_registration");

            migrationBuilder.DropTable(
                name: "code_provider_status");

            migrationBuilder.DropTable(
                name: "code_qual_level");

            migrationBuilder.DropTable(
                name: "code_qualification_type");

            migrationBuilder.DropTable(
                name: "code_receive_documents");

            migrationBuilder.DropTable(
                name: "code_receive_type");

            migrationBuilder.DropTable(
                name: "code_request_doc_status");

            migrationBuilder.DropTable(
                name: "code_request_doc_type");

            migrationBuilder.DropTable(
                name: "code_request_docs_operation");

            migrationBuilder.DropTable(
                name: "code_request_docs_series");

            migrationBuilder.DropTable(
                name: "code_speciality_curriculum_update_reason");

            migrationBuilder.DropTable(
                name: "code_speciality_vqs");

            migrationBuilder.DropTable(
                name: "code_stage_document_types");

            migrationBuilder.DropTable(
                name: "code_stage_documents");

            migrationBuilder.DropTable(
                name: "code_tcontract_type");

            migrationBuilder.DropTable(
                name: "code_tqualification_type");

            migrationBuilder.DropTable(
                name: "code_training_type");

            migrationBuilder.DropTable(
                name: "code_ui_control_type");

            migrationBuilder.DropTable(
                name: "code_upload_doc_status");

            migrationBuilder.DropTable(
                name: "code_upload_doc_type");

            migrationBuilder.DropTable(
                name: "code_validity_check_target");

            migrationBuilder.DropTable(
                name: "code_vet_area");

            migrationBuilder.DropTable(
                name: "code_vet_group");

            migrationBuilder.DropTable(
                name: "code_vet_list");

            migrationBuilder.DropTable(
                name: "code_vet_profession");

            migrationBuilder.DropTable(
                name: "code_vet_speciality");

            migrationBuilder.DropTable(
                name: "code_village_type");

            migrationBuilder.DropTable(
                name: "code_visit_result");

            migrationBuilder.DropTable(
                name: "code_wgdoi_function");

            migrationBuilder.DropTable(
                name: "iscs_iisclientdata");

            migrationBuilder.DropTable(
                name: "iscs_iisserverdata");

            migrationBuilder.DropTable(
                name: "mv_register_clients");

            migrationBuilder.DropTable(
                name: "ref_arch_expert_commissions");

            migrationBuilder.DropTable(
                name: "ref_arch_experts");

            migrationBuilder.DropTable(
                name: "ref_arch_procedure_expert_commissions");

            migrationBuilder.DropTable(
                name: "ref_arch_procedure_experts");

            migrationBuilder.DropTable(
                name: "ref_arch_procedure_procedures_documents");

            migrationBuilder.DropTable(
                name: "ref_arch_procedure_provider_premises_specialities");

            migrationBuilder.DropTable(
                name: "ref_arch_procedures_documents");

            migrationBuilder.DropTable(
                name: "ref_candidate_provider_premises_specialities");

            migrationBuilder.DropTable(
                name: "ref_candidates_expert_commissions");

            migrationBuilder.DropTable(
                name: "ref_candidates_experts");

            migrationBuilder.DropTable(
                name: "ref_candidates_procedures_documents");

            migrationBuilder.DropTable(
                name: "ref_cg_curric_files");

            migrationBuilder.DropTable(
                name: "ref_clients_courses");

            migrationBuilder.DropTable(
                name: "ref_clients_courses_documents_status");

            migrationBuilder.DropTable(
                name: "ref_course_document_type");

            migrationBuilder.DropTable(
                name: "ref_course_group_premises");

            migrationBuilder.DropTable(
                name: "ref_course_type_fr_curr");

            migrationBuilder.DropTable(
                name: "ref_doctype_reqdoctype");

            migrationBuilder.DropTable(
                name: "ref_document_status_locks");

            migrationBuilder.DropTable(
                name: "ref_experts_commissions");

            migrationBuilder.DropTable(
                name: "ref_experts_doi");

            migrationBuilder.DropTable(
                name: "ref_experts_types");

            migrationBuilder.DropTable(
                name: "ref_experts_vet_area");

            migrationBuilder.DropTable(
                name: "ref_fr_curr_ed_form");

            migrationBuilder.DropTable(
                name: "ref_fr_curr_educ_level");

            migrationBuilder.DropTable(
                name: "ref_fr_curr_qual_level");

            migrationBuilder.DropTable(
                name: "ref_main_expert_commissions");

            migrationBuilder.DropTable(
                name: "ref_main_experts");

            migrationBuilder.DropTable(
                name: "ref_procedure_step_stages");

            migrationBuilder.DropTable(
                name: "ref_procedure_steps");

            migrationBuilder.DropTable(
                name: "ref_procedures_documents");

            migrationBuilder.DropTable(
                name: "ref_provider_premises_specialities");

            migrationBuilder.DropTable(
                name: "ref_request_doc_status");

            migrationBuilder.DropTable(
                name: "ref_request_doc_type");

            migrationBuilder.DropTable(
                name: "ref_role_acl_actions");

            migrationBuilder.DropTable(
                name: "ref_role_groups");

            migrationBuilder.DropTable(
                name: "ref_role_users");

            migrationBuilder.DropTable(
                name: "ref_trainers_courses");

            migrationBuilder.DropTable(
                name: "ref_vet_specialities_nkpds");

            migrationBuilder.DropTable(
                name: "ref_visits");

            migrationBuilder.DropTable(
                name: "ref_visits_experts");

            migrationBuilder.DropTable(
                name: "report_clients");

            migrationBuilder.DropTable(
                name: "report_courses");

            migrationBuilder.DropTable(
                name: "report_curricula");

            migrationBuilder.DropTable(
                name: "report_premises");

            migrationBuilder.DropTable(
                name: "report_providers");

            migrationBuilder.DropTable(
                name: "report_trainers_qualification");

            migrationBuilder.DropTable(
                name: "ret");

            migrationBuilder.DropTable(
                name: "sys_acl");

            migrationBuilder.DropTable(
                name: "sys_locks");

            migrationBuilder.DropTable(
                name: "sys_mail_log");

            migrationBuilder.DropTable(
                name: "sys_operation_log");

            migrationBuilder.DropTable(
                name: "sys_sign_log");

            migrationBuilder.DropTable(
                name: "systransliterate");

            migrationBuilder.DropTable(
                name: "tb_acl_actions");

            migrationBuilder.DropTable(
                name: "tb_annual_info");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_provider_premises");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_provider_premises_rooms");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_provider_specialities_curriculum");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_providers");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_providers_cipo_management");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_providers_cpo_management");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_providers_documents_management");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_providers_specialities");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_trainer_documents");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_trainer_profiles");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_trainer_qualifications");

            migrationBuilder.DropTable(
                name: "tb_arch_procedure_trainers");

            migrationBuilder.DropTable(
                name: "tb_arch_providers");

            migrationBuilder.DropTable(
                name: "tb_candidate_provider_premises");

            migrationBuilder.DropTable(
                name: "tb_candidate_provider_premises_rooms");

            migrationBuilder.DropTable(
                name: "tb_candidate_provider_specialities_curriculum");

            migrationBuilder.DropTable(
                name: "tb_candidate_providers");

            migrationBuilder.DropTable(
                name: "tb_candidate_providers_cipo_management");

            migrationBuilder.DropTable(
                name: "tb_candidate_providers_cpo_management");

            migrationBuilder.DropTable(
                name: "tb_candidate_providers_documents_management");

            migrationBuilder.DropTable(
                name: "tb_candidate_providers_specialities");

            migrationBuilder.DropTable(
                name: "tb_candidate_providers_state");

            migrationBuilder.DropTable(
                name: "tb_candidate_trainer_documents");

            migrationBuilder.DropTable(
                name: "tb_candidate_trainer_profiles");

            migrationBuilder.DropTable(
                name: "tb_candidate_trainer_qualifications");

            migrationBuilder.DropTable(
                name: "tb_candidate_trainers");

            migrationBuilder.DropTable(
                name: "tb_clients");

            migrationBuilder.DropTable(
                name: "tb_clients_courses_documents");

            migrationBuilder.DropTable(
                name: "tb_clients_required_documents");

            migrationBuilder.DropTable(
                name: "tb_course_groups");

            migrationBuilder.DropTable(
                name: "tb_course_groups_40");

            migrationBuilder.DropTable(
                name: "tb_course_groups_duplicates");

            migrationBuilder.DropTable(
                name: "tb_course40_competences");

            migrationBuilder.DropTable(
                name: "tb_courses");

            migrationBuilder.DropTable(
                name: "tb_curric_modules");

            migrationBuilder.DropTable(
                name: "tb_doi");

            migrationBuilder.DropTable(
                name: "tb_doi_commissions");

            migrationBuilder.DropTable(
                name: "tb_e_signers");

            migrationBuilder.DropTable(
                name: "tb_expert_commissions");

            migrationBuilder.DropTable(
                name: "tb_experts");

            migrationBuilder.DropTable(
                name: "tb_generated_documents");

            migrationBuilder.DropTable(
                name: "tb_groups");

            migrationBuilder.DropTable(
                name: "tb_import_xml");

            migrationBuilder.DropTable(
                name: "tb_napoo_request_doc");

            migrationBuilder.DropTable(
                name: "tb_payments");

            migrationBuilder.DropTable(
                name: "tb_procedure_documents");

            migrationBuilder.DropTable(
                name: "tb_procedure_prices");

            migrationBuilder.DropTable(
                name: "tb_procedure_progress");

            migrationBuilder.DropTable(
                name: "tb_procedure_progress_documents");

            migrationBuilder.DropTable(
                name: "tb_provider_activities");

            migrationBuilder.DropTable(
                name: "tb_provider_premises");

            migrationBuilder.DropTable(
                name: "tb_provider_premises_rooms");

            migrationBuilder.DropTable(
                name: "tb_provider_specialities");

            migrationBuilder.DropTable(
                name: "tb_provider_specialities_curriculum");

            migrationBuilder.DropTable(
                name: "tb_provider_uploaded_docs");

            migrationBuilder.DropTable(
                name: "tb_providers");

            migrationBuilder.DropTable(
                name: "tb_providers_cipo_management");

            migrationBuilder.DropTable(
                name: "tb_providers_cpo_management");

            migrationBuilder.DropTable(
                name: "tb_providers_docs_dashboard");

            migrationBuilder.DropTable(
                name: "tb_providers_docs_offers");

            migrationBuilder.DropTable(
                name: "tb_providers_documents_management");

            migrationBuilder.DropTable(
                name: "tb_providers_licence_change");

            migrationBuilder.DropTable(
                name: "tb_providers_request_doc");

            migrationBuilder.DropTable(
                name: "tb_request_docs_management");

            migrationBuilder.DropTable(
                name: "tb_request_docs_sn");

            migrationBuilder.DropTable(
                name: "tb_roles");

            migrationBuilder.DropTable(
                name: "tb_sign_content");

            migrationBuilder.DropTable(
                name: "tb_started_procedure_progress");

            migrationBuilder.DropTable(
                name: "tb_started_procedures");

            migrationBuilder.DropTable(
                name: "tb_survey_answer");

            migrationBuilder.DropTable(
                name: "tb_survey_question");

            migrationBuilder.DropTable(
                name: "tb_trainer_documents");

            migrationBuilder.DropTable(
                name: "tb_trainer_profiles");

            migrationBuilder.DropTable(
                name: "tb_trainer_qualifications");

            migrationBuilder.DropTable(
                name: "tb_trainers");

            migrationBuilder.DropTable(
                name: "tb_user_press");

            migrationBuilder.DropTable(
                name: "tb_users");

            migrationBuilder.DropTable(
                name: "tb_version");

            migrationBuilder.DropSequence(
                name: "arch_tb_course_groups_40_id_seq");

            migrationBuilder.DropSequence(
                name: "arch_tb_course_groups_duplicates_id_seq");

            migrationBuilder.DropSequence(
                name: "ref_course_type_fr_curr_int_code_course_fr_curr_id_seq");

            migrationBuilder.DropSequence(
                name: "ref_course_type_fr_curr_int_code_course_type_id_seq");

            migrationBuilder.DropSequence(
                name: "seq_accs_adminlogid");

            migrationBuilder.DropSequence(
                name: "seq_accs_groupnames");

            migrationBuilder.DropSequence(
                name: "seq_accs_sessionid");

            migrationBuilder.DropSequence(
                name: "seq_arch_clients_courses_id");

            migrationBuilder.DropSequence(
                name: "seq_arch_courses_id");

            migrationBuilder.DropSequence(
                name: "seq_arch_courses_providers_id");

            migrationBuilder.DropSequence(
                name: "seq_arch_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_arch_providers_department_id");

            migrationBuilder.DropSequence(
                name: "seq_arch_providers_id");

            migrationBuilder.DropSequence(
                name: "seq_arch_trainers_id");

            migrationBuilder.DropSequence(
                name: "seq_code_assign_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_candidate_providers_state_id");

            migrationBuilder.DropSequence(
                name: "seq_code_candidate_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_ccontract_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_cfinished_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_cipo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_code_comm_member_id");

            migrationBuilder.DropSequence(
                name: "seq_code_commission_institution_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_correction_id");

            migrationBuilder.DropSequence(
                name: "seq_code_course_ed_form_id");

            migrationBuilder.DropSequence(
                name: "seq_code_course_fr_curr_id");

            migrationBuilder.DropSequence(
                name: "seq_code_course_group_required_documents_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_course_measure_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_course_status_id");

            migrationBuilder.DropSequence(
                name: "seq_code_course_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_cpo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_code_curric_hours_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_document_status");

            migrationBuilder.DropSequence(
                name: "seq_code_document_status_locks_id");

            migrationBuilder.DropSequence(
                name: "seq_code_document_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_documents_management_id");

            migrationBuilder.DropSequence(
                name: "seq_code_education_id");

            migrationBuilder.DropSequence(
                name: "seq_code_egn_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_ekatte_full_id");

            migrationBuilder.DropSequence(
                name: "seq_code_ekatte_id");

            migrationBuilder.DropSequence(
                name: "seq_code_employment_status_id");

            migrationBuilder.DropSequence(
                name: "seq_code_expert_position_id");

            migrationBuilder.DropSequence(
                name: "seq_code_expert_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_gender_id");

            migrationBuilder.DropSequence(
                name: "seq_code_licence_status_details_id");

            migrationBuilder.DropSequence(
                name: "seq_code_licence_status_id");

            migrationBuilder.DropSequence(
                name: "seq_code_municipality_details_id");

            migrationBuilder.DropSequence(
                name: "seq_code_municipality_id");

            migrationBuilder.DropSequence(
                name: "seq_code_nationality_id");

            migrationBuilder.DropSequence(
                name: "seq_code_nkpd_id");

            migrationBuilder.DropSequence(
                name: "seq_code_obl_id");

            migrationBuilder.DropSequence(
                name: "seq_code_operation_id");

            migrationBuilder.DropSequence(
                name: "seq_code_premises_correspondence_id");

            migrationBuilder.DropSequence(
                name: "seq_code_premises_status_id");

            migrationBuilder.DropSequence(
                name: "seq_code_premises_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_premises_usage_id");

            migrationBuilder.DropSequence(
                name: "seq_code_procedures_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_code_provider_ownership_id");

            migrationBuilder.DropSequence(
                name: "seq_code_provider_registration_id");

            migrationBuilder.DropSequence(
                name: "seq_code_provider_status_id");

            migrationBuilder.DropSequence(
                name: "seq_code_qual_level_id");

            migrationBuilder.DropSequence(
                name: "seq_code_qualification_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_request_doc_status_id");

            migrationBuilder.DropSequence(
                name: "seq_code_request_doc_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_speciality_curriculum_update_reason_id");

            migrationBuilder.DropSequence(
                name: "seq_code_speciality_vqs_id");

            migrationBuilder.DropSequence(
                name: "seq_code_tcontract_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_tqualification_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_training_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_validity_check_target");

            migrationBuilder.DropSequence(
                name: "seq_code_vet_area_id");

            migrationBuilder.DropSequence(
                name: "seq_code_vet_group_id");

            migrationBuilder.DropSequence(
                name: "seq_code_vet_list_id");

            migrationBuilder.DropSequence(
                name: "seq_code_vet_profession_id");

            migrationBuilder.DropSequence(
                name: "seq_code_vet_speciality_id");

            migrationBuilder.DropSequence(
                name: "seq_code_village_type_id");

            migrationBuilder.DropSequence(
                name: "seq_code_visit_result_id");

            migrationBuilder.DropSequence(
                name: "seq_code_wgdoi_function_id");

            migrationBuilder.DropSequence(
                name: "seq_groups_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_expert_commissions_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_procedure_expert_commissions_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_procedure_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_procedure_procedures_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_procedure_provider_premises_specialities_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_arch_procedures_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_candidates_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_candidates_procedures_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_cg_curric_files_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_clients_courses_documents_status");

            migrationBuilder.DropSequence(
                name: "seq_ref_clients_courses_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_clients_lab_offices_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_course_document_type_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_course_group_premises_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_courses_providers_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_document_type_cfinished_type_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_experts_commissions_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_experts_doi_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_experts_types_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_experts_vet_area_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_fr_curr_ed_form_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_fr_curr_educ_level_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_fr_curr_qual_level_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_main_expert_commissions_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_main_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_procedures_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_provider_premises_specialities_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_request_doc_status_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_request_doc_type_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_role_acl_actions_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_role_groups_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_role_users_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_trainers_courses_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_vet_specialities_nkpds_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_visits_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_ref_visits_id");

            migrationBuilder.DropSequence(
                name: "seq_sys_acl_id");

            migrationBuilder.DropSequence(
                name: "seq_sys_mail_log_id");

            migrationBuilder.DropSequence(
                name: "seq_sys_operation_log_id");

            migrationBuilder.DropSequence(
                name: "seq_sys_sign_log_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_acl_actions_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_acl_defaults_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_annual_info_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_provider_premises_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_provider_premises_rooms_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_provider_specialities_curriculum_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_provider_specialities_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_providers_cipo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_providers_cpo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_providers_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_trainer_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_trainer_profiles_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_trainer_qualifications_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_procedure_trainers_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_arch_providers_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_candidate_providers_cipo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_candidate_providers_cpo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_candidate_providers_documents_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_candidate_providers_state_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_clients_courses_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_clients_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_clients_required_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_course_groups_40_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_course_groups_duplicates_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_course_groups_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_course40_competences_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_courses_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_curric_modules_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_doi_commissions_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_doi_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_e_signers_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_expert_commissions_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_expert_profiles_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_experts_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_generated_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_import_xml_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_lab_offices_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_napoo_request_doc_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_payments_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_procedure_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_procedure_prices_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_procedure_progress_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_procedure_progress_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_activities_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_departments_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_premises_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_premises_rooms_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_specialities_curriculum_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_specialities_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_provider_uploaded_docs_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_cipo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_cpo_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_docs_dashboard_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_docs_offers_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_documents_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_licence_change_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_providers_request_doc_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_request_docs_management_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_request_docs_sn_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_roles_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_sign_content_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_started_procedure_progress_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_started_procedures_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_survey_answer_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_survey_question_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_trainer_documents_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_trainer_profiles_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_trainer_qualifications_id");

            migrationBuilder.DropSequence(
                name: "seq_tb_trainers_id");

            migrationBuilder.DropSequence(
                name: "seq_users_id");
        }
    }
}
