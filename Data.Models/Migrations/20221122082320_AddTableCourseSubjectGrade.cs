using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTableCourseSubjectGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseSubjectGrade",
                columns: table => new
                {
                    IdCourseSubjectGrade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Получател на услугата(обучаем) връзка с курс"),
                    IdCourseSubject = table.Column<int>(type: "int", nullable: true, comment: "Предмет от програма за обучение към курс"),
                    Grade = table.Column<double>(type: "float", nullable: true, comment: "Обща оценка по предмет от учебен план"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseSubjectGrade", x => x.IdCourseSubjectGrade);
                    table.ForeignKey(
                        name: "FK_Training_CourseSubjectGrade_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_CourseSubjectGrade_Training_CourseSubject_IdCourseSubject",
                        column: x => x.IdCourseSubject,
                        principalTable: "Training_CourseSubject",
                        principalColumn: "IdCourseSubject");
                },
                comment: "Оценка по предмет за даден курсист");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseSubjectGrade_IdClientCourse",
                table: "Training_CourseSubjectGrade",
                column: "IdClientCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseSubjectGrade_IdCourseSubject",
                table: "Training_CourseSubjectGrade",
                column: "IdCourseSubject");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseSubjectGrade");
        }
    }
}
