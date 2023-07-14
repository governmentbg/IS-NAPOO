using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Client_AddColumns_FirstNameEN_SecondNameEN_FamilyNameEN : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FamilyNameEN",
                table: "Training_Client",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Фамилия на Латиница");

            migrationBuilder.AddColumn<string>(
                name: "FirstNameEN",
                table: "Training_Client",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Име на Латиница");

            migrationBuilder.AddColumn<string>(
                name: "SecondNameEN",
                table: "Training_Client",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Презиме на Латиница");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FamilyNameEN",
                table: "Training_Client");

            migrationBuilder.DropColumn(
                name: "FirstNameEN",
                table: "Training_Client");

            migrationBuilder.DropColumn(
                name: "SecondNameEN",
                table: "Training_Client");
        }
    }
}
