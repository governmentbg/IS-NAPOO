using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderDocumentAddIsAdditionalDocument : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdditionalDocument",
                table: "Candidate_ProviderDocument",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Допълнителни документи");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdditionalDocument",
                table: "Candidate_ProviderDocument");
        }
    }
}
