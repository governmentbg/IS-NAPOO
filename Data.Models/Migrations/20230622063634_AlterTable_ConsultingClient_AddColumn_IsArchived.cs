using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_ConsultingClient_AddColumn_IsArchived : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Training_ConsultingClient",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Дали консултираното лице е архивирано");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Training_ConsultingClient");
        }
    }
}
