using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableArchCandidateProviderSpecialiyIDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Arch_Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Arch_Candidate_ProviderPremisesSpeciality");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Arch_Candidate_ProviderPremisesSpeciality");
        }
    }
}
