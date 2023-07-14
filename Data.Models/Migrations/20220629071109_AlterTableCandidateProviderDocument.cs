using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderDocument_Candidate_Provider_CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_ProviderDocument_CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument");

            migrationBuilder.DropColumn(
                name: "CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument");

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderDocument_IdCandidateProvider",
                table: "Candidate_ProviderDocument",
                column: "IdCandidateProvider");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderDocument_Candidate_Provider_IdCandidateProvider",
                table: "Candidate_ProviderDocument",
                column: "IdCandidateProvider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderDocument_Candidate_Provider_IdCandidateProvider",
                table: "Candidate_ProviderDocument");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_ProviderDocument_IdCandidateProvider",
                table: "Candidate_ProviderDocument");

            migrationBuilder.AddColumn<int>(
                name: "CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_ProviderDocument_CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument",
                column: "CandidateProviderIdCandidate_Provider");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderDocument_Candidate_Provider_CandidateProviderIdCandidate_Provider",
                table: "Candidate_ProviderDocument",
                column: "CandidateProviderIdCandidate_Provider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
