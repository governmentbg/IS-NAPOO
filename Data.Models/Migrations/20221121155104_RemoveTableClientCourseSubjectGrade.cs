using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class RemoveTableClientCourseSubjectGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Training_ClientCourseSubjectGrade");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
