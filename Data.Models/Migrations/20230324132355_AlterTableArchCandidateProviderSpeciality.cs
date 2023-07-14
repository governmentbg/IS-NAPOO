using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableArchCandidateProviderSpeciality : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_Curriculum_Arch_Candidate_ProviderSpeciality_IdArchCandidateProvider",
                table: "Arch_Candidate_Curriculum");

            migrationBuilder.RenameColumn(
                name: "IdArchCandidateProvider",
                table: "Arch_Candidate_Curriculum",
                newName: "IdArchCandidateProviderSpeciality");

            migrationBuilder.RenameIndex(
                name: "IX_Arch_Candidate_Curriculum_IdArchCandidateProvider",
                table: "Arch_Candidate_Curriculum",
                newName: "IX_Arch_Candidate_Curriculum_IdArchCandidateProviderSpeciality");

            migrationBuilder.AddColumn<int>(
                name: "IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Arch_Candidate_ProviderSpeciality_IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderSpeciality",
                column: "IdArchCandidateProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_Curriculum_Arch_Candidate_ProviderSpeciality_IdArchCandidateProviderSpeciality",
                table: "Arch_Candidate_Curriculum",
                column: "IdArchCandidateProviderSpeciality",
                principalTable: "Arch_Candidate_ProviderSpeciality",
                principalColumn: "IdArchCandidateProviderSpeciality",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_ProviderSpeciality_Arch_Candidate_Provider_IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderSpeciality",
                column: "IdArchCandidateProvider",
                principalTable: "Arch_Candidate_Provider",
                principalColumn: "IdArchCandidateProvider",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_Curriculum_Arch_Candidate_ProviderSpeciality_IdArchCandidateProviderSpeciality",
                table: "Arch_Candidate_Curriculum");

            migrationBuilder.DropForeignKey(
                name: "FK_Arch_Candidate_ProviderSpeciality_Arch_Candidate_Provider_IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderSpeciality");

            migrationBuilder.DropIndex(
                name: "IX_Arch_Candidate_ProviderSpeciality_IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "IdArchCandidateProvider",
                table: "Arch_Candidate_ProviderSpeciality");

            migrationBuilder.RenameColumn(
                name: "IdArchCandidateProviderSpeciality",
                table: "Arch_Candidate_Curriculum",
                newName: "IdArchCandidateProvider");

            migrationBuilder.RenameIndex(
                name: "IX_Arch_Candidate_Curriculum_IdArchCandidateProviderSpeciality",
                table: "Arch_Candidate_Curriculum",
                newName: "IX_Arch_Candidate_Curriculum_IdArchCandidateProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Arch_Candidate_Curriculum_Arch_Candidate_ProviderSpeciality_IdArchCandidateProvider",
                table: "Arch_Candidate_Curriculum",
                column: "IdArchCandidateProvider",
                principalTable: "Arch_Candidate_ProviderSpeciality",
                principalColumn: "IdArchCandidateProviderSpeciality",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
