using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesCandidateProviderPremisesAndCandidateProviderTrainer_AddColumn_InactiveDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "InactiveDate",
                table: "Candidate_ProviderTrainer",
                type: "datetime2",
                nullable: true,
                comment: "Дата на деактивиране на базата");

            migrationBuilder.AddColumn<DateTime>(
                name: "InactiveDate",
                table: "Candidate_ProviderPremises",
                type: "datetime2",
                nullable: true,
                comment: "Дата на деактивиране на базата");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InactiveDate",
                table: "Candidate_ProviderTrainer");

            migrationBuilder.DropColumn(
                name: "InactiveDate",
                table: "Candidate_ProviderPremises");
        }
    }
}
