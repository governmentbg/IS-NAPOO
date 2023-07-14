using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableNAPOORequestDoc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_request_NAPOORequestDoc",
                table: "request_NAPOORequestDoc");

            migrationBuilder.RenameTable(
                name: "request_NAPOORequestDoc",
                newName: "Request_NAPOORequestDoc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Request_NAPOORequestDoc",
                table: "Request_NAPOORequestDoc",
                column: "IdNAPOORequestDoc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Request_NAPOORequestDoc",
                table: "Request_NAPOORequestDoc");

            migrationBuilder.RenameTable(
                name: "Request_NAPOORequestDoc",
                newName: "request_NAPOORequestDoc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_request_NAPOORequestDoc",
                table: "request_NAPOORequestDoc",
                column: "IdNAPOORequestDoc");
        }
    }
}
