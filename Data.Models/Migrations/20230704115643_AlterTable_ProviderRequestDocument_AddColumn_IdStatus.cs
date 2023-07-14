using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ProviderRequestDocument_AddColumn_IdStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "Request_ProviderRequestDocument",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Статус на заявката");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "Request_ProviderRequestDocument");
        }
    }
}
