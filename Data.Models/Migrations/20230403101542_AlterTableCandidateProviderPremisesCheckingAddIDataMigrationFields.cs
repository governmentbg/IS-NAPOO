﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidateProviderPremisesCheckingAddIDataMigrationFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Candidate_ProviderPremisesChecking",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Candidate_ProviderPremisesChecking",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Candidate_ProviderPremisesChecking");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Candidate_ProviderPremisesChecking");
        }
    }
}
