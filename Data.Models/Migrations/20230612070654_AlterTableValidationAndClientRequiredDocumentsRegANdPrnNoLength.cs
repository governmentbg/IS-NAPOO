using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableValidationAndClientRequiredDocumentsRegANdPrnNoLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentRegNo",
                table: "Training_ValidationClientRequiredDocument",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Регистрационен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Регистрационен номер");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPrnNo",
                table: "Training_ValidationClientRequiredDocument",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Фабричен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Фабричен номер");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentRegNo",
                table: "Training_ClientRequiredDocument",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Регистрационен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Регистрационен номер");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPrnNo",
                table: "Training_ClientRequiredDocument",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Фабричен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Фабричен номер");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DocumentRegNo",
                table: "Training_ValidationClientRequiredDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Регистрационен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Регистрационен номер");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPrnNo",
                table: "Training_ValidationClientRequiredDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Фабричен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Фабричен номер");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentRegNo",
                table: "Training_ClientRequiredDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Регистрационен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Регистрационен номер");

            migrationBuilder.AlterColumn<string>(
                name: "DocumentPrnNo",
                table: "Training_ClientRequiredDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Фабричен номер",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Фабричен номер");
        }
    }
}
