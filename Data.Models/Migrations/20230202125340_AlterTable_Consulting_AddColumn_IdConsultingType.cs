using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Consulting_AddColumn_IdConsultingType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdConsultingType",
                table: "Training_Consulting",
                type: "int",
                nullable: true,
                comment: "Вид на дейността по консултиране");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdConsultingType",
                table: "Training_Consulting");
        }
    }
}
