using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddCandidateProviderActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCandidateProviderActive",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdCandidateProviderActive",
                table: "Candidate_Provider",
                column: "IdCandidateProviderActive");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Provider_Candidate_Provider_IdCandidateProviderActive",
                table: "Candidate_Provider",
                column: "IdCandidateProviderActive",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Provider_Candidate_Provider_IdCandidateProviderActive",
                table: "Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_Provider_IdCandidateProviderActive",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdCandidateProviderActive",
                table: "Candidate_Provider");
        }
    }
}
