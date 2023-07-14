using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableSelfAssessmentReportUploadedFileName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Arch_SelfAssessmentReport",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Arch_SelfAssessmentReport",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Arch_SelfAssessmentReport",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "",
                comment: "Прикачен файл");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Arch_SelfAssessmentReport");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Arch_SelfAssessmentReport");

            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Arch_SelfAssessmentReport");
        }
    }
}
