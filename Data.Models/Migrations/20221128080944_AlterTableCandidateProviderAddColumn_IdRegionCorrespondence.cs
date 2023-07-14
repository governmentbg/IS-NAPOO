using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddColumn_IdRegionCorrespondence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRegionCorrespondence",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdRegionCorrespondence",
                table: "Candidate_Provider",
                column: "IdRegionCorrespondence");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Provider_Region_IdRegionCorrespondence",
                table: "Candidate_Provider",
                column: "IdRegionCorrespondence",
                principalTable: "Region",
                principalColumn: "idRegion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Provider_Region_IdRegionCorrespondence",
                table: "Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_Provider_IdRegionCorrespondence",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdRegionCorrespondence",
                table: "Candidate_Provider");
        }
    }
}
