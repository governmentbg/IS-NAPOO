using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedureDocumentAddDocNumbers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DS_DocNumber",
                table: "Procedure_Document",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_OFFICIAL_DocNumber",
                table: "Procedure_Document",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DS_DocNumber",
                table: "Procedure_Document");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_DocNumber",
                table: "Procedure_Document");
        }
    }
}
