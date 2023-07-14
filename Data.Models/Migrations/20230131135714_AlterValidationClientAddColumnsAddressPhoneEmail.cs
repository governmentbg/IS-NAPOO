using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterValidationClientAddColumnsAddressPhoneEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Training_ValidationClient",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Адрес");

            migrationBuilder.AddColumn<string>(
                name: "EmailAddress",
                table: "Training_ValidationClient",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "E-mail адрес");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Training_ValidationClient",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Телефон");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "EmailAddress",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Training_ValidationClient");
        }
    }
}
