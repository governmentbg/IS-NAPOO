using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderSpecialityAddLicenceData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Candidate_ProviderSpeciality",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Candidate_ProviderSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Candidate_ProviderSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LicenceData",
                table: "Candidate_ProviderSpeciality",
                type: "datetime2",
                nullable: true,
                comment: "Дата на получаване на лицензия за специалността");

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Candidate_ProviderSpeciality",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "LicenceData",
                table: "Candidate_ProviderSpeciality");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Candidate_ProviderSpeciality");
        }
    }
}
