using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTableScheduleProcessHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ScheduleProcessHistory",
                columns: table => new
                {
                    IdScheduleProcessHistory = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExecuteDate = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на изпълнение"),
                    RunTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на стариране"),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата на приключване"),
                    Successful = table.Column<bool>(type: "bit", nullable: false, comment: "Статус на изпълнение"),
                    Exception = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Изключение - Грешка")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScheduleProcessHistory", x => x.IdScheduleProcessHistory);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ScheduleProcessHistory");
        }
    }
}
