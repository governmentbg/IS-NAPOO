using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableNAPOORequestDocAddColumnNAPOORequestNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NAPOORequestNumber",
                table: "Request_NAPOORequestDoc",
                type: "int",
                nullable: true,
                comment: "Ноемер на заявка");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NAPOORequestNumber",
                table: "Request_NAPOORequestDoc");
        }
    }
}
