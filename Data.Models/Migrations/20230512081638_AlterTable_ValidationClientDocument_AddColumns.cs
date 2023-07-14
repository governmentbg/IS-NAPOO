using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ValidationClientDocument_AddColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentPrnNo",
                table: "Training_ValidationClientDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Фабричен номер");

            migrationBuilder.AddColumn<string>(
                name: "DocumentSerNo",
                table: "Training_ValidationClientDocument",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Сериен номер");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentPrnNo",
                table: "Training_ValidationClientDocument");

            migrationBuilder.DropColumn(
                name: "DocumentSerNo",
                table: "Training_ValidationClientDocument");
        }
    }
}
