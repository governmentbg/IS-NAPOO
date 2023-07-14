using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableTemplateDocument_add_DateFrom_DateTo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DateFrom",
                table: "TemplateDocument",
                type: "datetime2",
                nullable: true,
                comment: "Активна от");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateTo",
                table: "TemplateDocument",
                type: "datetime2",
                nullable: true,
                comment: "Активна до");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateFrom",
                table: "TemplateDocument");

            migrationBuilder.DropColumn(
                name: "DateTo",
                table: "TemplateDocument");
        }
    }
}
