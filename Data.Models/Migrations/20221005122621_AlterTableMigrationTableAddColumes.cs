using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableMigrationTableAddColumes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_Speciality",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "SPPOO_Speciality",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_ProfessionalDirection",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "SPPOO_ProfessionalDirection",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_Profession",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "SPPOO_Profession",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "SPPOO_Area",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "SPPOO_Area",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Procedure_StartedProcedure",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Procedure_StartedProcedure",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainerQualification",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderTrainerQualification",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainerProfile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderTrainerProfile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainerDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderTrainerDocument",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderTrainer",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderPremises",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderPremises",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_Provider",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "SPPOO_Speciality");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_ProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "SPPOO_ProfessionalDirection");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_Profession");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "SPPOO_Profession");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "SPPOO_Area");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "SPPOO_Area");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Procedure_StartedProcedure");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Procedure_StartedProcedure");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainerQualification");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderTrainerQualification");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainerProfile");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderTrainerProfile");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainerDocument");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderTrainerDocument");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderTrainer");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderTrainer");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderPremises");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderPremises");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_Provider");
        }
    }
}
