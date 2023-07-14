using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseAndValidationProtocolIMIgrationData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_ValidationProtocol",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_ValidationProtocol",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MigrationNote",
                table: "Training_CourseProtocol",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OldId",
                table: "Training_CourseProtocol",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_ValidationProtocol");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_ValidationProtocol");

            migrationBuilder.DropColumn(
                name: "MigrationNote",
                table: "Training_CourseProtocol");

            migrationBuilder.DropColumn(
                name: "OldId",
                table: "Training_CourseProtocol");
        }
    }
}
