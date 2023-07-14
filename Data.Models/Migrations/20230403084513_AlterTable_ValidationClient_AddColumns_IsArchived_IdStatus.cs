using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ValidationClient_AddColumns_IsArchived_IdStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "Training_ValidationClient",
                type: "int",
                nullable: true,
                comment: "Статус на валидирането");

            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Training_ValidationClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Дали валидирането е архивирано");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "Training_ValidationClient");

            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Training_ValidationClient");
        }
    }
}
