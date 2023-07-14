using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCandidate_ProviderSetAccessibilityInfoOnlineTrainingInfoNotNULL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "OnlineTrainingInfo",
                table: "Candidate_Provider",
                type: "bit",
                maxLength: 512,
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 512,
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AccessibilityInfo",
                table: "Candidate_Provider",
                type: "bit",
                maxLength: 512,
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 512,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "OnlineTrainingInfo",
                table: "Candidate_Provider",
                type: "bit",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<bool>(
                name: "AccessibilityInfo",
                table: "Candidate_Provider",
                type: "bit",
                maxLength: 512,
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldMaxLength: 512);
        }
    }
}
