using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesCandidateProviderPremisesSpecialityAndCandidateProviderTrainerSpecialityAddUserData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Candidate_ProviderTrainerSpeciality",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Candidate_ProviderTrainerSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Candidate_ProviderTrainerSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Candidate_ProviderTrainerSpeciality",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Candidate_ProviderPremisesSpeciality",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Candidate_ProviderPremisesSpeciality",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Candidate_ProviderPremisesSpeciality",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Candidate_ProviderTrainerSpeciality");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Candidate_ProviderTrainerSpeciality");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Candidate_ProviderTrainerSpeciality");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Candidate_ProviderTrainerSpeciality");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Candidate_ProviderPremisesSpeciality");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Candidate_ProviderPremisesSpeciality");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Candidate_ProviderPremisesSpeciality");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Candidate_ProviderPremisesSpeciality");
        }
    }
}
