using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTablesCandidateProviderPremisesAndCandidateProviderTrainer_AddColumn_InactiveDates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InactiveDate",
                table: "Candidate_ProviderTrainer",
                type: "datetime2",
                nullable: true,
                comment: "Дата на деактивиране на преподавателя/консултанта",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Дата на деактивиране на базата");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InactiveDate",
                table: "Candidate_ProviderTrainer",
                type: "datetime2",
                nullable: true,
                comment: "Дата на деактивиране на базата",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Дата на деактивиране на преподавателя/консултанта");
        }
    }
}
