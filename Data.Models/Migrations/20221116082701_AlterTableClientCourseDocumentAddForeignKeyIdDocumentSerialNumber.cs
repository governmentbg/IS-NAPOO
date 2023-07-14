using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseDocumentAddForeignKeyIdDocumentSerialNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdDocumentSerialNumber",
                table: "Training_ClientCourseDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с фабричен номер на документ от печатница на МОН");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseDocument_IdDocumentSerialNumber",
                table: "Training_ClientCourseDocument",
                column: "IdDocumentSerialNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourseDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                table: "Training_ClientCourseDocument",
                column: "IdDocumentSerialNumber",
                principalTable: "Request_DocumentSerialNumber",
                principalColumn: "IdDocumentSerialNumber");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourseDocument_Request_DocumentSerialNumber_IdDocumentSerialNumber",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_ClientCourseDocument_IdDocumentSerialNumber",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropColumn(
                name: "IdDocumentSerialNumber",
                table: "Training_ClientCourseDocument");
        }
    }
}
