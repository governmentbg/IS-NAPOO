using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CandidateProviderIndicator_AddColumn_IdIndicator : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                type: "int",
                nullable: true,
                defaultValue: 0,
                comment: "Връзка с Показател");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_CandidateProviderIndicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                column: "IdIndicator");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Rating_Indicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator",
                column: "IdIndicator",
                principalTable: "Rating_Indicator",
                principalColumn: "IdIndicator",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_CandidateProviderIndicator_Rating_Indicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropIndex(
                name: "IX_Rating_CandidateProviderIndicator_IdIndicator",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropColumn(
                name: "IdIndicator",
                table: "Rating_CandidateProviderIndicator");
        }
    }
}
