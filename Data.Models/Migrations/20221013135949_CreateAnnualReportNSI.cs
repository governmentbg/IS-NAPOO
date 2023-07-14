using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateAnnualReportNSI : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arch_AnnualReportNSI",
                columns: table => new
                {
                    IdAnnualReportNSI = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false, comment: "Година"),
                    IdStatus = table.Column<int>(type: "int", nullable: false, comment: "Статус"),
                    SubmissionDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на подаване"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true, comment: "Име на служител подал годишната информация"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arch_AnnualReportNSI", x => x.IdAnnualReportNSI);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arch_AnnualReportNSI");
        }
    }
}
