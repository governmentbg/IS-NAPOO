using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableValidationOrCourseDocumentUploadedFileIDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ValidationClientDocumentStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_ValidationClientDocumentStatus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ClientCourseDocumentStatus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_ClientCourseDocumentStatus",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ValidationClientDocumentStatus");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_ValidationClientDocumentStatus");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ClientCourseDocumentStatus");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_ClientCourseDocumentStatus");
        }
    }
}
