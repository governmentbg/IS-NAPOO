using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableAnswer_Points_to_Decimal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Points",
                table: "Assess_Answer",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Точки",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Точки");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Points",
                table: "Assess_Answer",
                type: "int",
                nullable: true,
                comment: "Точки",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldComment: "Точки");
        }
    }
}
