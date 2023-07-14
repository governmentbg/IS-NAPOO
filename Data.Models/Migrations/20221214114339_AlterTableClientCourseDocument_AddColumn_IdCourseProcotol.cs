using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseDocument_AddColumn_IdCourseProcotol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCourseProtocol",
                table: "Training_ClientCourseDocument",
                type: "int",
                nullable: true,
                comment: "Връзка с протокол от курс за обучение");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseDocument_IdCourseProtocol",
                table: "Training_ClientCourseDocument",
                column: "IdCourseProtocol");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourseDocument_Training_CourseProtocol_IdCourseProtocol",
                table: "Training_ClientCourseDocument",
                column: "IdCourseProtocol",
                principalTable: "Training_CourseProtocol",
                principalColumn: "IdCourseProtocol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourseDocument_Training_CourseProtocol_IdCourseProtocol",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropIndex(
                name: "IX_Training_ClientCourseDocument_IdCourseProtocol",
                table: "Training_ClientCourseDocument");

            migrationBuilder.DropColumn(
                name: "IdCourseProtocol",
                table: "Training_ClientCourseDocument");
        }
    }
}
