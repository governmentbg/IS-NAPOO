using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesRoomIdCandidateProviderPremises : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderPremisesRoom_Candidate_ProviderPremises_IdCandidate_Provider",
                table: "Candidate_ProviderPremisesRoom");

            migrationBuilder.RenameColumn(
                name: "IdCandidate_Provider",
                table: "Candidate_ProviderPremisesRoom",
                newName: "IdCandidateProviderPremises");

            migrationBuilder.RenameIndex(
                name: "IX_Candidate_ProviderPremisesRoom_IdCandidate_Provider",
                table: "Candidate_ProviderPremisesRoom",
                newName: "IX_Candidate_ProviderPremisesRoom_IdCandidateProviderPremises");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderPremisesRoom_Candidate_ProviderPremises_IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesRoom",
                column: "IdCandidateProviderPremises",
                principalTable: "Candidate_ProviderPremises",
                principalColumn: "IdCandidateProviderPremises",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_ProviderPremisesRoom_Candidate_ProviderPremises_IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesRoom");

            migrationBuilder.RenameColumn(
                name: "IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesRoom",
                newName: "IdCandidate_Provider");

            migrationBuilder.RenameIndex(
                name: "IX_Candidate_ProviderPremisesRoom_IdCandidateProviderPremises",
                table: "Candidate_ProviderPremisesRoom",
                newName: "IX_Candidate_ProviderPremisesRoom_IdCandidate_Provider");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_ProviderPremisesRoom_Candidate_ProviderPremises_IdCandidate_Provider",
                table: "Candidate_ProviderPremisesRoom",
                column: "IdCandidate_Provider",
                principalTable: "Candidate_ProviderPremises",
                principalColumn: "IdCandidateProviderPremises",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
