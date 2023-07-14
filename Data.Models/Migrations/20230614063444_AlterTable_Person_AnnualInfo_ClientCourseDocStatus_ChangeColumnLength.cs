using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Person_AnnualInfo_ClientCourseDocStatus_ChangeColumnLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus",
                type: "nvarchar(4000)",
                maxLength: 4000,
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
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Person",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "Person",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Person",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Arch_AnnualInfo",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Длъжност",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Длъжност");

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Arch_AnnualInfo",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Arch_AnnualInfo",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Arch_AnnualInfo");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Arch_AnnualInfo");

            migrationBuilder.AlterColumn<string>(
                name: "SubmissionComment",
                table: "Training_ValidationClientDocumentStatus",
                type: "nvarchar(255)",
                maxLength: 255,
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
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Коментар при подаване към НАПОО",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true,
                oldComment: "Коментар при подаване към НАПОО");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Person",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "Person",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "Person",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Arch_AnnualInfo",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Длъжност",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Длъжност");
        }
    }
}
