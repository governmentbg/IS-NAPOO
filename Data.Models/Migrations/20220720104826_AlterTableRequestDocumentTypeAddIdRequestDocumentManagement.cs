using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableRequestDocumentTypeAddIdRequestDocumentManagement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdRequestDocumentManagement",
                table: "Request_RequestDocumentType",
                type: "int",
                nullable: true,
                comment: "Връзка с получен документ");

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestDocumentType_IdRequestDocumentManagement",
                table: "Request_RequestDocumentType",
                column: "IdRequestDocumentManagement");

            migrationBuilder.AddForeignKey(
                name: "FK_Request_RequestDocumentType_Request_RequestDocumentManagement_IdRequestDocumentManagement",
                table: "Request_RequestDocumentType",
                column: "IdRequestDocumentManagement",
                principalTable: "Request_RequestDocumentManagement",
                principalColumn: "IdRequestDocumentManagement");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Request_RequestDocumentType_Request_RequestDocumentManagement_IdRequestDocumentManagement",
                table: "Request_RequestDocumentType");

            migrationBuilder.DropIndex(
                name: "IX_Request_RequestDocumentType_IdRequestDocumentManagement",
                table: "Request_RequestDocumentType");

            migrationBuilder.DropColumn(
                name: "IdRequestDocumentManagement",
                table: "Request_RequestDocumentType");
        }
    }
}
