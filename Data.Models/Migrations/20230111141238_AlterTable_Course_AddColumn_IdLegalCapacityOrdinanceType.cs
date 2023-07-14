using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Course_AddColumn_IdLegalCapacityOrdinanceType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdLegalCapacityOrdinanceType",
                table: "Training_Course",
                type: "int",
                nullable: true,
                comment: "Вид на наредбата за правоспособност");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdLegalCapacityOrdinanceType",
                table: "Training_Course");
        }
    }
}
