using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderSpecialityAddIdFrameworkProgramIdFormEducation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdFormEducation",
                table: "Candidate_ProviderSpeciality",
                type: "int",
                nullable: true,
                comment: "Форма на обучение");

            migrationBuilder.AddColumn<int>(
                name: "IdFrameworkProgram",
                table: "Candidate_ProviderSpeciality",
                type: "int",
                nullable: true,
                comment: "Рамкова програма");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderSpeciality_IdFrameworkProgram",
                table: "Candidate_ProviderSpeciality",
                column: "IdFrameworkProgram");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderSpeciality_SPPOO_FrameworkProgram_IdFrameworkProgram",
                table: "Candidate_ProviderSpeciality",
                column: "IdFrameworkProgram",
                principalTable: "SPPOO_FrameworkProgram",
                principalColumn: "IdFrameworkProgram");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderSpeciality_SPPOO_FrameworkProgram_IdFrameworkProgram",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_ProviderSpeciality_IdFrameworkProgram",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "IdFormEducation",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "IdFrameworkProgram",
                table: "Candidate_ProviderSpeciality");
        }
    }
}
