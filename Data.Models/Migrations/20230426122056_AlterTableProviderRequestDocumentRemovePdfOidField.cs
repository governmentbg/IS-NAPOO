using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProviderRequestDocumentRemovePdfOidField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "oid_request_pdf",
                table: "Request_ProviderRequestDocument");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "oid_request_pdf",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: true);
        }
    }
}
