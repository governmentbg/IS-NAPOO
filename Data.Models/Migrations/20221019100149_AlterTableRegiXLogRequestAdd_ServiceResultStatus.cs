using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableRegiXLogRequestAdd_ServiceResultStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ServiceResultStatus",
                table: "Arch_RegiXLogRequest",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Статус на заявката към RegiX");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceResultStatus",
                table: "Arch_RegiXLogRequest");
        }
    }
}
