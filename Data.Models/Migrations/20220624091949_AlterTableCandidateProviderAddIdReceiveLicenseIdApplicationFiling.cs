using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddIdReceiveLicenseIdApplicationFiling : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdApplicationFiling",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                comment: "Начин на подаване на заявление и документ за платена държавна такса");

            migrationBuilder.AddColumn<int>(
                name: "IdReceiveLicense",
                table: "Candidate_Provider",
                type: "int",
                nullable: true,
                comment: "Начин на получаване на административен акт и лицензия");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdApplicationFiling",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdReceiveLicense",
                table: "Candidate_Provider");
        }
    }
}
