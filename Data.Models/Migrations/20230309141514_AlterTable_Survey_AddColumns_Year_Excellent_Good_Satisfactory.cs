using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTable_Survey_AddColumns_Year_Excellent_Good_Satisfactory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Excellent",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Отлично");

            migrationBuilder.AddColumn<int>(
                name: "Good",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Добро");

            migrationBuilder.AddColumn<int>(
                name: "Satisfactory",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Задоволително");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Assess_Survey",
                type: "int",
                nullable: true,
                comment: "Година");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Excellent",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "Good",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "Satisfactory",
                table: "Assess_Survey");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Assess_Survey");
        }
    }
}
