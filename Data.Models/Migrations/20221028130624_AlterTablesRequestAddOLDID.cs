using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesRequestAddOLDID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_RequestDocumentType",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_RequestDocumentType",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_RequestDocumentStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_RequestDocumentStatus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_RequestDocumentManagement",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_RequestDocumentManagement",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_ProviderRequestDocument",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_NAPOORequestDoc",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_NAPOORequestDoc",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Request_DocumentSerialNumber",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Request_DocumentSerialNumber",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_RequestDocumentType");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_RequestDocumentType");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_RequestDocumentStatus");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_RequestDocumentStatus");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_RequestDocumentManagement");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_ProviderRequestDocument");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_NAPOORequestDoc");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_NAPOORequestDoc");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Request_DocumentSerialNumber");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Request_DocumentSerialNumber");
        }
    }
}
