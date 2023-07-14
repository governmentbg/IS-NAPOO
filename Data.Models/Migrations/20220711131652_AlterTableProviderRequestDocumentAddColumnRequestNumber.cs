using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProviderRequestDocumentAddColumnRequestNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestNumber",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: true,
                comment: "Ноемер на заявка");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestNumber",
                table: "Request_ProviderRequestDocument");
        }
    }
}
