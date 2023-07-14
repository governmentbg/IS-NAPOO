using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterCourseCommissionMemberNewColumnMembersFromOldIs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommissionMembersFromOldIS",
                table: "Training_CourseCommissionMember",
                type: "varchar(MAX)",
                nullable: true,
                comment: "Комисия от стара система на НАПОО");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommissionMembersFromOldIS",
                table: "Training_CourseCommissionMember");
        }
    }
}
