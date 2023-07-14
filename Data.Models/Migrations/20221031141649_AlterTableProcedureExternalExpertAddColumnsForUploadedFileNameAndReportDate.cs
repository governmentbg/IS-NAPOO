using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableProcedureExternalExpertAddColumnsForUploadedFileNameAndReportDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Procedure_ExternalExpert",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Procedure_ExternalExpert",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ReportDate",
                table: "Procedure_ExternalExpert",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Дата на изготвяне на доклада");

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Procedure_ExternalExpert",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                comment: "Прикачен файл");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "ReportDate",
                table: "Procedure_ExternalExpert");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Procedure_ExternalExpert");
        }
    }
}
