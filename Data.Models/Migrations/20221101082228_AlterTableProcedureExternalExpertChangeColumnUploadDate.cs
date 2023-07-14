using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedureExternalExpertChangeColumnUploadDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportDate",
                table: "Procedure_ExternalExpert");

            migrationBuilder.AddColumn<DateTime>(
                name: "UploadDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: true,
                comment: "Дата на прикачване на доклада");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadDate",
                table: "Procedure_ExternalExpert");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: true,
                comment: "Дата на изготвяне на доклада");
        }
    }
}
