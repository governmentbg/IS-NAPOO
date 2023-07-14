using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderTrainerSpecialityAndCandidateProviderPremisesSpecialityAddIdComplianceDOC : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdComplianceDOC",
                table: "Candidate_ProviderTrainerSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Съответствие с ДОС");

            migrationBuilder.AddColumn<int>(
                name: "IdComplianceDOC",
                table: "Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Съответствие с ДОС");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdComplianceDOC",
                table: "Candidate_ProviderTrainerSpeciality");

            migrationBuilder.DropColumn(
                name: "IdComplianceDOC",
                table: "Candidate_ProviderPremisesSpeciality");
        }
    }
}
