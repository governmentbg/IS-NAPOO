using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesDocmentsIDataMigrationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderPremisesDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderPremisesDocument",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderPremisesDocument");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderPremisesDocument");
        }
    }
}
