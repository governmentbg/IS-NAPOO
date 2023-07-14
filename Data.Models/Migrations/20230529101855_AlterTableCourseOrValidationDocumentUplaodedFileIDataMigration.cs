using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseOrValidationDocumentUplaodedFileIDataMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ValidationDocumentUploadedFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_ValidationDocumentUploadedFile",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_CourseDocumentUploadedFile",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_CourseDocumentUploadedFile",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ValidationDocumentUploadedFile");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_ValidationDocumentUploadedFile");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_CourseDocumentUploadedFile");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_CourseDocumentUploadedFile");
        }
    }
}
