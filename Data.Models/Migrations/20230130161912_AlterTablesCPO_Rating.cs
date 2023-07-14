using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesCPO_Rating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "Rating_CandidateProviderIndicator",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "IdCreateUser",
                table: "Rating_CandidateProviderIndicator",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdModifyUser",
                table: "Rating_CandidateProviderIndicator",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifyDate",
                table: "Rating_CandidateProviderIndicator",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropColumn(
                name: "IdCreateUser",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropColumn(
                name: "IdModifyUser",
                table: "Rating_CandidateProviderIndicator");

            migrationBuilder.DropColumn(
                name: "ModifyDate",
                table: "Rating_CandidateProviderIndicator");
        }
    }
}
