using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableStartedProcedure_Add_IdCandidate_Provider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TS",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                comment: "Дата на заявката",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NapooReportDeadline",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                comment: "Kраен срок на доклад на експертната комисия",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MeetingHour",
                table: "Procedure_StartedProcedure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Час на заседание в Писмо-покана за заседание на ЕК ",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MeetingDate",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                comment: "Дата на заседание в Писмо-покана за заседание на ЕК ",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Procedure_StartedProcedure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                comment: "Приложение към издадена лицензия - Номер на лицензия",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LicenseDate",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                comment: "Приложение към издадена лицензия - Дата на издаване",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpertReportDeadline",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                comment: "Срок за представяне на доклад на външния експерт ",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdCandidate_Provider",
                table: "Procedure_StartedProcedure",
                type: "int",
                nullable: true,
                comment: "Връзка с  CPO,CIPO - Кандидат Обучаваща институция");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdCandidate_Provider",
                table: "Procedure_StartedProcedure");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TS",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Дата на заявката");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NapooReportDeadline",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Kраен срок на доклад на експертната комисия");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingHour",
                table: "Procedure_StartedProcedure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Час на заседание в Писмо-покана за заседание на ЕК ");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MeetingDate",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Дата на заседание в Писмо-покана за заседание на ЕК ");

            migrationBuilder.AlterColumn<string>(
                name: "LicenseNumber",
                table: "Procedure_StartedProcedure",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true,
                oldComment: "Приложение към издадена лицензия - Номер на лицензия");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LicenseDate",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Приложение към издадена лицензия - Дата на издаване");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpertReportDeadline",
                table: "Procedure_StartedProcedure",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Срок за представяне на доклад на външния експерт ");
        }
    }
}
