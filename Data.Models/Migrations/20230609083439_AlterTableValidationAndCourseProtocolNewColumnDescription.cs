using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableValidationAndCourseProtocolNewColumnDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Training_ValidationProtocol",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Описание на протокол");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Training_CourseProtocol",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Описание на протокол");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Training_ValidationProtocol");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Training_CourseProtocol");
        }
    }
}
