using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableRequestDocumentManagementAddIdDocumentRequestReceiveType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdDocumentRequestReceiveType",
                table: "Request_RequestDocumentManagement",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Начин на получаване на заявката за документи");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdDocumentRequestReceiveType",
                table: "Request_RequestDocumentManagement");
        }
    }
}
