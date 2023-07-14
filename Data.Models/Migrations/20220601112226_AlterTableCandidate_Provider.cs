using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidate_Provider : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "OnlineTrainingInfo",
                table: "Candidate_Provider",
                type: "bit",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AccessibilityInfo",
                table: "Candidate_Provider",
                type: "bit",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(512)",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateConfirmEMail",
                table: "Candidate_Provider",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateConfirmRequestNAPOO",
                table: "Candidate_Provider",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateRequest",
                table: "Candidate_Provider",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDateRequest",
                table: "Candidate_Provider",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdApplicationStatus",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdTypeApplication",
                table: "Candidate_Provider",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateConfirmEMail",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "DateConfirmRequestNAPOO",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "DateRequest",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "DueDateRequest",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdApplicationStatus",
                table: "Candidate_Provider");

            migrationBuilder.DropColumn(
                name: "IdTypeApplication",
                table: "Candidate_Provider");

            migrationBuilder.AlterColumn<string>(
                name: "OnlineTrainingInfo",
                table: "Candidate_Provider",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccessibilityInfo",
                table: "Candidate_Provider",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 512,
                oldNullable: true);
        }
    }
}
