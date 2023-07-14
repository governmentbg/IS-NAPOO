using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class alterTableIndicator_ChangeSize_RangeFormTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RangeTo",
                table: "Rating_Indicator",
                type: "decimal(10,2)",
                nullable: true,
                comment: "Диапазаон до",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldComment: "Диапазаон до");

            migrationBuilder.AlterColumn<decimal>(
                name: "RangeFrom",
                table: "Rating_Indicator",
                type: "decimal(10,2)",
                nullable: true,
                comment: "Диапазаон от",
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)",
                oldNullable: true,
                oldComment: "Диапазаон от");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RangeTo",
                table: "Rating_Indicator",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Диапазаон до",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true,
                oldComment: "Диапазаон до");

            migrationBuilder.AlterColumn<decimal>(
                name: "RangeFrom",
                table: "Rating_Indicator",
                type: "decimal(5,2)",
                nullable: true,
                comment: "Диапазаон от",
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldNullable: true,
                oldComment: "Диапазаон от");
        }
    }
}
