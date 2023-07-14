using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableRequestDocumentManagementAddReceiveDocumentYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReceiveDocumentYear",
                table: "Request_RequestDocumentManagement",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Календарна година");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiveDocumentYear",
                table: "Request_RequestDocumentManagement");
        }
    }
}
