using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Models.Migrations
{
    public partial class CreateTable_CourseProtocolGrade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Training_CourseProtocolGrade",
                columns: table => new
                {
                    IdCourseProtocolGrade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCourseProtocol = table.Column<int>(type: "int", nullable: false, comment: "Връзка с протокол към курс за обучение, предлаган от ЦПО"),
                    IdClientCourse = table.Column<int>(type: "int", nullable: false, comment: "Връзка с протокол към курс за обучение, предлаган от ЦПО"),
                    Grade = table.Column<double>(type: "float", nullable: true, comment: "Оценка от протокол от курс за обучение"),
                    IdCreateUser = table.Column<int>(type: "int", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdModifyUser = table.Column<int>(type: "int", nullable: false),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Training_CourseProtocolGrade", x => x.IdCourseProtocolGrade);
                    table.ForeignKey(
                        name: "FK_Training_CourseProtocolGrade_Training_ClientCourse_IdClientCourse",
                        column: x => x.IdClientCourse,
                        principalTable: "Training_ClientCourse",
                        principalColumn: "IdClientCourse",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Training_CourseProtocolGrade_Training_CourseProtocol_IdCourseProtocol",
                        column: x => x.IdCourseProtocol,
                        principalTable: "Training_CourseProtocol",
                        principalColumn: "IdCourseProtocol",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Протокол към курс за обучение, предлагани от ЦПО");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseProtocolGrade_IdClientCourse",
                table: "Training_CourseProtocolGrade",
                column: "IdClientCourse");

            migrationBuilder.CreateIndex(
                name: "IX_Training_CourseProtocolGrade_IdCourseProtocol",
                table: "Training_CourseProtocolGrade",
                column: "IdCourseProtocol");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Training_CourseProtocolGrade");
        }
    }
}
