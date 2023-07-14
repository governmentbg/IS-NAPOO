using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableCourseCommissionMemberChangeFirstNameSize : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Training_CourseCommissionMember",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                comment: "Име",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Име");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                table: "Training_CourseCommissionMember",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Име",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldComment: "Име");
        }
    }
}
