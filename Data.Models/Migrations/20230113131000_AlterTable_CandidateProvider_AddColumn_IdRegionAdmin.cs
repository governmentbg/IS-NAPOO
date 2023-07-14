using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CandidateProvider_AddColumn_IdRegionAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRegionAdmin",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidate_Provider_IdRegionAdmin",
                table: "Candidate_Provider",
                column: "IdRegionAdmin");

            migrationBuilder.AddForeignKey(
                name: "FK_Candidate_Provider_Region_IdRegionAdmin",
                table: "Candidate_Provider",
                column: "IdRegionAdmin",
                principalTable: "Region",
                principalColumn: "idRegion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Candidate_Provider_Region_IdRegionAdmin",
                table: "Candidate_Provider");

            migrationBuilder.DropIndex(
                name: "IX_Candidate_Provider_IdRegionAdmin",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdRegionAdmin",
                table: "Candidate_Provider");
        }
    }
}
