using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ClientCourseDocStatus_ValidationClientDocStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus",
                type: "ntext",
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ClientCourseDocumentStatus",
                type: "ntext",
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ClientCourseDocumentStatus",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");
        }
    }
}
