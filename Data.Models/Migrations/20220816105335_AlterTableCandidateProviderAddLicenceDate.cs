using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderAddLicenceDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LicenceNumber",
                table: "Candidate_Provider",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Номер на лиценза",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenceDate",
                table: "Candidate_Provider",
                type: "datetime2",
                nullable: true,
                comment: "Дата на получаване на лицензия");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LicenceDate",
                table: "Candidate_Provider");

            migrationBuilder.AlterColumn<string>(
                name: "LicenceNumber",
                table: "Candidate_Provider",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Номер на лиценза");
        }
    }
}
