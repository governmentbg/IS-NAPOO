using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTablesCPO_Rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rating_CandidateProviderIndicator",
                columns: table => new
                {
                    IdCandidateProviderIndicator = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdIndicatorType = table.Column<int>(type: "int", nullable: false, comment: "Показател вид"),
                    Year = table.Column<int>(type: "int", nullable: false, comment: "Година"),
                    Points = table.Column<decimal>(type: "decimal(5,2)", nullable: false, comment: "Точки"),
                    Weigh = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Тежест")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating_CandidateProviderIndicator", x => x.IdCandidateProviderIndicator);
                });

            migrationBuilder.CreateTable(
                name: "Rating_Indicator",
                columns: table => new
                {
                    IdIndicator = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false, comment: "Година"),
                    IdIndicatorType = table.Column<int>(type: "int", nullable: false, comment: "Показател вид"),
                    RangeFrom = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Диапазаон от"),
                    RangeTo = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Диапазаон до"),
                    Points = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Точки"),
                    PointsYes = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Точки Да"),
                    PointsNo = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Точки НЕ"),
                    Weigh = table.Column<decimal>(type: "decimal(5,2)", nullable: true, comment: "Тежест"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating_Indicator", x => x.IdIndicator);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropTable(
                name: "Rating_Indicator");
        }
    }
}
