using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableDOCAddEndDateIdStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "DOC_DOC",
                type: "datetime2",
                nullable: false,
                comment: "В сила от",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "DOC_DOC",
                type: "datetime2",
                nullable: true,
                comment: "В сила до");

            migrationBuilder.AddColumn<int>(
                name: "IdStatus",
                table: "DOC_DOC",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Статус");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "DOC_DOC");

            migrationBuilder.DropColumn(
                name: "IdStatus",
                table: "DOC_DOC");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "DOC_DOC",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "В сила от");
        }
    }
}
