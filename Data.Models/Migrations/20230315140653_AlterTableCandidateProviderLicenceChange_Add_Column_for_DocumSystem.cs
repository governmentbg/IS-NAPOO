using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderLicenceChange_Add_Column_for_DocumSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DS_DATE",
                table: "Candidate_ProviderLicenceChange",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_DocNumber",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_GUID",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DS_ID",
                table: "Candidate_ProviderLicenceChange",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_LINK",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DS_OFFICIAL_DATE",
                table: "Candidate_ProviderLicenceChange",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_OFFICIAL_DocNumber",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_OFFICIAL_GUID",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DS_OFFICIAL_ID",
                table: "Candidate_ProviderLicenceChange",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DS_PREP",
                table: "Candidate_ProviderLicenceChange",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFromDS",
                table: "Candidate_ProviderLicenceChange",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DS_DATE",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_DocNumber",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_GUID",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_ID",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_LINK",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_DATE",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_DocNumber",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_GUID",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_OFFICIAL_ID",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "DS_PREP",
                table: "Candidate_ProviderLicenceChange");

            migrationBuilder.DropColumn(
                name: "IsFromDS",
                table: "Candidate_ProviderLicenceChange");
        }
    }
}
