using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class AddTablesClientCourseSubjectGradeAndCourseSubject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseSubject",
                columns: table => new
                {
                    IdCourseSubject = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с курс за обучение, предлагани от ЦПО"),
                    IdProfessionalTraining = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PracticeHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой часове по практика"),
                    TheoryHours = table.Column<double>(type: "float", nullable: false, comment: "Сумарен брой часове по теория"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseSubject", x => x.IdCourseSubject);
                    table.ForeignKey(
                        name: "FK_Training_CourseSubject_Training_Course_IdCourse",
                        column: x => x.IdCourse,
                        principalTable: "Training_Course",
                        principalColumn: "IdCourse",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Предмет от програма за обучение към курс");

            migrationBuilder.CreateTable(
                name: "Training_ClientCourseSubjectGrade",
                columns: table => new
                {
                    IdClientCourseGrade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с Получател на услугата(обучаем) връзка с курс"),
                    IdCourseSubject = table.Column<int>(type: "int", nullable: false, comment: "Предмет от програма за обучение към курс"),
                    Grade = table.Column<double>(type: "float", nullable: false, comment: "Обща оценка по предмет от учебен план"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_ClientCourseSubjectGrade", x => x.IdClientCourseGrade);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourseSubjectGrade_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_ClientCourseSubjectGrade_Training_CourseSubject_IdCourseSubject",
                        column: x => x.IdCourseSubject,
                        principalTable: "Training_CourseSubject",
                        principalColumn: "IdCourseSubject",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Оценка по предмет за даден курсист");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseSubjectGrade_IdClientCourse",
                table: "Training_ClientCourseSubjectGrade",
                column: "IdClientCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_ClientCourseSubjectGrade_IdCourseSubject",
                table: "Training_ClientCourseSubjectGrade",
                column: "IdCourseSubject");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseSubject_IdCourse",
                table: "Training_CourseSubject",
                column: "IdCourse");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_ClientCourseSubjectGrade");

            migrationBuilder.DropTable(
                name: "Training_CourseSubject");
        }
    }
}
