using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ValidationClientDocumentStatus_AddColumn_SubmissionComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Коментар при подаване към НАПОО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus");
        }
    }
}
