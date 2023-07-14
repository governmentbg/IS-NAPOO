using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableDocumentSerialNumberAddIdRequestReport : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRequestReport",
                table: "Request_DocumentSerialNumber",
                type: "int",
                nullable: true,
                comment: "Връзка с Отчет на документи с фабрична номерация по наредба 8");

            migrationBuilder.CreateIndex(
                name: "IX_Request_DocumentSerialNumber_IdRequestReport",
                table: "Request_DocumentSerialNumber",
                column: "IdRequestReport");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_DocumentSerialNumber_Request_Report_IdRequestReport",
                table: "Request_DocumentSerialNumber",
                column: "IdRequestReport",
                principalTable: "Request_Report",
                principalColumn: "IdRequestReport");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_DocumentSerialNumber_Request_Report_IdRequestReport",
                table: "Request_DocumentSerialNumber");

            migrationBuilder.DropIndex(
                name: "IX_Request_DocumentSerialNumber_IdRequestReport",
                table: "Request_DocumentSerialNumber");

            migrationBuilder.DropColumn(
                name: "IdRequestReport",
                table: "Request_DocumentSerialNumber");
        }
    }
}
