using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterValidationOrCourseClientDocumentStatusColumnSubmissionComment : Migration
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
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ClientCourseDocumentStatus",
                type: "ntext",
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ClientCourseDocumentStatus",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "ntext",
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");
        }
    }
}
