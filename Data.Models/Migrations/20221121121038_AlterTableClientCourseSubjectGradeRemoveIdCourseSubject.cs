using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AlterTableClientCourseSubjectGradeRemoveIdCourseSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Training_ClientCourseSubjectGrade_Training_CourseSubject_IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade");

            migrationBuilder.DropIndex(
                name: "IX_Training_ClientCourseSubjectGrade_IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade");

            migrationBuilder.DropColumn(
                name: "IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Предмет от програма за обучение към курс");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseSubjectGrade_IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade",
                column: "IdCourseSubject");

            migrationBuilder.AddForeignKey(
                name: "FK_Training_ClientCourseSubjectGrade_Training_CourseSubject_IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade",
                column: "IdCourseSubject",
                principalTable: "Training_CourseSubject",
                principalColumn: "IdCourseSubject",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
