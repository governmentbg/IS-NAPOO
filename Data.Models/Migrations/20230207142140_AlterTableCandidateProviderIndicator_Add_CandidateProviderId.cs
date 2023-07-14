using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderIndicator_Add_CandidateProviderId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weigh",
                table: "Rating_Indicator");

            migrationBuilder.DropColumn(
                name: "Weigh",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.AddColumn<int>(
                name: "IdCandidate_Provider",
                table: "Rating_CandidateProviderIndicator",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Връзка с  CPO,CIPO - Кандидат Обучаваща институция");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_CandidateProviderIndicator_IdCandidate_Provider",
                table: "Rating_CandidateProviderIndicator",
                column: "IdCandidate_Provider");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Candidate_Provider_IdCandidate_Provider",
                table: "Rating_CandidateProviderIndicator",
                column: "IdCandidate_Provider",
                principalTable: "Candidate_Provider",
                principalColumn: "IdCandidate_Provider",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Candidate_Provider_IdCandidate_Provider",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropIndex(
                name: "IX_Rating_CandidateProviderIndicator_IdCandidate_Provider",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropColumn(
                name: "IdCandidate_Provider",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.AddColumn<decimal>(
                name: "Weigh",
                table: "Rating_Indicator",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Тежест");

            migrationBuilder.AddColumn<decimal>(
                name: "Weigh",
                table: "Rating_CandidateProviderIndicator",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Тежест");
        }
    }
}
