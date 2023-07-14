using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_CandidateProvider_add_AdditionalDocumentRequested : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AdditionalDocumentRequested",
                table: "Candidate_Provider",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Искане на допълнителни документи от Водещ експерт");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdditionalDocumentRequested",
                table: "Candidate_Provider");
        }
    }
}
