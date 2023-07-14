using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedureExternalExpertColumnReportDateNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReportDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: true,
                comment: "Дата на изготвяне на доклада",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Дата на изготвяне на доклада");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReportDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Дата на изготвяне на доклада",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Дата на изготвяне на доклада");
        }
    }
}
